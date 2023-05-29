using POS.Common.Constants;
using POS.Common.DTO;
using POS.Common.Models;
using POS.Services;
using POS.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.Auth
{
    public class POSAuthorization
    {
        private readonly RequestDelegate _next;

        public POSAuthorization(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IUserSessionService userSessionService, ISecurityService securityService)
        {
            string url = httpContext.Request.Path;

            if (url.ToLower() == CommonPath.registerUrl)
            {
                await _next(httpContext);
            }
            else
            {
                var handler = new JwtSecurityTokenHandler();
                var headerToken = SubstringToken(httpContext.Request.Headers[HttpHeaders.Token]);
                var token = handler.ReadToken(headerToken) as JwtSecurityToken;
                if (token != null)
                {
                    RequestMessage objRequestMessage = new RequestMessage();
                    ResponseMessage objResponseMessage = new ResponseMessage();

                    objRequestMessage.RequestObj = GetSystemUserId(token);
                    objResponseMessage = await userSessionService.GetUserSessionBySystemUserId(objRequestMessage);
                    try
                    {

                        UserSession objUserSession = objResponseMessage?.ResponseObj as UserSession;

                        if (objUserSession != null)
                        {
                            TimeSpan ts = DateTime.Now - objUserSession.SessionEnd.Value;
                            int min = ts.Minutes;
                            if (min <= CommonConstant.SessionExpired)
                            {
                                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                                {
                                    var obj = await reader.ReadToEndAsync();

                                    objRequestMessage = JsonConvert.DeserializeObject<RequestMessage>(obj);

                                    objRequestMessage.UserID = GetSystemUserId(token);
                                }

                                // for set user id in request boby.                            
                                var requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objRequestMessage));
                                httpContext.Request.Body = new MemoryStream(requestData);
                                httpContext.Request.ContentLength = httpContext.Request.Body.Length;


                                if (await securityService.CheckPermission(url))
                                {

                                    //update session
                                    RequestMessage objRequestMessageNew = new RequestMessage();

                                    DateTime dateTime = DateTime.Now.AddMinutes(CommonConstant.SessionExpired);
                                    objUserSession.SessionEnd= dateTime;
                                    objRequestMessageNew.RequestObj =JsonConvert.SerializeObject(objUserSession);

                                    await userSessionService.SaveUserSession(objRequestMessageNew);
                                    await _next(httpContext);
                                }
                                else
                                {
                                    httpContext.Response.ContentType = "application/json";
                                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    await httpContext.Response.WriteAsJsonAsync("You have no permission.");
                                }

                            }
                            else
                            {
                                httpContext.Response.ContentType = "application/json";
                                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                await httpContext.Response.WriteAsJsonAsync("Session expired.");

                            }
                        }
                        else
                        {
                            httpContext.Response.ContentType = "application/json";
                            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await httpContext.Response.WriteAsJsonAsync("Any active session not found.");
                        }


                    }
                    catch
                    {
                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await httpContext.Response.WriteAsJsonAsync("Internal server error.");
                    }
                }
                else
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync("Unauthorize");
                }
            }
            
        }


        private string SubstringToken(string fullToken)
        {
            return fullToken.Replace("Bearer ", "");
        }
        /// <summary>
        /// get user id from token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private int GetSystemUserId(JwtSecurityToken token)
        {
            return Convert.ToInt32(token.Claims.First(claim => claim.Type == JwtClaim.UserId).Value);
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class POSAuthorizationExtensions
    {
        public static IApplicationBuilder UsePOSAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<POSAuthorization>();
        }
    }
}
