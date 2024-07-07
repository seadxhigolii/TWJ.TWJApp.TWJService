using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Helpers.Interfaces
{
    public interface IGlobalHelperService
    {
        Task<User> GetUserFromCache();
        Task<bool> Log(Exception ex, string className, [CallerMemberName] string methodName = "");
        string TitleToUrlSlug(string title);
        string RemoveTextWithinSquareBrackets(string title);
        int CalculateFontSize(string text, RectangleF rect);
        void DrawTextWithSpacing(Graphics graphics, string text, Font font, Brush brush, RectangleF rect, float letterSpacing, float spaceSpacing, StringFormat format);
    }
}
