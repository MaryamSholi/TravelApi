using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.Entities.DTO
{
    public class ApiValidationResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationResponse(IEnumerable<string> errors = null, int? statusCode = 400) : base(statusCode)
        {
            Errors = errors ?? new List<string>();
        }
    }
}
