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
using System.Text.RegularExpressions;
using SystemImageFormat = System.Drawing.Imaging.ImageFormat;

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
        private readonly string _pexelsApiKey;
        private readonly string _googleSearchApiKey;
        private readonly string _envatoApiKey;
        private readonly string _googleSearchEngineID;

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
            _pexelsApiKey = _configuration["Pexels:Key"];
            _envatoApiKey = _configuration["Envato:Key"];
            _googleSearchApiKey = _configuration["Google:Search:Key"];
            _googleSearchEngineID = _configuration["Google:Search:EngineID"];
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
                    Domain.Entities.News news = null;

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
                                        case 18:
                                            news = await _context.News
                                                            .OrderBy(n => Guid.NewGuid())
                                                            .FirstOrDefaultAsync(cancellationToken);

                                            if (news == null)
                                            {
                                                throw new ApplicationException("No news title found.");
                                            }


                                            var newsTitle = news.Title;
                                            var prompt = $"Generate some words(keywords) for this title: '{newsTitle}'. The format must be <keyword>keyword</keyword><keyword>keyword</keyword><keyword>keyword</keyword>";
                                            var keywordResponse = await _openAiService.GenerateSectionAsync(prompt, cancellationToken);

                                            if (string.IsNullOrEmpty(keywordResponse))
                                            {
                                                throw new ApplicationException("Failed to generate keywords.");
                                            }

                                            var keywords = ParseKeywords(keywordResponse);

                                            if (keywords == null || !keywords.Any())
                                            {
                                                throw new ApplicationException("No suitable keywords found.");
                                            }

                                            string pexelsImageUrl = null;
                                            var pexelsApiKey = _pexelsApiKey;

                                            using (HttpClient pexelsClient = new HttpClient())
                                            {
                                                pexelsClient.DefaultRequestHeaders.Add("Authorization", pexelsApiKey);
                                                foreach (var keyword in keywords)
                                                {
                                                    var searchUrlHorizontal = $"https://api.pexels.com/v1/search?query={keyword}&per_page=10&orientation=horizontal";
                                                    var pexelsResponseHorizontal = await pexelsClient.GetAsync(searchUrlHorizontal);

                                                    if (pexelsResponseHorizontal.IsSuccessStatusCode)
                                                    {
                                                        var jsonResponse = await pexelsResponseHorizontal.Content.ReadAsStringAsync();
                                                        var searchResults = JObject.Parse(jsonResponse);
                                                        var photos = searchResults["photos"];
                                                        if (photos != null && photos.Any())
                                                        {
                                                            var randomIndex = new Random().Next(photos.Count());
                                                            pexelsImageUrl = photos[randomIndex]["src"]["original"].ToString();
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var errorContent = await pexelsResponseHorizontal.Content.ReadAsStringAsync();
                                                        Console.WriteLine($"Pexels API Error (Horizontal): {errorContent}");
                                                    }

                                                    if (string.IsNullOrEmpty(pexelsImageUrl))
                                                    {
                                                        var searchUrlSquare = $"https://api.pexels.com/v1/search?query={keyword}&per_page=10&orientation=square";
                                                        var pexelsResponseSquare = await pexelsClient.GetAsync(searchUrlSquare);

                                                        if (pexelsResponseSquare.IsSuccessStatusCode)
                                                        {
                                                            var jsonResponse = await pexelsResponseSquare.Content.ReadAsStringAsync();
                                                            var searchResults = JObject.Parse(jsonResponse);
                                                            var photos = searchResults["photos"];
                                                            if (photos != null && photos.Any())
                                                            {
                                                                var randomIndex = new Random().Next(photos.Count());
                                                                pexelsImageUrl = photos[randomIndex]["src"]["original"].ToString();
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {

                                                            var errorContent = await pexelsResponseSquare.Content.ReadAsStringAsync();
                                                            Console.WriteLine($"Pexels API Error (Square): {errorContent}");
                                                        }
                                                    }
                                                }
                                            }

                                            if (string.IsNullOrEmpty(pexelsImageUrl))
                                            {
                                                throw new ApplicationException("No image found from Pexels for any of the generated keywords.");
                                            }

                                            using (HttpClient pexelsImageClient = new HttpClient())
                                            {
                                                var pexelsImageResponse = await pexelsImageClient.GetAsync(pexelsImageUrl);
                                                pexelsImageResponse.EnsureSuccessStatusCode();

                                                using (var pexelsImageStream = await pexelsImageResponse.Content.ReadAsStreamAsync())
                                                {
                                                    using (var pexelsOriginalImage = Image.FromStream(pexelsImageStream))
                                                    {
                                                        int targetSize = 1080;
                                                        int width = pexelsOriginalImage.Width;
                                                        int height = pexelsOriginalImage.Height;
                                                        int newWidth, newHeight;

                                                        if (width > height)
                                                        {
                                                            newWidth = targetSize * width / height;
                                                            newHeight = targetSize;
                                                        }
                                                        else
                                                        {
                                                            newWidth = targetSize;
                                                            newHeight = targetSize * height / width;
                                                        }

                                                        using (var resizedImage = new Bitmap(pexelsOriginalImage, new Size(newWidth, newHeight)))
                                                        {

                                                            var cropRect = new Rectangle((newWidth - targetSize) / 2, (newHeight - targetSize) / 2, targetSize, targetSize);
                                                            using (var croppedImage = resizedImage.Clone(cropRect, pexelsOriginalImage.PixelFormat))
                                                            {

                                                                using (var pexelsImage = new Bitmap(croppedImage.Width, croppedImage.Height, PixelFormat.Format24bppRgb))
                                                                {
                                                                    using (var pexelsGraphics = Graphics.FromImage(pexelsImage))
                                                                    {
                                                                        pexelsGraphics.DrawImage(croppedImage, 0, 0);

                                                                        var pexelsOverlayBrush = new SolidBrush(Color.FromArgb(230, 140, 177, 109));

                                                                        pexelsGraphics.FillRectangle(pexelsOverlayBrush, new Rectangle(0, 0, pexelsImage.Width, pexelsImage.Height));


                                                                        customFontPath = Path.Combine(apiDirectory, "Fonts", "LeagueGothic Regular.otf");
                                                                        customFontPath = Path.GetFullPath(customFontPath);
                                                                        privateFonts.AddFontFile(customFontPath);

                                                                        var pexelsFontFamily = privateFonts.Families[0];
                                                                        var pexelsFontSize = _globalHelper.CalculateFontSize(newsTitle, new RectangleF(10, 10, pexelsImage.Width - 20, pexelsImage.Height - 20));
                                                                        var pexelsFont = new Font(pexelsFontFamily, pexelsFontSize, FontStyle.Bold);
                                                                        var pexelsBrush = new SolidBrush(Color.White);
                                                                        var pexelsTitleRect = new RectangleF(10, 10, pexelsImage.Width - 20, pexelsImage.Height - 20);
                                                                        var pexelsStringFormat = new StringFormat
                                                                        {
                                                                            Alignment = StringAlignment.Center,
                                                                            LineAlignment = StringAlignment.Center
                                                                        };

                                                                        pexelsGraphics.DrawString(newsTitle, pexelsFont, pexelsBrush, pexelsTitleRect, pexelsStringFormat);

                                                                        string pexelsFileName = $"{Guid.NewGuid()}-news-{request.Type}.jpg";
                                                                        string pexelsFilePath = Path.Combine(apiDirectory, pexelsFileName);
                                                                        pexelsImage.Save(pexelsFilePath, SystemImageFormat.Jpeg);

                                                                        var newsS3Path = await _amazonS3Service.UploadFileToS3Async(pexelsFilePath, "Instagram Posts", pexelsFileName);


                                                                        var instagramNewsCaptionPrompt = $"Begin an instagram caption on '{news.Title}'." +
                                                                                    $"Do not write the Title in caption please. Jump right to the facts." +
                                                                                    $"Provide essential insights and facts based on the following details: '{news.Description}'. " +
                                                                                    "Focus on delivering information in a straightforward manner, in a short format, avoiding any dramatic language." +
                                                                                    $"Don't use any Emoji or symbol please, but use 3 hashtags.";

                                                                        var twitterNewsCaptionPrompt = $"Begin tweet on '{news.Title}' with a question. " +
                                                                                    $"Do not write the Title in tweet please." +
                                                                                    $"Provide essential insights and the answer to the question based on the following details: '{news.Description}'. " +
                                                                                    "Focus on delivering information in a straightforward manner, avoiding any dramatic language. " +
                                                                                    $"DO NOT exceed the Twitter tweet characters limit of 250 characters, please make sure to not exceed tweet limit of 250 characters." +
                                                                                    $"Do not use any Emoji or other characters, only plain text.";

                                                                        var instagramNewsCaption = await _openAiService.GenerateSectionAsync(instagramNewsCaptionPrompt, cancellationToken);
                                                                        var twitterNewsCaption = await _openAiService.GenerateSectionAsync(twitterNewsCaptionPrompt, cancellationToken);

                                                                        //await _globalHelper.PostToInstagramAsync(newsS3Path, newsTitle, instagramNewsCaption, cancellationToken);
                                                                        //await _globalHelper.PostToTwitterAsync(newsS3Path, twitterNewsCaption, cancellationToken);

                                                                        news.IsUsedInstagram = true;
                                                                        _context.News.Update(news);
                                                                        await _context.SaveChangesAsync();

                                                                        return Unit.Value;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        case 19:
                                            string graphIdeaPrompt = "Think of an idea for a graph for health related stats which will have category on x-axis and value (number) on y-axis." +
                                                "Please write only the title for this graph, that's all. Nothing else, just the title.";

                                            var graphIdea = await _openAiService.GenerateSectionAsync(graphIdeaPrompt, cancellationToken);

                                            string graphPrompt = $"Generate a graph for a dataset for this title '{graphIdea}' in the following format: \n" +
                                                 "<graph>\n" +
                                                 "<categories>\n" +
                                                 "<element>Category1</element>\n" +
                                                 "<element>Category2</element>\n" +
                                                 "<element>Category3</element>\n" +
                                                 "<element>Category4</element>\n" +
                                                 "<element>Category5</element>\n" +
                                                 "</categories>\n" +
                                                 "<values>\n" +
                                                 "<Category1>Value1</Category1>\n" +
                                                 "<Category2>Value2</Category2>\n" +
                                                 "<Category3>Value3</Category3>\n" +
                                                 "<Category4>Value4</Category4>\n" +
                                                 "<Category5>Value5</Category5>\n" +
                                                 "</values>\n" +
                                                 "</graph>\n" +
                                                 "Please write down only the graph in this format and that's all. Nothing else.";
                                                    



                                            var graphResponse = await _openAiService.GenerateSectionAsync(graphPrompt, cancellationToken);

                                            if (string.IsNullOrEmpty(graphResponse))
                                            {
                                                throw new ApplicationException("Failed to generate graph data.");
                                            }

                                            var graphData = ParseGraphData(graphResponse);

                                            if (graphData == null || !graphData.Any())
                                            {
                                                throw new ApplicationException("No suitable graph data found.");
                                            }

                                            using (var chartImage = GenerateChartImage(graphData))
                                            {
                                                string chartFileName = $"{Guid.NewGuid()}-chart-{request.Type}.jpg";
                                                string chartFilePath = Path.Combine(apiDirectory, chartFileName);
                                                chartImage.Save(chartFilePath, SystemImageFormat.Jpeg);

                                                var s3ChartPath = await _amazonS3Service.UploadFileToS3Async(chartFilePath, "Instagram Posts", chartFileName);

                                                //var instagramChartCaptionPrompt = "Generate an Instagram caption for a graph showing the prevalence of common health conditions by age group. Focus on delivering the information in a straightforward manner and use 3 hashtags. No emojis or symbols.";
                                                //var instagramChartCaption = await _openAiService.GenerateSectionAsync(instagramChartCaptionPrompt, cancellationToken);

                                                //var twitterChartCaptionPrompt = "Generate a tweet for a graph showing the prevalence of common health conditions by age group. The tweet should provide essential insights and stay within the 250-character limit. No emojis or symbols.";
                                                //var twitterChartCaption = await _openAiService.GenerateSectionAsync(twitterChartCaptionPrompt, cancellationToken);

                                                //await _globalHelper.PostToInstagramAsync(s3ChartPath, "Health Conditions Chart", instagramChartCaption, cancellationToken);
                                                //await _globalHelper.PostToTwitterAsync(s3ChartPath, twitterChartCaption, cancellationToken);
                                            }

                                            return Unit.Value;
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
                                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                                //string filePath = fileName;

                                image.Save(filePath, SystemImageFormat.Jpeg);

                                var s3Path = await _amazonS3Service.UploadFileToS3Async(filePath, "Instagram Posts", fileName);


                                //var instagramCaptionPrompt = $"Begin an instagram caption on this quote '{quote.Content}'." +
                                //        $"Do not write the quote itself in caption please. Jump right to the facts." +
                                //        $"Provide essential insights and facts. " +
                                //        "Focus on delivering information in a straightforward manner, in a short format, avoiding any dramatic language." +
                                //        $"Don't use any Emoji or symbol please, but use 3 hashtags.";

                                //var instagramCaption = await _openAiService.GenerateSectionAsync(instagramCaptionPrompt, cancellationToken);
                                
                                //var twitterCaptionPrompt = $"Begin tweet on this quote '{quote.Content}'. " +
                                //    $"Do not write the quote itself in tweet please." +
                                //    $"Provide essential insights only. " +
                                //    "Focus on delivering information in a straightforward manner, avoiding any dramatic language. " +
                                //    $"DO NOT exceed the Twitter tweet characters limit of 250 characters, please make sure to not exceed tweet limit of 250 characters." +
                                //    $"Do not use any Emoji or other characters, only plain text.";

                                //var twitterCaption = await _openAiService.GenerateSectionAsync(twitterCaptionPrompt, cancellationToken);

                                //await _globalHelper.PostToInstagramAsync(s3Path, quote.Content, instagramCaption, cancellationToken);
                                //await _globalHelper.PostToTwitterAsync(s3Path, twitterCaption, cancellationToken);
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

        private List<string> ParseKeywords(string response)
        {
            var keywords = new List<string>();
            var matches = Regex.Matches(response, @"<keyword>(.*?)</keyword>");
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    keywords.Add(match.Groups[1].Value);
                }
            }
            return keywords;
        }
        private Dictionary<string, double> ParseGraphData(string graphResponse)
        {
            var cleanedGraphResponse = Regex.Replace(graphResponse, @"\s+", " ").Trim();

            var graphData = new Dictionary<string, double>();

            var categoriesMatch = Regex.Match(cleanedGraphResponse, @"<categories>(.*?)<\/categories>");
            var categories = new List<string>();
            if (categoriesMatch.Success)
            {
                var categoryMatches = Regex.Matches(categoriesMatch.Groups[1].Value, @"<element>(.*?)<\/element>");
                foreach (Match match in categoryMatches)
                {
                    categories.Add(match.Groups[1].Value);
                }
            }

            var valuesMatch = Regex.Match(cleanedGraphResponse, @"<values>(.*?)<\/values>");
            if (valuesMatch.Success)
            {
                foreach (var category in categories)
                {
                    var valueMatch = Regex.Match(valuesMatch.Groups[1].Value, $@"<{category}>(.*?)<\/{category}>");
                    if (valueMatch.Success && double.TryParse(valueMatch.Groups[1].Value, out double value))
                    {
                        graphData[category] = value;
                    }
                }
            }

            return graphData;
        }





        private Bitmap GenerateChartImage(Dictionary<string, double> graphData)
        {
            int width = 720;  // 10% smaller
            int height = 540;  // 10% smaller
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);
            var font = new Font("Arial", 8);
            var brush = new SolidBrush(Color.Black);

            int margin = 60;
            int graphWidth = width - 2 * margin;
            int graphHeight = height - 2 * margin;

            var categories = graphData.Keys.ToList();
            int barWidth = graphWidth / categories.Count;

            // Calculate the maximum value in the data to scale the graph appropriately
            double maxValue = graphData.Values.Max();

            int x = margin;
            foreach (var category in categories)
            {
                int barHeight = (int)(graphData[category] / maxValue * graphHeight);
                graphics.FillRectangle(Brushes.Blue, x, height - margin - barHeight, barWidth, barHeight);

                // Draw x-axis labels diagonally
                graphics.TranslateTransform(x + barWidth / 2, height - margin + 5);
                graphics.RotateTransform(45);
                graphics.DrawString(category, font, brush, 0, 0);
                graphics.ResetTransform();

                x += barWidth + 5;
            }

            // Draw y-axis labels horizontally
            int yInterval = (int)Math.Ceiling(maxValue / 10); // Adjust interval as necessary
            for (int i = 0; i <= maxValue; i += yInterval)
            {
                int yPosition = height - margin - (int)(i / maxValue * graphHeight);
                graphics.DrawString(i.ToString(), font, brush, margin - 30, yPosition - 10);
            }

            // Draw axes
            graphics.DrawLine(Pens.Black, margin, height - margin, width - margin, height - margin);  // x-axis
            graphics.DrawLine(Pens.Black, margin, height - margin, margin, margin);  // y-axis

            return bitmap;
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
