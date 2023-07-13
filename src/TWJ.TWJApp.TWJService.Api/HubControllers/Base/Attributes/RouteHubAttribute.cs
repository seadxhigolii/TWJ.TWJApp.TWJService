using System;

namespace TWJ.TWJApp.TWJService.Api.HubControllers.Base.Attributes
{
    public class RouteHubAttribute : Attribute
    {
        public string _path { get; private set; }
        public RouteHubAttribute(string path)
        {
            _path = path;
        }
    }
}
