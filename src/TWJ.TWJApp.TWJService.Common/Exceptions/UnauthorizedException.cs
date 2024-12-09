using System;

namespace TWJ.TWJApp.TWJService.Common.Exceptions
{
    public class Unauthorized : Exception
    {
        public Unauthorized() : base("Unauthorized")
        {
        }
    }
}
