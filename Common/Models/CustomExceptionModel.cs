using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CustomExceptionModel:Exception
    {
        public int ErrorCode { get; set; }

        public CustomExceptionModel(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
