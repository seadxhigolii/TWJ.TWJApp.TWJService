using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using TWJ.TWJApp.TWJService.Application.Interfaces.Amazon;
using TWJ.TWJApp.TWJService.Application.Interfaces.Video;

namespace TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.Add
{
    public class AddInstagramPostCommandHandler : IRequestHandler<AddInstagramPostCommand, Unit>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOpenAiService _openAiService;
        private readonly IAmazonS3Service _amazonS3Service;
        private readonly IVideoService _videoService;

        private readonly string _accessToken;

        public AddInstagramPostCommandHandler(
            ITWJAppDbContext context,
            IGlobalHelperService globalHelper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IOpenAiService openAiService,
            IAmazonS3Service amazonS3Service,
            IVideoService videoService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _accessToken = _configuration["Instagram:LongLiveAccessToken"];
            _openAiService = openAiService;
            _amazonS3Service = amazonS3Service;
            _videoService = videoService;
        }

        public async Task<Unit> Handle(AddInstagramPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.IsVideo == true)
                {
                    var video = await _videoService.GenerateVideo(request.Type, cancellationToken);
                }
                else
                {
                    var instagramTemplate = await _context.InstagramPosts.Where(x => x.Type == request.Type).FirstOrDefaultAsync();

                    string imageUrl = instagramTemplate.Image;
                    string fileName = $"{Guid.NewGuid()}-template-{request.Type}.jpg";

                    var quotes = await _context.Quotes
                        .Where(q => (q.Category == "Health" || q.Category == "Motivational") && q.Length < 460)
                        .ToListAsync(cancellationToken);

                    var random = new Random();
                    var quote = quotes[random.Next(quotes.Count)];

                    if (!quotes.Any())
                    {
                        throw new ApplicationException("No suitable quote found.");
                    }

                    string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string apiDirectory = Path.Combine(Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName);

                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(imageUrl);
                        response.EnsureSuccessStatusCode();

                        using (var ms = await response.Content.ReadAsStreamAsync())
                        {
                            using (var image = Image.FromStream(ms))
                            {
                                using (Graphics graphics = Graphics.FromImage(image))
                                {
                                    Font font = null;
                                    RectangleF quoteRect = RectangleF.Empty;
                                    RectangleF dateRect = RectangleF.Empty;
                                    SolidBrush brush = null;
                                    PrivateFontCollection privateFonts = new PrivateFontCollection();
                                    Font authorFont = null;
                                    RectangleF authorRect = RectangleF.Empty;
                                    FontFamily fontFamily = null;
                                    string customFontPath = "";
                                    int fontSize = 0;
                                    Color color = Color.Empty;

                                    switch (request.Type)
                                    {
                                        case 1:
                                            fontFamily = new FontFamily("Times New Roman");
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(303, 315, 470, 457);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font("Lovelace", 14, FontStyle.Regular);
                                            authorRect = new RectangleF(330, 794, 451, 25);
                                            break;
                                        case 2:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Poppins Regular.otf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.White);
                                            dateRect = new RectangleF(241, 186, 579, 58);
                                            quoteRect = new RectangleF(103, 308, 856, 521);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Regular);
                                            authorRect = new RectangleF(266, 874, 550, 59);
                                            break;
                                        case 3:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Lovelace Text Regular.otf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.White);
                                            quoteRect = new RectangleF(176, 192, 726, 589);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 14, FontStyle.Regular);
                                            authorRect = new RectangleF(252, 874, 541, 29);
                                            break;
                                        case 4:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Cardo Regular.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.White);
                                            quoteRect = new RectangleF(93, 340, 862, 474);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 20, FontStyle.Regular);
                                            authorRect = new RectangleF(324, 899, 431, 38);
                                            break;
                                        case 5:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Lovelace Text Regular.otf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(144, 194, 791, 546);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 20, FontStyle.Regular);
                                            authorRect = new RectangleF(382, 858, 315, 32);
                                            break;
                                        case 6:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "BlackMango Regular.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.White);
                                            quoteRect = new RectangleF(193, 200, 692, 678);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 18, FontStyle.Regular);
                                            authorRect = new RectangleF(350, 968, 383, 27);
                                            break;
                                        case 7:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "NotoSerif Regular.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.White);
                                            quoteRect = new RectangleF(183, 555, 709, 361);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 20, FontStyle.Regular);
                                            authorRect = new RectangleF(326, 916, 418, 45);
                                            break;
                                        case 8:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "PublicSans Regular.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(167, 379, 753, 399);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 20, FontStyle.Regular);
                                            authorRect = new RectangleF(361, 823, 332, 49);
                                            break;
                                        case 9:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Lovelace Text Regular.otf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(228, 386, 631, 375);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 16, FontStyle.Regular);
                                            authorRect = new RectangleF(328, 844, 420, 40);
                                            break;
                                        case 10:
                                            fontFamily = new FontFamily("Times New Roman");
                                            brush = new SolidBrush(Color.White);
                                            quoteRect = new RectangleF(139, 141, 785, 683);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 20, FontStyle.Regular);
                                            authorRect = new RectangleF(347, 898, 394, 80);
                                            break;
                                        case 11:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Flagoriacalintha Regular.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.White);
                                            quoteRect = new RectangleF(98, 176, 886, 596);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Regular);
                                            authorRect = new RectangleF(284, 813, 527, 54);
                                            break;
                                        case 12:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Gotham Medium.otf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.White);
                                            quoteRect = new RectangleF(200, 325, 676, 463);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Regular);
                                            authorRect = new RectangleF(335, 855, 403, 52);
                                            break;
                                        case 13:
                                            color = ColorTranslator.FromHtml("#4B4130");
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "Bestaline Sans.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(color);
                                            quoteRect = new RectangleF(245, 364, 617, 386);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Regular);
                                            authorRect = new RectangleF(289, 844, 523, 46);
                                            break;
                                        case 14:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "KisahHororSobat.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(115, 207, 838, 527);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Regular);
                                            authorRect = new RectangleF(250, 810, 568, 55);
                                            break;
                                        case 15:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "DelicateSans.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(124, 365, 831, 421);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Bold);
                                            authorRect = new RectangleF(120, 841, 522, 57);
                                            break;
                                        case 16:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "DelicateSans.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(83, 245, 913, 462);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Bold);
                                            authorRect = new RectangleF(327, 797, 424, 63);
                                            break;
                                        case 17:
                                            customFontPath = Path.Combine(apiDirectory, "Fonts", "DelicateSans.ttf");
                                            customFontPath = Path.GetFullPath(customFontPath);
                                            privateFonts.AddFontFile(customFontPath);

                                            fontFamily = privateFonts.Families[0];
                                            brush = new SolidBrush(Color.Black);
                                            quoteRect = new RectangleF(74, 225, 934, 625);
                                            fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                                            authorFont = new Font(fontFamily, 23, FontStyle.Bold);
                                            authorRect = new RectangleF(237, 879, 609, 70);
                                            break;
                                        default:
                                            break;
                                    }

                                    font = new Font(fontFamily, fontSize, FontStyle.Regular);

                                    var stringFormat = new StringFormat
                                    {
                                        Alignment = StringAlignment.Center,
                                        LineAlignment = StringAlignment.Center
                                    };

                                    if (request.Type == 15 || request.Type == 16)
                                    {
                                        font = new Font(fontFamily, fontSize, FontStyle.Bold);
                                        graphics.DrawString(quote.Content, font, brush, quoteRect, stringFormat);
                                    }
                                    else
                                    {
                                        graphics.DrawString(quote.Content, font, brush, quoteRect, stringFormat);
                                    }

                                    if (request.Type == 3)
                                    {
                                        _globalHelper.DrawTextWithSpacing(graphics, quote.Author, authorFont, brush, authorRect, 2, 10, stringFormat);
                                    }
                                    else if (request.Type == 5 || request.Type == 6 || request.Type == 7)
                                    {
                                        brush = new SolidBrush(Color.White);
                                        graphics.DrawString(quote.Author, authorFont, brush, authorRect, stringFormat);
                                    }
                                    else if (request.Type == 8)
                                    {
                                        color = ColorTranslator.FromHtml("#3E641D");
                                        brush = new SolidBrush(color);
                                        graphics.DrawString(quote.Author, authorFont, brush, authorRect, stringFormat);
                                    }
                                    else if (request.Type == 2)
                                    {
                                        DateTime now = DateTime.Now;
                                        string formattedDate = now.ToString("MMMM dd, yyyy, HH:mm");
                                        float dateFontSize = 22f;
                                        Font dateFont = new Font(fontFamily, dateFontSize, FontStyle.Regular);

                                        graphics.DrawString(formattedDate, dateFont, brush, dateRect, stringFormat);
                                        graphics.DrawString(quote.Author, authorFont, brush, authorRect, stringFormat);

                                        dateFont.Dispose();
                                    }

                                    else
                                    {
                                        graphics.DrawString(quote.Author, authorFont, brush, authorRect, stringFormat);
                                    }
                                }
                                //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                                string filePath = fileName;

                                image.Save(filePath, ImageFormat.Jpeg);

                                //var s3Path = await _amazonS3Service.UploadFileToS3Async(filePath, "Instagram Posts", fileName);
                                //await PostToInstagramAsync(s3Path, quote.Content, cancellationToken);
                            }
                        }
                    }
                }
                return Unit.Value;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred: " + ex.Message);
            }
        }


        public async Task<Unit> PostToInstagramAsync(string imageUrl, string quoteContent, CancellationToken cancellationToken)
        {
            string currentClassName = nameof(AddInstagramPostCommandHandler);

            string captionPrompt =$"Generate an Instagram Caption regarding this quote: \"{quoteContent}\"";
            var caption = await _openAiService.GenerateSectionAsync(captionPrompt, cancellationToken);
            string pageId = "374426615745709";

            try
            {
                var client = _httpClientFactory.CreateClient();

                string businessAccountRequestUrl = $"https://graph.facebook.com/v20.0/{pageId}?fields=instagram_business_account&access_token={_accessToken}";

                var businessAccountResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.GetAsync(businessAccountRequestUrl, cancellationToken);
                });

                var businessAccountContent = await businessAccountResponse.Content.ReadAsStringAsync();

                if (!businessAccountResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error retrieving Instagram Business Account ID: {businessAccountContent}";
                    await _globalHelper.Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                var businessAccountId = JObject.Parse(businessAccountContent)["instagram_business_account"]["id"].ToString();

                var createMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media";
                var createMediaContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("image_url", imageUrl),
                    new KeyValuePair<string, string>("caption", caption),
                    new KeyValuePair<string, string>("access_token", _accessToken)
                });

                var createMediaResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.PostAsync(createMediaUrl, createMediaContent, cancellationToken);
                });

                var createMediaResponseContent = await createMediaResponse.Content.ReadAsStringAsync();

                if (!createMediaResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error creating media object: {createMediaResponseContent}";
                    await _globalHelper.Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                var mediaObjectId = JObject.Parse(createMediaResponseContent)["id"].ToString();

                var publishMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media_publish";
                var publishMediaContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("creation_id", mediaObjectId),
                    new KeyValuePair<string, string>("access_token", _accessToken)
                });

                var publishMediaResponse = await SendHttpRequestWithRetry(async () =>
                {
                    return await client.PostAsync(publishMediaUrl, publishMediaContent, cancellationToken);
                });

                var publishMediaResponseContent = await publishMediaResponse.Content.ReadAsStringAsync();

                if (!publishMediaResponse.IsSuccessStatusCode)
                {
                    var errorMessage = $"Error publishing media object: {publishMediaResponseContent}";
                    await _globalHelper.Log(new Exception(errorMessage), currentClassName);
                    throw new ApplicationException(errorMessage);
                }

                return Unit.Value;
            }
            catch (HttpRequestException ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new ApplicationException("Error posting to Instagram: " + ex.Message);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new ApplicationException("An unexpected error occurred: " + ex.Message);
            }
        }

        private async Task<HttpResponseMessage> SendHttpRequestWithRetry(Func<Task<HttpResponseMessage>> httpRequestFunc, int maxRetries = 3)
        {
            int retries = 0;
            HttpResponseMessage response = null;

            while (retries < maxRetries)
            {
                try
                {
                    response = await httpRequestFunc();
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                }
                catch (HttpRequestException)
                {
                    if (retries == maxRetries - 1)
                    {
                        throw;
                    }
                }

                retries++;
                await Task.Delay(TimeSpan.FromSeconds(2)); // Exponential backoff can be applied here
            }

            return response;
        }

        //public async Task<Unit> Handle(AddInstagramPostCommand request, CancellationToken cancellationToken)
        //{
        //    string currentClassName = nameof(AddInstagramPostCommandHandler);

        //    string imageUrl = "https://thewellnessjunctionbucket.s3.eu-north-1.amazonaws.com/6f013177-c7e3-45ae-88c4-f848a5b6de58.jpg";
        //    string caption = "First Post";
        //    string pageId = "374426615745709";

        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient();

        //        string businessAccountRequestUrl = $"https://graph.facebook.com/v20.0/{pageId}?fields=instagram_business_account&access_token={_accessToken}";

        //        var businessAccountResponse = await SendHttpRequestWithRetry(async () =>
        //        {
        //            return await client.GetAsync(businessAccountRequestUrl, cancellationToken);
        //        });

        //        var businessAccountContent = await businessAccountResponse.Content.ReadAsStringAsync();

        //        if (!businessAccountResponse.IsSuccessStatusCode)
        //        {
        //            var errorMessage = $"Error retrieving Instagram Business Account ID: {businessAccountContent}";
        //            await _globalHelper.Log(new Exception(errorMessage), currentClassName);
        //            throw new ApplicationException(errorMessage);
        //        }

        //        var businessAccountId = JObject.Parse(businessAccountContent)["instagram_business_account"]["id"].ToString();

        //        var createMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media";
        //        var createMediaContent = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("image_url", imageUrl),
        //            new KeyValuePair<string, string>("caption", caption),
        //            new KeyValuePair<string, string>("access_token", _accessToken)
        //        });

        //        var createMediaResponse = await SendHttpRequestWithRetry(async () =>
        //        {
        //            return await client.PostAsync(createMediaUrl, createMediaContent, cancellationToken);
        //        });

        //        var createMediaResponseContent = await createMediaResponse.Content.ReadAsStringAsync();

        //        if (!createMediaResponse.IsSuccessStatusCode)
        //        {
        //            var errorMessage = $"Error creating media object: {createMediaResponseContent}";
        //            await _globalHelper.Log(new Exception(errorMessage), currentClassName);
        //            throw new ApplicationException(errorMessage);
        //        }

        //        var mediaObjectId = JObject.Parse(createMediaResponseContent)["id"].ToString();

        //        var publishMediaUrl = $"https://graph.facebook.com/v14.0/{businessAccountId}/media_publish";
        //        var publishMediaContent = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("creation_id", mediaObjectId),
        //            new KeyValuePair<string, string>("access_token", _accessToken)
        //        });

        //        var publishMediaResponse = await SendHttpRequestWithRetry(async () =>
        //        {
        //            return await client.PostAsync(publishMediaUrl, publishMediaContent, cancellationToken);
        //        });

        //        var publishMediaResponseContent = await publishMediaResponse.Content.ReadAsStringAsync();

        //        if (!publishMediaResponse.IsSuccessStatusCode)
        //        {
        //            var errorMessage = $"Error publishing media object: {publishMediaResponseContent}";
        //            await _globalHelper.Log(new Exception(errorMessage), currentClassName);
        //            throw new ApplicationException(errorMessage);
        //        }

        //        return Unit.Value;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        await _globalHelper.Log(ex, currentClassName);
        //        throw new ApplicationException("Error posting to Instagram: " + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _globalHelper.Log(ex, currentClassName);
        //        throw new ApplicationException("An unexpected error occurred: " + ex.Message);
        //    }
        //}
    }
}
