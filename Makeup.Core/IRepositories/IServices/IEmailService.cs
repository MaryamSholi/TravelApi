using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.IRepositories.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string ToEmail, string subject, string message);
    }
}
