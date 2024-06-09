using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using nQuant;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Markdig;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces.Amazon;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.OpenAi;
using TWJ.TWJApp.TWJService.Domain.Enums;


namespace TWJ.TWJApp.TWJService.Application.Services.OpenAI
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _gptModel;
        private readonly string _affiliate;
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";
        private readonly IAmazonS3Service _amazonS3Service;
        public OpenAiService(HttpClient httpClient, IConfiguration configuration, ITWJAppDbContext context, IGlobalHelperService globalHelper, IPreplexityService preplexityService, IAmazonS3Service amazonS3Service)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.openai.com/");
            _apiKey = configuration["ChatGPT:Key2"];
            _gptModel = configuration["ChatGPT:Model"];
            _affiliate = configuration["ClickBank:Affiliate:Username"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _context = context;
            _globalHelper = globalHelper;
            currentClassName = GetType().Name;
            _amazonS3Service = amazonS3Service;
        }
        #region Public Services

        public async Task<BlogPostResponse> GenerateBlogPostAsync(BlogPostType postType, CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            BlogPostSettings settings = await GetSettingsForPostType(postType);

            try
            {
                string title = await GenerateSectionAsync(settings.TitlePrompt, cancellationToken);
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]"); 
                
                var categoryList = await GetStringedCategoriesAsync(cancellationToken);
                var category = await GenerateSectionAsync($"Pick one category that is related to this title '{title}' " +
                    $"Please don't write the number of category, don't write anything else except the category name. " +
                    $"Pick only one name, that's all. Here is the category list: {categoryList} ");

                var productToPromote = await GetProductToPromoteAsync(category, cancellationToken);
                if (productToPromote == null) 
                {
                    return new BlogPostResponse();
                }

                string introduction = await GenerateSectionAsync(settings.IntroductionPrompt.Replace("%%TITLE%%", title), cancellationToken);
                string introductionHtmlContent = Markdown.ToHtml(introduction);
                blogPostContent.AppendLine("<div class='sQxpAj1VQF'>");
                blogPostContent.AppendLine($"{introductionHtmlContent}");
                blogPostContent.AppendLine("</div>");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(settings.ContentPrompt.Replace("%%TITLE%%", title), cancellationToken);
                string mainContentHtmlContent = Markdown.ToHtml(mainContent);
                blogPostContent.AppendLine("<div class='nm4JsSMWGj'>");
                blogPostContent.AppendLine($"{mainContentHtmlContent}");
                blogPostContent.AppendLine("</div>");
                blogPostContent.AppendLine($"[advertisement]");

                var productList = await GetRandomProductsAsync(settings.NumberOfAdvertisements + 1, cancellationToken);

                string callToAction = await GenerateSectionAsync(settings.CallToActionPrompt
                    .Replace("%%PRODUCT_NAME%%", productToPromote.ProductName)
                    .Replace("%%PRODUCT_DESCRIPTION%%", productToPromote.Description)
                    .Replace("%%TITLE%%", title), cancellationToken);
                string callToActionHtmlContent = Markdown.ToHtml(callToAction);
                blogPostContent.AppendLine("<div class='pJDcMLp9xo'>");
                blogPostContent.AppendLine($"{callToActionHtmlContent}");
                blogPostContent.AppendLine("</div>");
                blogPostContent.AppendLine($"[advertisement]");

                string concepts = await GenerateSectionAsync(settings.ImageConceptsPrompt, cancellationToken);

                string backLinkKeywordList = await GenerateSectionAsync(settings.BackLinkKeywordsPrompt.Replace("%%TITLE%%", title), cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);

                string finalHtmlContent = await FormatBlogPostToHtmlAndInsertProductLink(blogPostContent.ToString(), productList, productToPromote);


                return new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = finalHtmlContent,
                    BlogPostCategoryId = settings.CategoryId,
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productToPromote.Id,
                    Image = await GenerateImageAsync(settings.ImagePrompt.Replace("%%IMAGE_CONCEPTS%%", concepts))
                    //Image = "https://thewellnessjunctionbucket.s3.eu-north-1.amazonaws.com/6f013177-c7e3-45ae-88c4-f848a5b6de58.jpg"
                };
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }

        private async Task<BlogPostSettings> GetSettingsForPostType(BlogPostType postType)
        {
            BlogPostSettings settings = new BlogPostSettings();
            switch (postType)
            {
                case BlogPostType.Myth:
                        settings.TitlePrompt = $"Create a captivating blog post title that reveals unexpected insights into everyday health and" +
                            $" lifestyle topics. The title should uncover a common myth, provide a solution to a frequent concern, or share" +
                            $" expert advice in a surprising and thought-provoking way. The title should combine a sense of urgency with " +
                            $"practical advice, and be structured in a way that instantly grabs attention by highlighting little-known facts " +
                            $"or suggesting transformative benefits.";
                        settings.IntroductionPrompt = $"Write a clear and attention-grabbing introduction for a blog post about this topic: '%%TITLE%%'.";
                        settings.ContentPrompt = $"Provide detailed and professional information on this topic: '%%TITLE%%'.";
                        settings.CategoryId = Guid.Parse("79320ffc-726e-4392-9b3a-ee6c2b64bd3f");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.WeightLoss:
                        settings.TitlePrompt = $"Generate a dynamic blog post title related to weight loss that captures the reader's attention. " +
                                "The title could address emerging diet trends, debunk myths about weight loss, offer insights into effective workout routines, " +
                                "or explore the psychological aspects of losing weight. Aim for variety, ensuring that each title sparks curiosity " +
                                "and offers a unique take on the weight loss journey.";
                        settings.IntroductionPrompt = $"Craft a compelling introduction for the blog post titled '%%TITLE%%'. " +
                                 $"The introduction should set the stage for an insightful exploration " +
                                 $"of the topic, encouraging readers to continue for valuable tips, debunked myths, or novel weight loss strategies.";
                        settings.ContentPrompt = $"Delve into the details of '%%TITLE%%', providing readers with actionable advice, scientific findings, " +
                                "or motivational stories. Ensure the article is as informative as it is engaging.";
                        settings.CategoryId = Guid.Parse("79320ffc-726e-4392-9b3a-ee6c2b64bd3f");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.Brightside:
                    settings.TitlePrompt = $"Create a captivating blog post title that offers a straightforward dietary solution to a " +
                            $"common health concern. The title should hint at an unexpected yet easy change in the reader's morning routine" +
                            $" that could have a significant positive impact on their health. Incorporate numbers to suggest specific advice" +
                            $" or benefits, and make sure the title arouses curiosity while promising practical value.";
                    settings.IntroductionPrompt = $"Write a compelling introduction for a blog post focusing on '%%TITLE%%'. The introduction " +
                            $"should empathize with the reader's frustrations or " +
                            "discomforts regarding this issue. Then, hint at a simple yet unexpected dietary change available in their" +
                            " morning routine that could offer significant benefits. The tone should be friendly and reassuring, promising " +
                            "that the solution will be easy to incorporate into their daily life and backed by nutritional insights. The goal " +
                            "is to make the reader feel understood and intrigued about the solution, encouraging them to continue reading for " +
                            "practical advice.";
                    settings.ContentPrompt = $"Craft engaging and detailed content for the blog post with a focus on this topic: '%%TITLE%%'. " +
                            "Begin by presenting the problem or question in a way that resonates with the reader's experiences. " +
                            "Then, introduce the solution or key insights, breaking down the information into easily digestible segments. " +
                            "Throughout, maintain a conversational and positive tone, aiming to educate and motivate the reader. " +
                            "Ensure all formatting tags are used correctly to enhance readability and engagement.";
                    settings.CategoryId = Guid.Parse("7f002904-1bb0-4bbb-bd2f-ecb88dc3122e");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.NutritionalAdvice:
                    settings.TitlePrompt = $"Create a compelling blog post title centered around diet and nutritional advice for weight loss. " +
                            "The title should debunk common dietary myths, highlight the importance of balancing macronutrients, " +
                            "or offer insights into reading food labels. Aim for a title that combines intrigue with practical guidance, " +
                            "and motivates readers to discover nutritional truths that can transform their approach to weight loss.";
                    settings.IntroductionPrompt = $"Craft a clear and engaging introduction for a blog post on the topic: '%%TITLE%%'. " +
                            $"Set the stage for an informative deep dive into nutritional advice.";
                    settings.ContentPrompt = $"Deliver comprehensive and expertly curated information on the topic: %%TITLE%%.";
                    settings.CategoryId = Guid.Parse("fe74be57-a371-4a55-99a9-a8493f5b5f82");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.ExerciseGuides:
                    settings.TitlePrompt = $"Generate a dynamic and motivating blog post title about exercise guides. " +
                            "The title should appeal to both beginners and advanced fitness enthusiasts, covering aspects like home workouts, " +
                            "gym routines, and outdoor exercises. Ensure the title encapsulates the spirit of inclusivity and the benefits of regular exercise.";
                    settings.IntroductionPrompt = $"Compose a compelling introduction for a blog post titled '%%TITLE%%'. " +
                            "Emphasize the importance of physical activity for all fitness levels and environments.";
                    settings.ContentPrompt = $"Create detailed and informative content for the blog post: '%%TITLE%%'. Include workout plans, " +
                            "exercise routines, and tips suitable for a wide range of individuals, from beginners to advanced.";
                    settings.CategoryId = Guid.Parse("a213dda9-e0a8-4869-9d9b-c3e74d2e39a5");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.RecipesAndMealPlans:
                    settings.TitlePrompt = $"Craft an enticing blog post title that showcases healthy, low-calorie recipes and meal plans. " +
                           "The title should communicate the ease of meal prep and the benefits of following these meal plans for weight loss and healthy living. " +
                           "Aim for a title that is both inviting and informative, encouraging readers to explore simple yet nutritious meal options.";
                    settings.IntroductionPrompt = $"Compose an engaging introduction for a blog post with the title '%%TITLE%%', focusing on healthy eating habits. " +
                            "Highlight the significance of low-calorie recipes and well-planned meals in achieving weight loss goals." +
                            " Structure the introduction to captivate the reader's interest in discovering delicious and nutritious meal options.";
                    settings.ContentPrompt = $"Develop in-depth and appetizing content for the blog post: '%%TITLE%%'. Feature healthy, low-calorie recipes " +
                            "and offer practical weekly meal plans that aid in weight loss. Structure the content with [sub]subtitles[/sub] for each recipe and meal plan.";
                    settings.CategoryId = Guid.Parse("fe74be57-a371-4a55-99a9-a8493f5b5f82");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.Numbered:
                    settings.TitlePrompt = $"Generate a compelling blog post title in the category of weight loss that includes a number. " +
                            "The title should promise actionable advice, tips, or insights for readers interested in weight loss, " +
                            "such as revealing foods that aid in weight loss, simple exercise routines, or myths debunked. " +
                            "The title must be engaging, informative, and include a list format, e.g., '7 Foods That...', '5 Ways To...', etc.";
                    settings.IntroductionPrompt = $"Compose an engaging introduction for a blog post with the title '%%TITLE%%'. ";
                    settings.ContentPrompt = $"Develop in-depth content for the blog post: '%%TITLE%%'.";
                    settings.CategoryId = Guid.Parse("79320ffc-726e-4392-9b3a-ee6c2b64bd3f");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.LatestNews:
                    var randomNews = await _context.News
                        .FromSqlRaw("SELECT * FROM \"News\" WHERE \"Active\" = TRUE ORDER BY RANDOM() LIMIT 1")
                        .FirstOrDefaultAsync();

                    settings.TitlePrompt = $"Generate a captivating blog post title about this topic \"{randomNews.Title}\".";
                    settings.IntroductionPrompt = $"Begin the blog post on '%%TITLE%%' with a clear and direct introduction. " +
                            $"Provide essential insights based on the following details: '{randomNews.Description}'. " +
                            "Focus on delivering information in a straightforward manner, avoiding any dramatic language. " +
                            $"Engage the reader with facts and thoughtful analysis.";
                    settings.ContentPrompt = $"Craft professional and detailed content for the blog post titled '%%TITLE%%'." +
                            "Maintain a conversational and uplifting tone throughout, and write as much as you can, inform the reader as much as you can.";
                    settings.CategoryId = Guid.Parse("3ee72486-7a14-474f-a4ac-a45f93eb49e6");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.MindsetAndMotivation:
                    settings.TitlePrompt = "Generate a captivating blog post title about the psychological aspects of losing weight, " +
                                      "such as maintaining motivation, dealing with setbacks, and building healthy habits. The title should be " +
                                      "inspiring and include a number to suggest practical tips or insights.";
                    settings.IntroductionPrompt = "Write an engaging introduction for a blog post focusing on 'Mindset and Motivation' " +
                                             "for weight loss based on '%%TITLE%%'. Start by empathizing with the reader's struggles in " +
                                             "maintaining motivation and dealing with setbacks. Offer effective strategies for building " +
                                             "healthy habits and sustaining a positive mindset throughout their weight loss journey.";
                    settings.ContentPrompt = "Craft engaging and detailed content for the blog post titled '%%TITLE%%', focusing on the psychological " +
                                        "aspects of losing weight. Cover maintaining motivation, " +
                                        "dealing with setbacks, and building healthy habits. Include actionable tips and highlight key insights.";
                    settings.CategoryId = Guid.Parse("ac1641f2-01cd-4d0e-ad1d-5baeceae5c35");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.HealthyLifestyle:
                    settings.TitlePrompt = $"Create a captivating blog post title that offers fresh perspectives on healthy living. " +
                            "Focus on debunking popular health myths, introducing easy-to-adopt healthy habits, or providing innovative " +
                            "tips for maintaining a balanced lifestyle. The title should intrigue readers by challenging common beliefs, " +
                            "highlighting unexpected benefits of simple habits, or revealing expert insights into wellness practices.";
                    settings.IntroductionPrompt = $"Begin the blog post titled '%%TITLE%%' with a clear and direct introduction. " +
                            "Focus on delivering information in a straightforward manner, avoiding any dramatic language. ";
                    settings.ContentPrompt = $"Delve into the subject of '%%TITLE%%' with a focus on practical advice, evidence-based practices, " +
                            "and relatable examples.";
                    settings.CategoryId = Guid.Parse("294f283a-76a1-4c3e-9e3f-9450570af6af");
                    settings.FinalizeSettings();
                    return settings;
                case BlogPostType.PersonalStory:
                    settings.TitlePrompt = $"Create a compelling and personal blog post title about 'health or weight loss'. " +
                                "Think about a journey or personal experience of a person (you must use 'I' for the person)" +
                                " that can inspire others.";
                    settings.IntroductionPrompt = $"Write an engaging and personal introduction for the blog post titled '%%TITLE%%'. " +
                                    $"Discuss the personal connection to the topic, '%%TITLE%%' (do not write the title in the content) " +
                                    "and set the stage for a story that is both informative and deeply personal." +
                                    "You must use 'I' for the person in the blog post." +
                                    "Incorporate [b]bold[/b] and [i]italic[/i] text for emphasis where it naturally fits, " +
                                    "and structure the introduction into concise paragraphs that engage the reader with facts and thoughtful analysis." +
                                   "Ensure all formatting tags are correctly opened and closed.";
                    settings.ContentPrompt = $"Develop a heartfelt and detailed narrative for the blog post '%%TITLE%%'. " +
                                   "Use anecdotes and personal experiences to illustrate key points about health and wellness. " +
                                   "Structure your content with emotional depth, using natural dialogues, reflections, " +
                                   "and lessons learned along the way. Include practical tips that you've found personally beneficial." +
                                   "You must use 'I' for the person in the blog post." +
                                   "Organize the content with clear subtitles, each within '[sub]' and '[/sub]' tags, so make sure to put [sub][/sub] tags.Include " +
                                   "bullet points for easy reading, these are the tags for bullet points [b]bold[/b], and use italic tags for text formatting [i]italic[/i]," +
                                   " to emphasize key points. " +
                                   "Ensure all formatting tags are correctly opened and closed.";
                    settings.CategoryId = Guid.Parse("2c9e1da1-3299-451b-99d4-87418bea9c23");
                    settings.FinalizeSettings();
                    return settings;
                default:
                    throw new ArgumentException("Unsupported blog post type");
            }
        }


        #endregion


        #region Private Methods
        private async Task<string> GenerateSectionAsync(string prompt, CancellationToken cancellationToken = default)
        {
            try
            {
                ChatCompletionRequest chatCompletionRequest = new ChatCompletionRequest
                {
                    Model = _gptModel,
                    Messages = new List<ChatMessage>
                {
                    new ChatMessage { Role = "system", Content = prompt }
                },
                    Temperature = 1
                };

                var responseString = await PostOpenAiRequestAsync(chatCompletionRequest, cancellationToken);
                return await ExtractGeneratedText(responseString);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                return $"An error occurred while generating the blog post: {ex.Message}";
            }

        }

        private async Task<string> GenerateImageAsync(string prompt, int n = 1, string size = "1792x1024")
        {
            try
            {
                var payload = new
                {
                    model = "dall-e-3",
                    prompt = prompt,
                    n = n,
                    size = size
                };

                var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/images/generations", payload);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<DalleResponse>();
                    var imageUrl = (string)responseContent.Data[0].Url;

                    using (var imageHttpClient = new HttpClient())
                    {
                        var imageResponse = await imageHttpClient.GetAsync(imageUrl);
                        if (imageResponse.IsSuccessStatusCode)
                        {
                            var imageData = await imageResponse.Content.ReadAsByteArrayAsync();
                            var tempFilePath = Path.GetTempFileName();
                            await File.WriteAllBytesAsync(tempFilePath, imageData);

                            string quantizedFilePath = await QuantizeImageAsync(tempFilePath);

                            var filePath = await OptimizeAndCompressImage(quantizedFilePath);

                            string subDirectoryInBucket = "";
                            string fileNameInS3 = $"{Guid.NewGuid()}.png";
                            var url = await _amazonS3Service.UploadFileToS3Async(filePath, subDirectoryInBucket, fileNameInS3);


                            File.Delete(tempFilePath);
                            File.Delete(quantizedFilePath);

                            return url;
                        }
                        else
                        {
                            var errorContent = await imageResponse.Content.ReadAsStringAsync();
                            throw new ApplicationException($"Error fetching image data: {errorContent}");
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new ApplicationException($"Error generating image: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }

        private async Task<string> QuantizeImageAsync(string imagePath)
        {
            var quantizer = new WuQuantizer();
            using (var originalImage = new Bitmap(imagePath))
            {
                var argbImage = new Bitmap(originalImage.Width, originalImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (var graphics = Graphics.FromImage(argbImage))
                {
                    graphics.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, argbImage.Width, argbImage.Height));
                }

                using (var quantizedImage = quantizer.QuantizeImage(argbImage))
                {
                    string quantizedFilePath = Path.GetTempFileName();
                    quantizedImage.Save(quantizedFilePath, System.Drawing.Imaging.ImageFormat.Png);
                    return quantizedFilePath;
                }
            }
        }

        private async Task<string> OptimizeAndCompressImage(string imagePath)
        {
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imagePath))
            {
                if (image.Width > 1200 || image.Height > 685)
                {
                    image.Mutate(x => x.Resize(1200, 685));
                }
                var encoder = new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.BestCompression,
                    ColorType = PngColorType.Palette,
                    FilterMethod = PngFilterMethod.None
                };

                string compressedFilePath = Path.GetTempFileName();
                await image.SaveAsPngAsync(compressedFilePath, encoder);
                return compressedFilePath;
            }
        }

        private async Task<Domain.Entities.Product> GetProductToPromoteAsync(string category, CancellationToken cancellationToken = default)
        {
            try
            {
                var productCategory = await _context.ProductCategories
                    .Where(x => x.Name == category)
                    .FirstOrDefaultAsync(cancellationToken);

                if (productCategory == null)
                {
                    throw new Exception("Category not found");
                }

                var products = await _context.Products
                    .Where(x => x.CategoryId == productCategory.Id && x.Active)
                    .ToListAsync(cancellationToken);

                if (!products.Any())
                {
                    throw new Exception("No active products found for the specified category");
                }

                var random = new Random();
                int index = random.Next(products.Count);
                return products[index];
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw;
            }
        }

        private async Task<string> GetStringedCategoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                string categories = "";
                var categoryList = await _context.ProductCategories.Where(x=>x.Active == true).ToListAsync();
                for (int i = 0; i < categoryList.Count; i++)
                {
                    categories += $"{i + 1}. {categoryList[i].Name}, ";
                }

                return categories;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }

        private async Task<List<Domain.Entities.Product>> GetRandomProductsAsync(int numberOfProducts, CancellationToken cancellationToken)
        {
            try
            {
                var productList = await _context.Products
                                        .Where(k => k.Active == true && k.VendorName.ToLower() != "Amazon".ToLower())
                                        .ToListAsync(cancellationToken);

                if (productList.Count <= numberOfProducts)
                {
                    return productList;
                }
                else
                {
                    var randomProducts = new List<Domain.Entities.Product>();
                    var rnd = new Random();

                    var selectedIndices = new HashSet<int>();
                    while (randomProducts.Count < numberOfProducts)
                    {
                        int index = rnd.Next(productList.Count);
                        if (selectedIndices.Add(index))
                        {
                            randomProducts.Add(productList[index]);
                        }
                    }
                    return randomProducts;
                }
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        private async Task<(string primaryKeyword, string secondaryKeyword, string longTailKeyword)> GetRandomKeywordsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var rnd = new Random();

                var primaryKeywords = await _context.SEOKeywords
                    .Where(k => k.Type == KeywordType.Primary)
                    .Select(k => k.Keyword)
                    .ToListAsync(cancellationToken);


                var secondaryKeywords = await _context.SEOKeywords
                    .Where(k => k.Type == KeywordType.Secondary)
                    .Select(k => k.Keyword)
                    .ToListAsync(cancellationToken);

                var longTailKeywords = await _context.SEOKeywords
                    .Where(k => k.Type == KeywordType.LongTail)
                    .Select(k => k.Keyword)
                    .ToListAsync(cancellationToken);

                string primaryKeyword = primaryKeywords.OrderBy(k => rnd.Next()).FirstOrDefault();
                string secondaryKeyword = secondaryKeywords.OrderBy(k => rnd.Next()).FirstOrDefault();
                string longTailKeyword = longTailKeywords.OrderBy(k => rnd.Next()).FirstOrDefault();

                return (primaryKeyword, secondaryKeyword, longTailKeyword);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw;
            }
        }

        private async Task<string> FormatBlogPostToHtmlAndInsertProductLink(string blogPostContent, List<Domain.Entities.Product> productList, Domain.Entities.Product productToPromote)
        {
                int titleMarkerIndex = blogPostContent.IndexOf("TITLE:\n");
                if (titleMarkerIndex != -1)
                {
                    int titleStartIndex = titleMarkerIndex + "TITLE:\n".Length;
                    int titleEndIndex = blogPostContent.IndexOf("\n", titleStartIndex);
                    titleEndIndex = titleEndIndex == -1 ? blogPostContent.Length : titleEndIndex;

                    string titleText = blogPostContent.Substring(titleStartIndex, titleEndIndex - titleStartIndex).Trim().Trim('"');
                    blogPostContent = "<h1 class=\"m7nrultbfc\" style=\"\">" + titleText + "</h1>" + blogPostContent.Substring(titleEndIndex + 1);
                }

            string encodedProductName = System.Net.WebUtility.HtmlEncode(productToPromote.ProductName);

            string updatedAffiliateLink = productToPromote.AffiliateLink.Replace("zzzzz", _affiliate);

            string linkedProductName = $"<a href='{updatedAffiliateLink}' target='_blank' class='acfU3b1AHX' style=''>{encodedProductName}</a>";
            blogPostContent = blogPostContent.Replace(productToPromote.ProductName, linkedProductName);

            for (int i = 0; i < productList.Count; i++)
            {
                string adHtml = await GetAdvertisementHtml(blogPostContent, productList[i]);

                int placeholderIndex = blogPostContent.IndexOf("[advertisement]");

                if (placeholderIndex != -1)
                {
                    blogPostContent = blogPostContent.Remove(placeholderIndex, "[advertisement]".Length).Insert(placeholderIndex, adHtml);
                }
                else
                {
                    break;
                }
            }

            return blogPostContent;
        }
        private async Task<string> GetAdvertisementHtml(string content, Domain.Entities.Product product = null)
        {
            var productName = System.Net.WebUtility.HtmlEncode(product.ProductName);
            string updatedAffiliateLink = System.Net.WebUtility.HtmlEncode(product.AffiliateLink.Replace("zzzzz", "medamri"));

            string firstBulletPoint = await GenerateSectionAsync($"Write down 1 very short sentence, the most important fact about this product: {product.Description}");
            string secondBulletPoint = await GenerateSectionAsync($"Write down 1 very short sentence, the 2nd most important fact about this product: {product.Description}");
            string thirdBulletPoint = await GenerateSectionAsync($"Write down 1 very short sentence, the 3rd most important fact about this product: {product.Description}");

            string bulletPoints = $@"
                <li class=""z5u6ygxkhb""><span class=""woi1dxuc1l"">•</span>{firstBulletPoint}</li>
                <li class=""z5u6ygxkhb""><span class=""woi1dxuc1l"">•</span>{secondBulletPoint}</li>
                <li class=""z5u6ygxkhb""><span class=""woi1dxuc1l"">•</span>{thirdBulletPoint}</li>
        ";
            return $@"
                    <div class=""yu5kvzao1k"">
                        <div class=""q63fh1n5n6"">
                            FEATURED PARTNER OFFER
                        </div>
                        <div class=""ofu5n6af6r"">
                            <div class=""row"">
                                <div class=""col-lg-12"">
                                    <div class=""sn931i665u"">{productName}</div>
                                </div>
                            </div>
                            <div class=""row"">
                                <div class=""col-lg-3"">
                                    <div class=""parent-nWQRd354Je"">
                                        <div class=""nWQRd354Je"">
                                            <a href=""{updatedAffiliateLink}"">
                                                <img src=""{product.Image}"" alt=""Product Image"" class=""iRTWT8nXBC img-fluid"">
                                            </a>
                                        </div>
                                    </div>
                                </div>
                                <div class=""col-lg-9"">
                                    <ul class=""kz1a5bt068"">
                                       {bulletPoints}
                                    </ul>
                                </div>
                            </div>
                            <div class=""row"">
                                <div class=""col-lg-12"">
                                    <div class=""t7aluspha4 btn btn-primary"" onclick=""window.location.href='{updatedAffiliateLink}';"">LEARN MORE</div>
                                </div>
                            </div>
                            <div class=""row"">
                                <div class=""col-lg-12"">
                                    <div class=""c9uxf3f2wb"">On Product's Official Website</div>
                                </div>
                            </div>
                        </div>
                    </div>
                ";
        }

        private async Task<string> ConvertBackLinkKeywordsToJSONAsync(string backLinkKeywordList)
        {
            try
            {
                string fixedBackLinkKeywordList = FixJsonFormatting(backLinkKeywordList);

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fixedBackLinkKeywordList)))
                {
                    var keywordDictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, int>>(stream);

                    var keywordScores = keywordDictionary.Select(kvp => new KeywordScore
                    {
                        Keyword = kvp.Key,
                        Score = kvp.Value
                    }).ToList();

                    using (var outputStream = new MemoryStream())
                    {
                        await JsonSerializer.SerializeAsync(outputStream, keywordScores);
                        outputStream.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(outputStream))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                return "[]";
            }
        }

        private string FixJsonFormatting(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            json = Regex.Replace(json, "(?<=\\{|,)(\\s*)([^\"\\s\\}]+)(\\s*:\\s*)", "\"$2\":");

            return json;
        }

        public async Task<string> GenerateTagsAsync(string content, Guid blogPostId, CancellationToken cancellationToken)
        {
            string tagsPrompt = $"Generate two to five tags for this content: \"{content}\". Make sure to separate the tags by comma ','.";

            string tags = await GenerateSectionAsync(tagsPrompt, cancellationToken);

            await ProcessTagsAsync(tags, blogPostId, cancellationToken);

            return tags;
        }
        private async Task ProcessTagsAsync(string tags, Guid blogPostId, CancellationToken cancellationToken)
        {
            var tagNames = tags.Split(',').Select(tag => tag.Trim()).Distinct();

            foreach (var tagName in tagNames)
            {
                if (string.IsNullOrEmpty(tagName)) continue;

                var tag = await _context.Tag.FirstOrDefaultAsync(t => t.Name == tagName, cancellationToken);
                if (tag == null)
                {
                    tag = new Domain.Entities.Tag { Name = tagName.ToLower() };
                    await _context.Tag.AddAsync(tag);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var existingLink = await _context.BlogPostTags
                    .AnyAsync(bpt => bpt.BlogPostID == blogPostId && bpt.TagID == tag.Id, cancellationToken);

                if (!existingLink)
                {
                    await _context.BlogPostTags.AddAsync(new Domain.Entities.BlogPostTags { BlogPostID = blogPostId, TagID = tag.Id });
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
        private async Task<string> PostOpenAiRequestAsync(ChatCompletionRequest chatCompletionRequest, CancellationToken cancellationToken)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var body = JsonSerializer.Serialize(chatCompletionRequest, options);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("v1/chat/completions", content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync(cancellationToken);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    Debug.WriteLine(errorContent);
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            throw new Exception("Bad request to the API. Details: " + errorContent);
                        case System.Net.HttpStatusCode.Unauthorized:
                            throw new Exception("Unauthorized access. Make sure your API key is correct.");
                        case System.Net.HttpStatusCode.Forbidden:
                            throw new Exception("Access forbidden. Check API key permissions.");
                        case System.Net.HttpStatusCode.NotFound:
                            throw new Exception("The requested resource was not found.");
                        case System.Net.HttpStatusCode.TooManyRequests:
                            throw new Exception("Rate limit exceeded. Try again later.");
                        default:
                            throw new Exception($"Unexpected API error: {response.StatusCode}. Details: {errorContent}");
                    }
                }
                throw new InvalidOperationException("Failed to generate content from OpenAI.");
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }

        }
        private async Task<string> ExtractGeneratedText(string responseString)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var response = JsonSerializer.Deserialize<ChatCompletionResponse>(responseString, options);
                if (response == null || response.Choices == null || !response.Choices.Any())
                {
                    return "Failed to extract content. No choices available.";
                }

                var firstChoiceContent = response.Choices.FirstOrDefault()?.Message?.Content;
                if (string.IsNullOrEmpty(firstChoiceContent))
                {
                    return "Failed to extract content. The first choice is empty.";
                }

                return firstChoiceContent;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

        #endregion

    }
}
