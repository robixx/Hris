using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application
{
    public interface IAuth
    {
        Task<WebUserResponse> LoginWebUser(AuthenticationRequest request);
    }
}
