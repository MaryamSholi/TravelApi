using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.Entities.DTO
{
    public class LoginResponseDTO
    {
        public LocalUserDTO User { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
