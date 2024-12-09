using System;
using System.Drawing;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using static AngleSharp.Css.Values.Angle;

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
        Task<Unit> PostToInstagramAsync(string imageUrl, string quoteContent, string caption, CancellationToken cancellationToken);
        Task<Unit> PostToFacebookAsync(string imageUrl, string quoteContent, string caption, CancellationToken cancellationToken);
        Task<Unit> PostToTwitterAsync(string imageUrl, string caption, CancellationToken cancellationToken = default);
        Task<Unit> PostReelToInstagramAsync(string videoUrl, string caption, CancellationToken cancellationToken = default);
        (byte[] Key, byte[] IV) GenerateAesKeys();
        byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV);
        string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV);
    }
}
