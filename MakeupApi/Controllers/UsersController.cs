using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel.Core.Entities.DTO;
using Travel.Core.Entities;
using Travel.Core.IRepositories;
using Microsoft.AspNetCore.Identity;
using Travel.Services;
using Travel.Core.IRepositories.IServices;

namespace Travel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly UserManager<LocalUser> userManager;
        private readonly IEmailService emailService;

        public UsersController(IUsersRepository usersRepository, UserManager<LocalUser> userManager, IEmailService emailService)
        {
            this.usersRepository = usersRepository;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            try
            {
                bool uniqueEmail = usersRepository.IsUniqueUser(model.Email);
                if (!uniqueEmail)
                {
                    return BadRequest(new ApiResponse(400, "Email already exist !"));
                }
                var user = await usersRepository.Register(model);
                if (user == null)
                {
                    return BadRequest(new ApiResponse(400, "Error While registration user"));
                }
                else
                {
                    return Ok(new ApiResponse(201, result: user));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiValidationResponse(new List<string>() { ex.Message, "ann error occurred while processing your request" }));
            }
        }
       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await usersRepository.Login(model);
                if (user.User == null)
                {
                    return Unauthorized(new ApiValidationResponse(new List<string>() { "Email or password inCorrect " }, 401));
                }
                return Ok(user);
            }
            return BadRequest(new ApiValidationResponse(new List<string>() { "please try to enter the email and password correctly" }, 400));
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmailForUser([FromQuery] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return BadRequest(new ApiValidationResponse(new List<string> { $"This Email {email} not found!!" }));

            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var ForgetPasswordLink = Url.Action("ResetPassword", "Users", new { token = token, email = user.Email}, Request.Scheme);
            var subject = "Reset Password Request";
            var message = $"Please click on the link to reset your password : {ForgetPasswordLink}";

            await emailService.SendEmailAsync(user.Email, subject, message);
            return Ok(new ApiResponse(200, "password reset link has been sent to your email. Please Check Your Email"));
        }
       
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (string.Compare(model.NewPassword, model.ConfirmNewPassword) != 0)
                {
                    return BadRequest(new ApiResponse(400, "passwords not match"));
                }
                if (string.IsNullOrEmpty(model.Token))
                {
                    return BadRequest(new ApiResponse(400, "Token invalid"));
                }

                var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new ApiResponse(200, "Password reset successfully"));
                }
                else
                {
                    return BadRequest(new ApiResponse(400, "Error while resting password"));
                }

            }
            return BadRequest(new ApiResponse(400, "check your Information"));

        }
      
        [HttpPost("resetToken")]
        public async Task<IActionResult> TokenToResetPassword([FromBody] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return NotFound(new ApiResponse(404));
            }
            var token = userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(new { token = token });
        }


    }
}
