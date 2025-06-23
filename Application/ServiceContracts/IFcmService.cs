using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceContracts
{
    public interface IFcmService
    {
        Task SendNotificationAsync(string title, string body);

    }
}
