using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Enums
{
    public class Enums
    {
        public enum Status
        {
            Active = 1,
            Inactive = 2,
            Delete = 9
        }
        public enum ResponseCode
        {
            Success = 1,
            Failed = 2,
            Warning = 3
        }
        public enum ActionType
        {
            Insert = 1,
            Update = 2,
            View = 3,
            Delete = 4,
            Login = 5,
            Register = 6,
            Logout = 7
        }
        public enum Gender
        {
            Male = 1,
            Famale = 2,
            Other = 3
        }
        public enum TaxType
        {
            Exclusive = 1,
            Inclusive = 2
        }
        public enum DiscountType
        {
            Percentage = 1,
            Fixed = 2
        }
    }
}
