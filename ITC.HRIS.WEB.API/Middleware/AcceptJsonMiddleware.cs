using Newtonsoft.Json;

namespace ITC.Hris.Web.API.Middleware
{
    public class AcceptJsonMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] _excludedPaths = new[]
               {
                   "/api/security/security/auth-user"
                };

        public AcceptJsonMiddleware(RequestDelegate next) 
        { 
            _next = next;
        }
       
        
        public async Task Invoke(HttpContext context)
        {
            // Check if the Accept header contains "application/json"
            var acceptHeader = context.Request.Headers["Accept"].ToString();
            string path = context.Request.Path.Value?.ToLower() ?? "";

            if (_excludedPaths.Contains(path))
            {
                await _next(context);
                return;
            }
            if (!string.IsNullOrEmpty(acceptHeader) && acceptHeader.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                var jsonData = new
                {
                    code = "401",
                    message = "Accept header must be set to application/json",
                    data = ""
                };
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(jsonData));
            }

        }
    }
}
