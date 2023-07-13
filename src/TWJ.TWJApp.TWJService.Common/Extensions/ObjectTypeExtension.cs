using System;

namespace TWJ.TWJApp.TWJService.Common.Extensions
{
    public static class ObjectTypeExtension
    {
        public static bool HasProperty(this Type type, string propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }
    }
}
