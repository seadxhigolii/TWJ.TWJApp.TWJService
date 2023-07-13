using System;

namespace TWJ.TWJApp.TWJService.Common.Exceptions
{
    public class DeleteRestrictedException : Exception
    {
        public DeleteRestrictedException(string message) : base(message)
        {
        }
    }
}
