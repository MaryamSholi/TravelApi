using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.Entities;

namespace Travel.Core.IRepositories
{
    public interface IDestinationRepository : IGenericRepository<Destination>
    {
        public Task<IEnumerable<Destination>> GetWithHotelsAsync();

    }
}
