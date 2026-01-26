using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Errors
{
    public class Error
    {
        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; set; }=null!;
        public string Message { get; set; }=null!;
        public static Error ErrorFactory(string code,string message)
            => new (code, message);
    }
}
