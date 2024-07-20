using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces.Amazon;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces.Video;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading;
using MediatR;
using System.Drawing;
using MediaToolkit.Options;
using MediaToolkit;
using MediaToolkit.Model;
using OpenCvSharp;
using Xabe.FFmpeg;
using System.Drawing.Text;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.Video
{
    public class VideoService : IVideoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOpenAiService _openAiService;
        private readonly IAmazonS3Service _amazonS3Service;

        private readonly string _accessToken;

        public VideoService(
            ITWJAppDbContext context,
            IGlobalHelperService globalHelper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IOpenAiService openAiService,
            IAmazonS3Service amazonS3Service)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _accessToken = _configuration["Instagram:LongLiveAccessToken"];
            _openAiService = openAiService;
            _amazonS3Service = amazonS3Service;
        }

        public async Task<Unit> GenerateVideo(int requestType, CancellationToken cancellationToken)
        {
            try
            {
                var instagramTemplate = await GetInstagramTemplate(requestType, cancellationToken);
                var quote = await GetRandomQuote(cancellationToken);

                string videoUrl = instagramTemplate.Image;
                string outputFilePath = $"{Guid.NewGuid()}-video-template-{requestType}.mp4";
                string tempVideoPath = $"{Guid.NewGuid()}-temp-video.mp4";
                string tempAudioPath = $"{Guid.NewGuid()}-temp-audio.wav";
                string tempVideoWithAudioPath = $"{Guid.NewGuid()}-template-{requestType}.mp4";

                await DownloadVideoTemplate(videoUrl, tempVideoPath);
                var (width, height, fps, frameCount) = GetVideoMetadata(tempVideoPath);

                var tempFramePaths = await ProcessFrames(tempVideoPath, quote, width, height, fps, frameCount, requestType);

                CombineFramesIntoVideo(tempFramePaths, outputFilePath, width, height, fps);
                ExtractAudioFromVideo(tempVideoPath, tempAudioPath);
                await MergeAudioWithVideo(outputFilePath, tempAudioPath, tempVideoWithAudioPath);

                CleanUpTemporaryFiles(tempVideoPath, tempAudioPath, outputFilePath);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, "VideoService");
                return Unit.Value;
            }
        }

        private async Task<InstagramPost> GetInstagramTemplate(int requestType, CancellationToken cancellationToken)
        {
            var instagramTemplate = await _context.InstagramPosts
                .Where(x => x.Type == requestType && x.IsVideo == true)
                .FirstOrDefaultAsync(cancellationToken);

            if (instagramTemplate == null)
            {
                await _globalHelper.Log(new Exception("No suitable video template found."), "VideoService");
            }

            return instagramTemplate;
        }

        private async Task<Domain.Entities.Quote> GetRandomQuote(CancellationToken cancellationToken)
        {
            var quotes = await _context.Quotes
                .Where(q => q.Category == "Motivational" && q.Author == null && q.Length < 460)
                .ToListAsync(cancellationToken);

            if (!quotes.Any())
            {
                await _globalHelper.Log(new Exception("No suitable quote found.."), "VideoService");
            }

            var random = new Random();
            return quotes[random.Next(quotes.Count)];
        }

        private async Task DownloadVideoTemplate(string videoUrl, string tempVideoPath)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(videoUrl);
                response.EnsureSuccessStatusCode();

                await using (var fs = new FileStream(tempVideoPath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
        }

        private (int width, int height, double fps, int frameCount) GetVideoMetadata(string tempVideoPath)
        {
            var inputFile = new MediaFile { Filename = tempVideoPath };
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                var frameSizeParts = inputFile.Metadata.VideoData.FrameSize.Split('x');
                int width = int.Parse(frameSizeParts[0]);
                int height = int.Parse(frameSizeParts[1]);
                double fps = inputFile.Metadata.VideoData.Fps;
                double durationInSeconds = inputFile.Metadata.Duration.TotalSeconds;
                int frameCount = (int)(durationInSeconds * fps);

                return (width, height, fps, frameCount);
            }
        }

        private async Task<string[]> ProcessFrames(string tempVideoPath, Domain.Entities.Quote quote, int width, int height, double fps, int frameCount, int requestType)
        {
            var tempFramePaths = new string[frameCount];

            PrivateFontCollection privateFonts = new PrivateFontCollection();
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string apiDirectory = Path.Combine(Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName);
            string customFontPath = "";
            FontFamily fontFamily = new FontFamily(GenericFontFamilies.SansSerif);

            RectangleF quoteRect;
            float fontSize;

            switch (requestType)
            {
                case 1:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "Rinnet Regular.otf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
                case 2:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "Youngserif Regular.ttf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
                case 3:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "Kollektif.ttf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
                case 4:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "Bahuraksa.otf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
                case 5:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "Paragon Regular.otf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
                case 6:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "Suave.ttf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
                case 7:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "MartianMono.ttf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
                default:
                    customFontPath = Path.Combine(apiDirectory, "Fonts", "Rinnet Regular.otf");
                    customFontPath = Path.GetFullPath(customFontPath);
                    privateFonts.AddFontFile(customFontPath);
                    fontFamily = privateFonts.Families[0];

                    quoteRect = new RectangleF(103, 608, 856, 521);
                    fontSize = _globalHelper.CalculateFontSize(quote.Content, quoteRect);
                    break;
            }

            var inputFile = new MediaFile { Filename = tempVideoPath };
            using (var engine = new Engine())
            {
                for (int i = 0; i < frameCount; i++)
                {
                    var frameFileName = $"{Guid.NewGuid()}-frame-{i}.jpg";
                    tempFramePaths[i] = frameFileName;
                    var frameFile = new MediaFile { Filename = frameFileName };
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i * (1.0 / fps)) };
                    engine.GetThumbnail(inputFile, frameFile, options);

                    using (var frameImage = new Bitmap(frameFile.Filename))
                    {
                        using (var graphics = Graphics.FromImage(frameImage))
                        {
                            var font = new Font(fontFamily, fontSize, FontStyle.Bold);
                            var brush = new SolidBrush(Color.White);

                            var stringFormat = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };

                            graphics.DrawString(quote.Content, font, brush, quoteRect, stringFormat);
                        }

                        var modifiedFrameFileName = $"{Guid.NewGuid()}-modified-frame-{i}.jpg";
                        frameImage.Save(modifiedFrameFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        tempFramePaths[i] = modifiedFrameFileName;
                    }

                    File.Delete(frameFileName);
                }
            }

            return tempFramePaths;
        }

        private void CombineFramesIntoVideo(string[] tempFramePaths, string outputFilePath, int width, int height, double fps)
        {
            using (var videoWriter = new VideoWriter(outputFilePath, FourCC.H264, fps, new OpenCvSharp.Size(width, height)))
            {
                foreach (var framePath in tempFramePaths)
                {
                    using (var frame = new Mat(framePath))
                    {
                        videoWriter.Write(frame);
                    }

                    File.Delete(framePath);
                }
            }
        }

        private void ExtractAudioFromVideo(string tempVideoPath, string tempAudioPath)
        {
            var inputFile = new MediaFile { Filename = tempVideoPath };
            var audioFile = new MediaFile { Filename = tempAudioPath };
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, audioFile, new ConversionOptions { AudioSampleRate = AudioSampleRate.Hz44100 });
            }
        }

        private async Task MergeAudioWithVideo(string outputFilePath, string tempAudioPath, string tempVideoWithAudioPath)
        {
            FFmpeg.SetExecutablesPath(@"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Api\ffmpeg\bin");
            await FFmpeg.Conversions.New()
                .AddParameter($"-i {outputFilePath}")
                .AddParameter($"-i {tempAudioPath}")
                .SetOutput(tempVideoWithAudioPath)
                .Start();
        }

        private void CleanUpTemporaryFiles(string tempVideoPath, string tempAudioPath, string outputFilePath)
        {
            File.Delete(tempVideoPath);
            File.Delete(tempAudioPath);
            File.Delete(outputFilePath);
        }
    }
}