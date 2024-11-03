using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.Entities.DTO;

namespace Travel.Core.IRepositories
{
    public interface IUsersRepository 
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
        bool IsUniqueUser(string username);
    }
}
