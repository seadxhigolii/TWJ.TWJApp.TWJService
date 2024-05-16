using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Helpers.Interfaces
{
    public interface IGlobalHelperService
    {
        Task<bool> Log(Exception ex, string className, [CallerMemberName] string methodName = "");
        string TitleToUrlSlug(string title);
        string RemoveTextWithinSquareBrackets(string title);
    }
}
