using System;

namespace TWJ.TWJApp.TWJService.Common.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException() : base("You have no permission for this action!")
        {
        }
    }
}
