using System;

namespace TWJ.TWJApp.TWJService.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string msg) : base(msg)
        {

        }
    }
}
