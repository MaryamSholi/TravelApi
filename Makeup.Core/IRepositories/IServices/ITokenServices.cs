using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.Entities;

namespace Travel.Core.IRepositories.IServices
{
    public interface ITokenServices
    {
        Task<string> CreateTokenAsync(LocalUser localUser);

    }
}
