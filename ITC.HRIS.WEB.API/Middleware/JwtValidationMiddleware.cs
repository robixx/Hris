using ITC.Hris.Infrastructure;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ITC.Hris.Web.API.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly string _expectedIssuer;
        private readonly string _expectedAudience;
        private static readonly string[] _excludedPaths = new[]
        {
            "/", "/api/accounts/security/auth-user"
        };
        public JwtValidationMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
            _ = _config;
            string? _tempIssuer = config["Jwt:Issuer"];
            if (string.IsNullOrEmpty(_tempIssuer))
            {
                _expectedIssuer = "http://localhost:80";
            }
            else
            {
                _expectedIssuer = _tempIssuer;
            }

            string? _tempAudience = config["Jwt:Audience"];
            if (string.IsNullOrEmpty(_tempAudience))
            {
                _expectedAudience = "http://localhost:80";
            }
            else
            {
                _expectedAudience = _tempAudience;
            }
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value?.ToLower() ?? "";

            // Skip path validation
            if (_excludedPaths.Contains(path))
            {
                await _next(context);
                return;
            }
            // Check Identity is Found
            if (context.User?.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            {
                var json = JsonConvert.SerializeObject(new
                {
                    code = "301",
                    message = "Access Token Error",
                    data = ""
                });
                await context.Response.WriteAsync(json);
                return;
            }

            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                {

                    var jtiClaim = identity.FindFirst("JWTId")?.Value;
                    if (string.IsNullOrEmpty(jtiClaim))
                    {
                        var json = JsonConvert.SerializeObject(new
                        {
                            code = "303",
                            message = "JWTId (jti) not found in token",
                            data = new { value = "" }
                        });
                       
                        await context.Response.WriteAsync(json);
                        return;
                    }

                    var user = JwtConfig.ReadToken(identity);

                    // Validate JWT Token Expiery
                    if (user == null || user.TokenExpired < DateTime.UtcNow)
                    {
                        var json = JsonConvert.SerializeObject(new
                        {
                            code = "302",
                            message = "JWT token expired",
                            data = new { value = "" }
                        });
                       
                        await context.Response.WriteAsync(json);
                        return;
                    }
                    #region Check User Access to the Path
                   

                    #endregion Check User Access to the Path

                    #region Temp Blocked for Fast Report


                   
                    #endregion Temp Blocked for Fast Report
                }
                
                await _next(context);
            }
            catch (Exception ex)
            {
             
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var json = JsonConvert.SerializeObject(new
                {
                    code = "303",
                    message = "Cannot handle token",
                    data = new { value = ex.StackTrace }
                });

                await context.Response.WriteAsync(json);
                return;
            }
        }
    }
}
