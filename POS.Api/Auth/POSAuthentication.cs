using POS.Common.Constants;
using POS.Common.DTO;
using POS.Common.Enums;
using POS.Common.Models;
using POS.Common.VM;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POS.API.Auth
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class POSAuthentication
    {
        private readonly RequestDelegate _next;

        public POSAuthentication(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IAccessTokenService accessTokenService, ISecurityService securityService)
        {
            string url = httpContext.Request.Path;
            if (url?.ToLower() == CommonPath.loginUrl)
            {
                ResponseMessage objResponseMessage = new ResponseMessage();
                RequestMessage objRequestMessage = new RequestMessage();
                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                {
                    try
                    {
                        var obj = await reader.ReadToEndAsync();
                        objRequestMessage = JsonConvert.DeserializeObject<RequestMessage>(obj);

                        //for getting user info

                        objResponseMessage = await securityService.Login(objRequestMessage);

                        if (objResponseMessage != null && objResponseMessage.ResponseObj != null)
                        {

                            VMLogin objVMLogin = objResponseMessage.ResponseObj as VMLogin;

                            if (objVMLogin != null)
                            {

                                AccessToken objAccessToken = new AccessToken();
                                objAccessToken.SystemUserID = objVMLogin.SystemUserID;
                                objAccessToken.RoleID = objVMLogin.RoleID;

                                //for creating sesssion token
                                AccessToken accessToken = await accessTokenService.Create(objAccessToken);
                                objVMLogin.Token = (accessToken != null) ? accessToken.Token : String.Empty;

                                objVMLogin.SystemUserID = 0;
                                objVMLogin.RoleID = 0;
                                objResponseMessage.ResponseObj = objVMLogin;
                                objResponseMessage.ResponseCode = (int)Enums.ResponseCode.Success;
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = false
                                };

                                await httpContext.Response.WriteAsJsonAsync(objResponseMessage, options);
                            }
                            else
                            {
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = false
                                };

                                await httpContext.Response.WriteAsJsonAsync(objResponseMessage, options);
                            }

                        }
                        else
                        {
                            httpContext.Response.ContentType = "application/json";
                            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await httpContext.Response.WriteAsJsonAsync(MessageConstant.Unauthorizerequest);
                        }
                    }
                    catch
                    {
                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await httpContext.Response.WriteAsJsonAsync(MessageConstant.InternalServerError);
                    }
                }

            }
            else
            {
                await _next(httpContext);
            }


        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class POSAuthenticationExtensions
    {
        public static IApplicationBuilder UsePOSAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<POSAuthentication>();
        }
    }
}
