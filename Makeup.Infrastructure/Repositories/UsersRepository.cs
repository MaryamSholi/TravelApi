using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.Entities.DTO;
using Travel.Core.Entities;
using Travel.Core.IRepositories;
using Travel.Infrastructure.Data;
using AutoMapper;
using Travel.Core.IRepositories.IServices;

namespace Travel.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<LocalUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly SignInManager<LocalUser> signInManager;
        private readonly ITokenServices tokenServices;

        public UsersRepository(ApplicationDbContext dbContext, UserManager<LocalUser> userManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper, SignInManager<LocalUser> signInManager, ITokenServices tokenServices)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.tokenServices = tokenServices;
        }
        public bool IsUniqueUser(string Email)
        {
            var result = dbContext.Users.FirstOrDefault(x => x.Email == Email);
            return result == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDTO.Email);
            var checkPassword = await signInManager.CheckPasswordSignInAsync(user, loginRequestDTO.Password, false);

            if (!checkPassword.Succeeded)
            {
                return new LoginResponseDTO()
                {
                    User = null,
                    Token = ""
                };
            }
            var role = await userManager.GetRolesAsync(user);
            return new LoginResponseDTO()
            {
                User = mapper.Map<LocalUserDTO>(user),
                Token = await tokenServices.CreateTokenAsync(user),
                Role = role.FirstOrDefault()
            };
        }

        public async Task<LocalUserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var user = new LocalUser
            {
                Email = registrationRequestDTO.Email,
                UserName = registrationRequestDTO.Email.Split('@')[0],
                FirstName = registrationRequestDTO.Fname,
                LastName = registrationRequestDTO.Lname,
                Address = registrationRequestDTO.Address,
            };

            using (var transaction = await dbContext.Database.BeginTransactionAsync())//begin connection after this i need to disbause the conn
            {
                try
                {
                    var result = await userManager.CreateAsync(user, registrationRequestDTO.Password);
                    if (result.Succeeded)
                    {
                        //to check if the roles that user entered existing or not
                        var role = await roleManager.RoleExistsAsync(registrationRequestDTO.Role);
                        if (!role)
                        {
                            throw new Exception($"The role {registrationRequestDTO.Role} doesn't exist");
                        }

                        var userRoleResult = await userManager.AddToRoleAsync(user, registrationRequestDTO.Role);
                        if (userRoleResult.Succeeded)
                        {
                            var userReturn = dbContext.Users.FirstOrDefault(u => u.Email == registrationRequestDTO.Email);
                            await transaction.CommitAsync();//after everything is ok
                            return mapper.Map<LocalUserDTO>(userReturn);
                        }
                        else
                        {
                            await transaction.RollbackAsync();//if their is sth wrong(if adding to userroles fails)
                            throw new Exception("Failed to add user to userRoles");
                        }

                    }
                    else
                    {
                        await transaction.RollbackAsync();//if their is sth wrong(if adding user fails)
                        throw new Exception("User Registerated failed");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }

            }

        }
    }
}
