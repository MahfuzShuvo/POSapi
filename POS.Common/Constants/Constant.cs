using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Constants
{
    public static class MessageConstant
    {
        public const string SavedSuccessfully = "Saved successfully";
        public const string RegisterSuccessfully = "Register successfully";
        public const string SaveFailed = "Failed to save information";
        public const string DeleteFailed = "Failed to delete";
        public const string DeleteSuccess = "Delete successfully";
        public const string Token = "Token is required";
        public const string Unauthorizerequest = "Unauthorize request";
        public const string InternalServerError = "Internal server error";
        public const string LoginSuccess = "Logged in successfully";
        public const string LogOutSuccessfully = "Logout successfully";
        public const string Invaliddatafound = "Invalid data found";
    }

    public static class CommonConstant
    {
        public static DateTime DeafultDate = Convert.ToDateTime("1900/01/01");
        public static int SessionExpired = 30;
        public static string NoImage = "no-image.png";
    }

    public static class CommonPath
    {
        public const string loginUrl = "/api/security/login";
        public const string registerUrl = "/api/security/register";
    }
    public class HttpHeaders
    {
        public const string Token = "Authorization";
        public const string AuthenticationSchema = "Bearer";
    }
}
