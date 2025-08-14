using ITC.Hris.Application;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure
{
    public class AuthService : IAuth
    {
        private readonly IhelpDbLiveContext _ihelpdb;
        public AuthService (IhelpDbLiveContext ihelpdb)
        {
            _ihelpdb = ihelpdb;
        }
        public async Task<WebUserResponse> LoginWebUser(AuthenticationRequest request)
        {
            #region Login User
            try
            {
                var parameters = new SqlParameter[]
                 {
                     new SqlParameter("@loginName", SqlDbType.NVarChar, 128) { Value = request.UserName },
                     new SqlParameter("@password", SqlDbType.NVarChar, 50) { Value = request.Password }
                };

                // Execute stored procedure and bring results to memory
                var responses = await _ihelpdb.WebUserResponse
                    .FromSqlRaw("EXEC V2_usp_check_user_login @loginName, @password", parameters)
                    .ToListAsync();

                var response = responses.FirstOrDefault();

                if (response == null || response.UserId <= 0)
                {
                    return new WebUserResponse();
                }

                return response;


            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            #endregion
        }
    }
}
