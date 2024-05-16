using Amazon.S3;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Mvc;
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
        private readonly string niche1;
        private readonly string niche2;
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";
        private readonly IPreplexityService _preplexityService;
        private readonly IAmazonS3Service _amazonS3Service;
        int numberOfAdvertisements = 4;
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
            niche1 = configuration["Blogging:Niche1:Name"];
            niche2 = configuration["Blogging:Niche2:Name"];
            _preplexityService = preplexityService;
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

                string introduction = await GenerateSectionAsync(settings.IntroductionPrompt.Replace("%%TITLE%%", title), cancellationToken);
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(settings.ContentPrompt.Replace("%%TITLE%%", title), cancellationToken);
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                var productList = await GetRandomProductsAsync(settings.NumberOfAdvertisements + 1, cancellationToken);
                string callToAction = await GenerateSectionAsync(settings.CallToActionPrompt
                    .Replace("%%PRODUCT_NAME%%", productList[0].ProductName)
                    .Replace("%%PRODUCT_DESCRIPTION%%", productList[0].Description)
                    .Replace("%%TITLE%%", title),cancellationToken);
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string concepts = await GenerateSectionAsync(settings.ImageConceptsPrompt, cancellationToken);

                string backLinkKeywordList = await GenerateSectionAsync(settings.BackLinkKeywordsPrompt.Replace("%%TITLE%%", title), cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);

                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                return new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = settings.CategoryId,
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    //Image = await GenerateImageAsync(settings.ImagePrompt.Replace("%%IMAGE_CONCEPTS%%", concepts))
                    Image = "https://thewellnessjunctionbucket.s3.eu-north-1.amazonaws.com/6f013177-c7e3-45ae-88c4-f848a5b6de58.jpg"
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



        public async Task<BlogPostResponse> GenerateSEOFocusedBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                var (primaryKeyword, secondaryKeyword, longTailKeyword) = await GetRandomKeywordsAsync(cancellationToken);

                string titlePrompt = $"Generate an engaging and SEO-rich title for a blog post focused on {primaryKeyword}.";

                string introductionPrompt = $"Write a clear and attention-grabbing introduction for a blog post that introduces effective strategies for {primaryKeyword} and {secondaryKeyword}. Use [b] and [/b] for bold text, [i] and [/i] for italic text. Please structure the introduction into short paragraphs.";

                string contentPrompt = $"Provide a detailed guide on the best fitness programs and dietary supplements for {primaryKeyword}, including tips for {secondaryKeyword}. Highlight how {longTailKeyword} can be beneficial. Structure the content with subtitles, each subtitle wrapped inside '[sub]' and '[/sub]'. Use bullet points, use '[b]' and '[/b]' for bold text, and '[i]' and '[/i]' for italic text. (YOU MUST MAKE SURE TO OPEN AND CLOSE THE TAGS PERFECTLY CAREFULLY!!)";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string ctaTitlePrompt = $"Generate a very simple CTA title for an affiliated section on a blog post with the topic on {title}.";

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a decimanl number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);

                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                finalHtmlContent = _globalHelper.RemoveTextWithinSquareBrackets(finalHtmlContent);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("79320ffc-726e-4392-9b3a-ee6c2b64bd3f"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateMythBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = $"Create a captivating blog post title that reveals unexpected insights into everyday health and" +
                    $" lifestyle topics. The title should uncover a common myth, provide a solution to a frequent concern, or share" +
                    $" expert advice in a surprising and thought-provoking way. The title should combine a sense of urgency with " +
                    $"practical advice, and be structured in a way that instantly grabs attention by highlighting little-known facts " +
                    $"or suggesting transformative benefits.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Write a clear and attention-grabbing introduction for a blog post about this topic: '{title}'. Use [b] and [/b] for bold text, [i] and [/i] for italic text. Please structure the introduction into short paragraphs.";

                string contentPrompt = $"Provide detailed and professional information on this topic: {title}. " +
                    $"Structure the content with subtitles, each subtitle wrapped inside '[sub]' and '[/sub]'." +
                    $" Use bullet points, use '[b]' and '[/b]' for bold text, and '[i]' and '[/i]' for italic text. " +
                    $"(YOU MUST MAKE SURE TO OPEN AND CLOSE THE TAGS PERFECTLY CAREFULLY!!)";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify and list the key concepts or topics from the following blog title: '{title}'. Split the concepts by comma.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = $"Create a detailed prompt for an illustrative, split-design image that conveys the following key concepts: {concepts}. The left side should depict the absence or negative aspect related to the concepts, and the right side should show the positive outcome or presence of these elements. The style should be colorful, friendly, and clear enough to be understood at a glance.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);
                

                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("79320ffc-726e-4392-9b3a-ee6c2b64bd3f"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateWeightLossBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = "Generate a dynamic blog post title related to weight loss that captures the reader's attention. " +
                    "The title could address emerging diet trends, debunk myths about weight loss, offer insights into effective workout routines, " +
                    "or explore the psychological aspects of losing weight. Aim for variety, ensuring that each title sparks curiosity " +
                    "and offers a unique take on the weight loss journey.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = "Craft a compelling introduction for the blog post titled '" + title + "'. Use [b] and [/b] for bold text, " +
                    "[i] and [/i] for italic text, to emphasize key points. The introduction should set the stage for an insightful exploration " +
                    "of the topic, encouraging readers to continue for valuable tips, debunked myths, or novel weight loss strategies.";

                string contentPrompt = "Delve into the details of '" + title + "', providing readers with actionable advice, scientific findings, " +
                    "or motivational stories. Organize the content with engaging subtitles, formatted as '[sub][/sub]', and utilize bullet points " +
                    "for clarity. Incorporate bold ([b][/b]) and italic ([i][/i]) text for emphasis, ensuring the article is as informative as it is engaging.";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = "Inspire readers to take action towards their weight loss goals by highlighting the benefits " +
                    "of integrating specific products, like '" + productList[0].ProductName + "', into their routines. Discuss the product " +
                    "in a way that emphasizes its relevance to the blog's theme, focusing on benefits and features without direct promotion. " +
                    "Maintain an informative tone, structuring the call to action in short, impactful paragraphs.";

                // ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = "From the blog title '" + title + "', extract key concepts or themes that can be visually represented. " +
                    "List these concepts, aiming for a mix that could include dieting tips, exercise routines, motivational elements, or myth debunking, " +
                    "to guide the creation of a compelling image that aligns with the weight loss theme.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = "Based on key concepts: " + concepts + ", create a detailed prompt for an image that juxtaposes common " +
                    "weight loss myths with truths, showcases before-and-after scenarios, or illustrates a journey of transformation. The image " +
                    "should be vibrant and engaging, clearly conveying a positive message about weight loss.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = "For enhancing SEO in the weight loss category, generate a list of 5-10 diverse and SEO-rich keywords or phrases, " +
                    "based on the blog post titled '" + title + "'. These should be unique, aiming for a mix of broad appeal and niche specificity, " +
                    "to attract high-quality backlinks. Format the keywords in JSON with 'keyword' as the key and a score from 0 to 100 for each." +
                    "Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);

                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("79320ffc-726e-4392-9b3a-ee6c2b64bd3f"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateBrightsideBlogPostTitleAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = $"Create a captivating blog post title that offers a straightforward dietary solution to a " +
                    $"common health concern. The title should hint at an unexpected yet easy change in the reader's morning routine" +
                    $" that could have a significant positive impact on their health. Incorporate numbers to suggest specific advice" +
                    $" or benefits, and make sure the title arouses curiosity while promising practical value.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Write a compelling introduction for a blog post focusing on {title}. The introduction " +
                    $"should empathize with the reader's frustrations or " +
                    "discomforts regarding this issue. Then, hint at a simple yet unexpected dietary change available in their" +
                    " morning routine that could offer significant benefits. The tone should be friendly and reassuring, promising " +
                    "that the solution will be easy to incorporate into their daily life and backed by nutritional insights. The goal " +
                    "is to make the reader feel understood and intrigued about the solution, encouraging them to continue reading for " +
                    "practical advice.";


                string contentPrompt = $"Craft engaging and detailed content for the blog post with a focus on this topic: '{title}'. " +
                    "Begin by presenting the problem or question in a way that resonates with the reader's experiences. " +
                    "Then, introduce the solution or key insights, breaking down the information into easily digestible segments. " +
                    $"Use [sub]subheadings[/sub] to organize the content into clear sections, each offering a specific piece of advice, insight, or information related to '{title}'. " +
                    "Incorporate bullet points to list out tips, steps, or benefits, making the content skimmable and easy to follow. " +
                    "Highlight critical information or surprising facts using [b]bold[/b] text and emphasize important concepts or terms with [i]italic[/i] text. " +
                    "Throughout, maintain a conversational and positive tone, aiming to educate and motivate the reader. " +
                    "Ensure all formatting tags are used correctly to enhance readability and engagement.";


                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify 1 key concept from this blog title: '{title}'.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = $"Based on this identified key concept '{conceptsForImagePrompt}', create a short and simple prompt for an " +
                "image that represents these nutritional advice themes.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);


                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("7f002904-1bb0-4bbb-bd2f-ecb88dc3122e"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateNutritionalAdviceBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = $"Create a compelling blog post title centered around diet and nutritional advice for weight loss. " +
                    "The title should debunk common dietary myths, highlight the importance of balancing macronutrients, " +
                    "or offer insights into reading food labels. Aim for a title that combines intrigue with practical guidance, " +
                    "and motivates readers to discover nutritional truths that can transform their approach to weight loss.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Craft a clear and engaging introduction for a blog post on the topic: '{title}'. " +
                    $"Use [b] and [/b] for bold text, [i] and [/i] for italic text. Structure the introduction to outline the post's"+
                    $"key points in short paragraphs, setting the stage for an informative deep dive into nutritional advice.";

                string contentPrompt = $"Deliver comprehensive and expertly curated information on the topic: {title}. Organize the content " +
                "with subtitles, using '[sub]' and '[/sub]' for subtitles. Include bullet points for lists, and use text formatting " +
                "(bold and italic) to emphasize key points. Ensure all formatting tags are correctly opened and closed.";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify key concepts or topics from the blog title: '{title}'.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = "Based on the identified key concepts, create a detailed prompt for an illustrative, " +
                "image that visually represents these nutritional advice themes. " +
                "Opt for a style that's colorful, engaging, and informative, making " +
                "complex concepts accessible at a glance.";
                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);


                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("fe74be57-a371-4a55-99a9-a8493f5b5f82"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateExerciseGuidesBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = "Generate a dynamic and motivating blog post title about exercise guides. " +
                    "The title should appeal to both beginners and advanced fitness enthusiasts, covering aspects like home workouts, " +
                    "gym routines, and outdoor exercises. Ensure the title encapsulates the spirit of inclusivity and the benefits of regular exercise.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Compose a compelling introduction for a blog post titled '{title}'. " +
                    "Emphasize the importance of physical activity for all fitness levels and environments. Use [b] and [/b] for bold text, " +
                    "[i] and [/i] for italic text, and ensure the introduction is broken into short, engaging paragraphs.";

                string contentPrompt = $"Create detailed and informative content for the blog post: {title}. Include workout plans, " +
                    "exercise routines, and tips suitable for a wide range of individuals, from beginners to advanced. Structure the content " +
                    "with [sub]subtitles[/sub] for each section, such as home workouts, gym routines, and outdoor exercises. Incorporate bullet points, " +
                    "and use [b]bold[/b] and [i]italic[/i] text for emphasis. All formatting tags must be correctly opened and closed.";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify key concepts or topics from the blog title: '{title}'.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = $"Based on the identified key concepts {conceptsForImagePrompt}, create a detailed prompt for an " +
                "image that represents these nutritional advice themes.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);


                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("a213dda9-e0a8-4869-9d9b-c3e74d2e39a5"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateRecipesAndMealPlansBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = "Craft an enticing blog post title that showcases healthy, low-calorie recipes and meal plans. " +
                   "The title should communicate the ease of meal prep and the benefits of following these meal plans for weight loss and healthy living. " +
                   "Aim for a title that is both inviting and informative, encouraging readers to explore simple yet nutritious meal options.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Compose an engaging introduction for a blog post with the title '{title}', focusing on healthy eating habits. " +
                    "Highlight the significance of low-calorie recipes and well-planned meals in achieving weight loss goals. Use [b] and [/b] for bold text, " +
                    "[i] and [/i] for italic text. Structure the introduction to captivate the reader's interest in discovering delicious and nutritious meal options.";

                string contentPrompt = $"Develop in-depth and appetizing content for the blog post: {title}. Feature healthy, low-calorie recipes " +
                    "and offer practical weekly meal plans that aid in weight loss. Structure the content with [sub]subtitles[/sub] for each recipe and meal plan, " +
                    "emphasize the nutritional benefits, and use bullet points for ingredients and steps. Employ [b]bold[/b] and [i]italic[/i] text for emphasis. " +
                    "Ensure all formatting tags are correctly opened and closed.";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify 1 key concept from this blog title: '{title}'.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = $"Based on this identified key concept '{conceptsForImagePrompt}', create a short and simple prompt for an " +
                "image that represents these nutritional advice themes.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);


                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("fe74be57-a371-4a55-99a9-a8493f5b5f82"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateNumberedBlogPostTitleAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = $"Generate a compelling blog post title in the category of weight loss that includes a number. " +
                    "The title should promise actionable advice, tips, or insights for readers interested in weight loss, " +
                    "such as revealing foods that aid in weight loss, simple exercise routines, or myths debunked. " +
                    "The title must be engaging, informative, and include a list format, e.g., '7 Foods That...', '5 Ways To...', etc.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Compose an engaging introduction for a blog post with the title '{title}'. " +
                    "Use [b] and [/b] for bold text, " +
                    "[i] and [/i] for italic text.";

                string contentPrompt = $"Develop in-depth content for the blog post: {title}. Feature healthy." +
                    $"Structure the content with [sub]subtitles[/sub], " +
                    "Also you can use bullet points. Employ [b]bold[/b] and [i]italic[/i] text for emphasis. " +
                    "Ensure all formatting tags are correctly opened and closed.";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify 1 key concept from this blog title: '{title}'.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = $"Based on this identified key concept '{conceptsForImagePrompt}', create a short and simple prompt for an " +
                "image that represents these nutritional advice themes.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);


                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("79320ffc-726e-4392-9b3a-ee6c2b64bd3f"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateLatestNewsBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {

                var randomNews = await _context.News
                    .FromSqlRaw("SELECT * FROM \"News\" ORDER BY RANDOM() LIMIT 1")
                    .FirstOrDefaultAsync();


                string titlePrompt = $"Generate a captivating blog post title about this topic \"{randomNews.Title}\".";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Begin the blog post on '{title}' with a clear and direct introduction. " +
                    $"Provide essential insights based on the following details: '{randomNews.Description}'. " +
                    "Focus on delivering information in a straightforward manner, avoiding any dramatic language. " +
                    "Incorporate [b]bold[/b] and [i]italic[/i] text for emphasis where it naturally fits, " +
                    "and structure the introduction into concise paragraphs that engage the reader with facts and thoughtful analysis.";



                string contentPrompt = $"Craft professional and detailed content for the blog post titled '{title}'." +
                    $" Organize the content with subtitles, using '[sub]' and '[/sub]' " +
                    $"for subtitles, " +
                    "Include bullet points." +
                    "Maintain a conversational and uplifting tone throughout, and write as much as you can, inform the reader as much as you can." +
                    "Use [b] and [/b] for bold text, [i] and [/i] for italic text. Please structure the introduction into short paragraphs." +
                    $"(YOU MUST MAKE SURE TO OPEN AND CLOSE THE TAGS PERFECTLY CAREFULLY!!)"; ;

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify 1 key concept from this blog title: '{title}'.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = $"Based on this identified key concept '{conceptsForImagePrompt}', create a short and simple prompt for an " +
                "image that represents these nutritional advice themes.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT

                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);


                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);



                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("3ee72486-7a14-474f-a4ac-a45f93eb49e6"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateMindsetAndMotivationBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = "Generate a captivating blog post title about the psychological aspects of losing weight, " +
                    "such as maintaining motivation, dealing with setbacks, and building healthy habits. The title should be " +
                    "inspiring and include a number to suggest practical tips or insights.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = "Write an engaging introduction for a blog post focusing on 'Mindset and Motivation' " +
                   "for weight loss. Start by empathizing with the reader's struggles in maintaining motivation and dealing with setbacks. " +
                   "Hint at offering effective strategies for building healthy habits and sustaining a positive mindset throughout their weight loss journey." +
                   "Use [b] and [/b] for bold text, [i] and [/i] for italic text. Please structure the introduction into short paragraphs.";


                string contentPrompt = $"Craft engaging and detailed content for the blog post titled '{title}', focusing on the psychological " +
                    "aspects of losing weight. Organize the content with subtitles, using '[sub]' and '[/sub]' for subtitles, covering maintaining motivation, " +
                    "dealing with setbacks, and building healthy habits. Include bullet points for actionable tips, and highlight key insights " +
                    "in bold or italic for emphasis. Maintain a conversational and uplifting tone throughout." +
                    "Use [b] and [/b] for bold text, [i] and [/i] for italic text. Please structure the introduction into short paragraphs." +
                    $"(YOU MUST MAKE SURE TO OPEN AND CLOSE THE TAGS PERFECTLY CAREFULLY!!)"; ;

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = $"Encourage readers to explore our recommended supplements and programs for achieving their " +
                $"goals. Highlight the benefits of integrating quality products into their regimen. Talk about the product called " +
                $"\"{productList[0].ProductName}\" " +
                $"and explain why this product is outstanding, focusing solely on the product's features and benefits." +
                $"Structure the call to action in short paragraphs. " +
                $"Please avoid personalizing the content to any specific company, do not insert any links or suggest using " +
                $"a discount code. The focus should be on the generic advantages and qualities of the product.";

                //ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = $"Identify 1 key concept from this blog title: '{title}'.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = $"Based on this identified key concept '{conceptsForImagePrompt}', create a short and simple prompt for an " +
                "image that represents these nutritional advice themes.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                //ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"I'm seeking to enhance my blog's SEO through a targeted backlink strategy" +
                $". The blog focuses on {niche1} and {niche2}, with the latest post covering {title}. " +
                $"Here's a brief summary of the post: {introduction}. Based on this, could you generate a list of " +
                $"5-10 SEO-rich, unique, and potentially high-impact keywords or phrases that other websites and blogs might use to " +
                $"link back to this post? These keywords should align with the post's content, have a blend of search volume and " +
                $"specificity to target niche audiences, and be able to attract quality backlinks from reputable sources within " +
                $"{niche1} and {niche2}.The format of the keywords should be JSON. The key will be called 'keyword' and the value will" +
                $"be the score (a number from 0 to 100) for that keyword. " +
                $"Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);


                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("ac1641f2-01cd-4d0e-ad1d-5baeceae5c35"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GenerateHealthyLifestyleBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = $"Create a captivating blog post title that offers fresh perspectives on healthy living. " +
                    "Focus on debunking popular health myths, introducing easy-to-adopt healthy habits, or providing innovative " +
                    "tips for maintaining a balanced lifestyle. The title should intrigue readers by challenging common beliefs, " +
                    "highlighting unexpected benefits of simple habits, or revealing expert insights into wellness practices.";

                string title = await GenerateSectionAsync(titlePrompt, cancellationToken);

                string introductionPrompt = $"Begin the blog post on '{title}' with a clear and direct introduction. " +
                    "Focus on delivering information in a straightforward manner, avoiding any dramatic language. " +
                    "Incorporate [b]bold[/b] and [i]italic[/i] text for emphasis where it naturally fits, " +
                    "and structure the introduction into concise paragraphs that engage the reader with facts and thoughtful analysis.";

                string contentPrompt = "Delve into the subject of '" + title + "' with a focus on practical advice, evidence-based practices, " +
                    "and relatable examples. Organize the content with clear subtitles, each within '[sub]' and '[/sub]'. Include " +
                    "bullet points for easy reading, and use text formatting ([b], [/b], [i], [/i]) to emphasize key points. " +
                    "Ensure all formatting tags are correctly opened and closed.";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = "Encourage readers to embrace the recommended products and strategies for enhancing their " +
                    "health and wellness routines. Detail the features and benefits of '" + productList[0].ProductName + "', " +
                    "emphasizing its relevance to the blog's theme without direct promotion or links. Construct the call to action " +
                    "in concise paragraphs, focusing on the value these recommendations add to a healthy lifestyle.";

                // ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = "Extract the core concepts or themes from the blog title: '" + title +
                    "'. List these concepts, separated by commas, to guide the creation of a visually appealing image that reflects " +
                    "the blog's content.";

                string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                string promptForImagePrompt = "Develop a detailed prompt for a split-design image that visually contrasts the misconceptions " +
                    "and the truths about healthy living, based on the key concepts: " + concepts + ". The left side should portray common " +
                    "myths or unhealthy practices, while the right side showcases healthy habits or truths. Design the image to be colorful, " +
                    "engaging, and easy to understand, effectively communicating the positive shift towards a healthier lifestyle.";

                string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                string image = await GenerateImageAsync(imagePrompt);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = $"In an effort to boost SEO for a blog focused on healthy lifestyles and habits, " +
                    "and considering the latest post titled '" + title + "', generate a list of 5-10 SEO-rich keywords or phrases. " +
                    "These should be unique, likely to draw high-quality backlinks, and relevant to the content's focus on wellness. " +
                    "Present the keywords in JSON format with 'keyword' as the key and a relevance score from 0 to 100 as the value." +
                    "Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);

                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("294f283a-76a1-4c3e-9e3f-9450570af6af"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
        public async Task<BlogPostResponse> GeneratePersonalStoryBlogPostAsync(CancellationToken cancellationToken)
        {
            StringBuilder blogPostContent = new StringBuilder();
            try
            {
                string titlePrompt = $"Create a compelling and personal blog post title about 'health or weight loss'. " +
                            "Think about a journey or personal experience of a person (you must use 'I' for the person)" +
                            " that can inspire others.";

                string title = "I Lost 50 Pounds and Gained a New Lease on Life: My Journey to Optimal health and happiness";

                string introductionPrompt = $"Write an engaging and personal introduction for the blog post titled '{title}'. " +
                            $"Discuss the personal connection to the topic, '{title}' (do not write the title in the content) " +
                            "and set the stage for a story that is both informative and deeply personal." +
                            "You must use 'I' for the person in the blog post." +
                            "Incorporate [b]bold[/b] and [i]italic[/i] text for emphasis where it naturally fits, " +
                            "and structure the introduction into concise paragraphs that engage the reader with facts and thoughtful analysis." +
                           "Ensure all formatting tags are correctly opened and closed."; ;

                string contentPrompt = $"Develop a heartfelt and detailed narrative for the blog post '{title}'. " +
                           "Use anecdotes and personal experiences to illustrate key points about health and wellness. " +
                           "Structure your content with emotional depth, using natural dialogues, reflections, " +
                           "and lessons learned along the way. Include practical tips that you've found personally beneficial." +
                           "You must use 'I' for the person in the blog post."+
                           "Organize the content with clear subtitles, each within '[sub]' and '[/sub]' tags, so make sure to put [sub][/sub] tags.Include " +
                           "bullet points for easy reading, these are the tags for bullet points [b]bold[/b], and use italic tags for text formatting [i]italic[/i]," +
                           " to emphasize key points. " +
                           "Ensure all formatting tags are correctly opened and closed.";

                var productList = await GetRandomProductsAsync(numberOfAdvertisements + 1, cancellationToken);
                string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

                string callToActionPrompt = "Encourage readers to embrace the recommended products and strategies for enhancing their " +
                    "health and wellness routines. Detail the features and benefits of '" + productList[0].ProductName + "', " +
                    "emphasizing its relevance to the blog's theme without direct promotion or links. Construct the call to action " +
                    "in concise paragraphs, focusing on the value these recommendations add to a healthy lifestyle.";

                // ADVERTISEMENT
                blogPostContent.AppendLine($"TITLE:\n{title}");
                blogPostContent.AppendLine($"[advertisement]");

                string introduction = await GenerateSectionAsync(introductionPrompt, cancellationToken);

                string conceptsForImagePrompt = "Extract the core concepts or themes from the blog title: '" + title +
                    "'. List these concepts, separated by commas, to guide the creation of a visually appealing image that reflects " +
                    "the blog's content.";

                //string concepts = await GenerateSectionAsync(conceptsForImagePrompt, cancellationToken);

                //string promptForImagePrompt = $"Based on this identified key concept '{conceptsForImagePrompt}', create a short and simple prompt for an " +
                                       //"image that represents these nutritional advice themes." +
                                       //"Make sure to not violate the OPENAI content policy.";

                //string imagePrompt = await GenerateSectionAsync(promptForImagePrompt, cancellationToken);

                //string image = await GenerateImageAsync(imagePrompt);

                string image = "https://thewellnessjunctionbucket.s3.eu-north-1.amazonaws.com/6f013177-c7e3-45ae-88c4-f848a5b6de58.jpg";

                // ADVERTISEMENT
                blogPostContent.AppendLine($"INTRODUCTION:\n{introduction}");
                blogPostContent.AppendLine($"[advertisement]");

                string mainContent = await GenerateSectionAsync(contentPrompt, cancellationToken);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"MAINCONTENT:\n{mainContent}");
                blogPostContent.AppendLine($"[advertisement]");

                string callToAction = await GenerateSectionAsync(callToActionPrompt.Replace("%%PRODUCT_NAME%%", productList[0].ProductName), cancellationToken);

                // ADVERTISEMENT
                blogPostContent.AppendLine($"CALLTOACTION:\n{callToAction}");
                blogPostContent.AppendLine($"[advertisement]");

                string backLinkKeywordsPrompt = "In an effort to boost SEO for a blog focused on healthy lifestyles and habits, " +
                    "and considering the latest post titled '" + title + "', generate a list of 5-10 SEO-rich keywords or phrases. " +
                    "These should be unique, likely to draw high-quality backlinks, and relevant to the content's focus on wellness. " +
                    "Present the keywords in JSON format with 'keyword' as the key and a relevance score from 0 to 100 as the value." +
                    "Here is the template you must use: {{\"keyword\", \"score\"}}.";

                string backLinkKeywordList = await GenerateSectionAsync(backLinkKeywordsPrompt, cancellationToken);
                var json = await ConvertBackLinkKeywordsToJSONAsync(backLinkKeywordList);

                string correctedContent = AutoCloseFormattingTags(blogPostContent.ToString());
                string finalHtmlContent = FormatBlogPostToHtmlAndInsertProductLink(correctedContent, productList);

                var removedUnnecessaryBrackets = RemoveWordsInBrackets(finalHtmlContent);

                var response = new BlogPostResponse
                {
                    Title = title.Replace("'", "").Replace("\"", ""),
                    HtmlContent = removedUnnecessaryBrackets,
                    BlogPostCategoryId = Guid.Parse("2c9e1da1-3299-451b-99d4-87418bea9c23"),
                    BackLinkKeywords = json,
                    URL = _globalHelper.TitleToUrlSlug(title),
                    ProductId = productList[0].Id,
                    Image = image
                };

                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }


        #endregion


        #region Private Methods
        private async Task<string> GenerateSectionAsync(string prompt, CancellationToken cancellationToken)
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

        private async Task<Domain.Entities.Product> GetRandomProductAsync(CancellationToken cancellationToken)
        {
            try
            {
                var rnd = new Random();
                var productList = await _context.Products
                                        .Where(k => k.CategoryId == Guid.Parse("66a10c17-79de-44f5-b680-6e2e78079f1e"))
                                        .ToListAsync(cancellationToken);

                if (productList.Any())
                {
                    var random = new Random();
                    var product = productList[random.Next(productList.Count)];
                    return product;
                }

                return null;
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
                                        .Where(k => k.CategoryId == Guid.Parse("66a10c17-79de-44f5-b680-6e2e78079f1e"))
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
        private static string FormatBlogPostToHtml(string blogPostText, List<Domain.Entities.Product> productList)
        {
            int titleMarkerIndex = blogPostText.IndexOf("TITLE:\n");
            if (titleMarkerIndex != -1)
            {
                int titleStartIndex = titleMarkerIndex + "TITLE:\n".Length;
                int titleEndIndex = blogPostText.IndexOf("\n", titleStartIndex);
                titleEndIndex = titleEndIndex == -1 ? blogPostText.Length : titleEndIndex;

                string titleText = blogPostText.Substring(titleStartIndex, titleEndIndex - titleStartIndex).Trim().Trim('"');
                blogPostText = "<h1 class=\"m7nrultbfc\" style=\"\">" + titleText + "</h1>" + blogPostText.Substring(titleEndIndex + 1);
            }

            int ctaTitleMarkerIndex = blogPostText.IndexOf("CTATITLE:\n");
            if (ctaTitleMarkerIndex != -1)
            {
                int ctaTitleStartIndex = ctaTitleMarkerIndex + "CTATITLE:\n".Length;
                int ctaTitleEndIndex = blogPostText.IndexOf("\n", ctaTitleStartIndex);
                ctaTitleEndIndex = ctaTitleEndIndex == -1 ? blogPostText.Length : ctaTitleEndIndex;

                string ctaTitleText = blogPostText.Substring(ctaTitleStartIndex, ctaTitleEndIndex - ctaTitleStartIndex).Trim().Trim('"');
                blogPostText = blogPostText.Substring(0, ctaTitleMarkerIndex) + "<h2 class=\"cw5zqvh8ls\" style=\"\">" + ctaTitleText + "</h2>" + blogPostText.Substring(ctaTitleEndIndex);
            }

            blogPostText = blogPostText.Replace("INTRODUCTION:\n", "<p class=\"m2zfzbb9e2\" style=\"\">")
                                       .Replace("MAINCONTENT:\n", "<div class=\"shgvcukb4y\" style=\"\">")
                                       .Replace("CALLTOACTION:\n", "</div><div class='uhtqsa4iyy' style=\"\"><p class=\"m2zfzbb9e2\" style=''>");

            blogPostText = Regex.Replace(blogPostText, @"\*\*(.+?)\*\*", "<strong>$1</strong>");

            blogPostText = blogPostText.Replace("[sub]", "<h2 class=\"fdc026o2fu\">").Replace("[/sub]", "</h2>")
                                       .Replace("[h3]", "<h3 class=\"fdc026o2fu\">").Replace("[/h3]", "</h3>")
                                       .Replace("[numtitle]", "<h4 class=\"fdc026o2fu\">").Replace("[/numtitle]", "</h4>")
                                       .Replace("[b]", "<strong>").Replace("[/b]", "</strong>")
                                       .Replace("[i]", "<em>").Replace("[/i]", "</em>");

            blogPostText = blogPostText.Replace("[*]", "<ul><li>").Replace("\n[*]", "</li><li>")
                                       .Replace("[**]", "<strong>").Replace("[/b]", "</strong>");
                                       //.Replace("[1.]", "<ol><li>").Replace("\n[1.]", "</li><li>");

            blogPostText = blogPostText.Replace("</li><li></ul>", "</li></ul>")
                                       .Replace("</li><li></ol>", "</li></ol>");
            blogPostText += "</p></div>";

            blogPostText = ConvertListItemsAndParagraphs(blogPostText);

            for (int i = 0; i < productList.Count; i++)
            {
                string adHtml = GetAdvertisementHtml(blogPostText, productList[i]);

                int placeholderIndex = blogPostText.IndexOf("[advertisement]");

                if (placeholderIndex != -1)
                {
                    blogPostText = blogPostText.Remove(placeholderIndex, "[advertisement]".Length).Insert(placeholderIndex, adHtml);
                }
                else
                {
                    break;
                }
            }


            return blogPostText;
        }
        private static string ConvertListItemsAndParagraphs(string content)
        {
            content = content.Replace("[*]", "<ul><li>").Replace("[1.]", "<ol><li>")
                             .Replace("[**]", "<strong>").Replace("[/b]", "</strong>")
                             .Replace("\n[*]", "</li><li>").Replace("\n[1.]", "</li><li>")
                             .Replace("\n", "</p><p class=\"m2zfzbb9e2\">")
                             .Replace("<p><ul>", "<ul>").Replace("<p><ol>", "<ol>")
                             .Replace("</ul></p>", "</ul>").Replace("</ol></p>", "</ol>");

            content = content + "</p>";
            content = content.Replace("<p></li><li>", "<li>").Replace("</li><li></p>", "</li>")
                             .Replace("<p><h2>", "</p><h2>").Replace("<p><h3>", "</p><h3>")
                             .Replace("</h2><p>", "</h2>").Replace("</h3><p>", "</h3>");

            if (!content.StartsWith("<ul>") && !content.StartsWith("<ol>") && !content.StartsWith("<h2>") && !content.StartsWith("<h3>"))
            {
                content = "<p class=\"m2zfzbb9e2\">" + content;
            }
            content = content.Replace("<p><p>", "<p class=\"m2zfzbb9e2\">").Replace("</p></p>", "</p>");

            return content;
        }
        private string FormatBlogPostToHtmlAndInsertProductLink(string blogPostContent, List<Domain.Entities.Product> productList)
        {
            string encodedProductName = System.Net.WebUtility.HtmlEncode(productList[0].ProductName);

            string updatedAffiliateLink = productList[0].AffiliateLink.Replace("zzzzz", _affiliate);

            string linkedProductName = $"<a href='{updatedAffiliateLink}' target='_blank'  style='text-decoration: none; color: #2C20DF !important;'>{encodedProductName}</a>";
            blogPostContent = blogPostContent.Replace(productList[0].ProductName, linkedProductName);

            blogPostContent = FormatBlogPostToHtml(blogPostContent, productList);

            return blogPostContent;
        }
        private static string GetAdvertisementHtml(string content, Domain.Entities.Product product = null)
        {
            var productName = System.Net.WebUtility.HtmlEncode(product.ProductName);
            string updatedAffiliateLink = System.Net.WebUtility.HtmlEncode(product.AffiliateLink.Replace("zzzzz", "medamri"));
            //iRTWT8nXBC
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
                                    <div class=""nWQRd354Je"">
                                        <img src=""https://thumbor.forbes.com/thumbor/fit-in/x/https://www.forbes.com/health/wp-content/uploads/2021/12/Nutrisystem-Diabetes.jpeg"" alt=""Product Image"" class=""iRTWT8nXBC img-fluid"">
                                    </div>
                                </div>
                                <div class=""col-lg-9"">
                                    <ul class=""kz1a5bt068"">
                                        <li class=""z5u6ygxkhb""><span class=""woi1dxuc1l"">•</span>This Product is tailored to meet your unique wellness goals</li>
                                        <li class=""z5u6ygxkhb""><span class=""woi1dxuc1l"">•</span>Carefully chosen for its efficacy and user feedback</li>
                                        <li class=""z5u6ygxkhb""><span class=""woi1dxuc1l"">•</span>Designed with the latest scientific research to ensure the best outcomes</li>
                                        <li class=""z5u6ygxkhb""><span class=""woi1dxuc1l"">•</span>Easy to incorporate into your daily routine for sustained wellness</li>
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
        private static string AutoCloseFormattingTags(string content)
        {
            string[] tags = new[] { "b", "i", "sub", "numtitle", "h3", "em" };

            foreach (var tag in tags)
            {
                string pattern = $@"\[{tag}\](.*?)(?=(\[{tag}\])|(\[/{tag}\])|[,.?!]\s|$)";

                content = Regex.Replace(content, pattern, match =>
                {
                    if (!match.Value.EndsWith($"[/{tag}]"))
                    {
                        return $"[{tag}]{match.Groups[1].Value}[/{tag}]";
                    }
                    return match.Value;
                }, RegexOptions.Singleline);
            }
            return content;
        }
        public static string RemoveWordsInBrackets(string input)
        {
            return Regex.Replace(input, @"\[[^\]]*\]", "");
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
                    tag = new Tag { Name = tagName };
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






        //public async Task<string> GenerateBlogPostAsync(GenerateBlogPostCommand command, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var chatCompletionRequest = await CreateRequestBody(command);
        //        var responseString = await PostOpenAiRequestAsync(chatCompletionRequest, cancellationToken);
        //        var generatedText = await ExtractGeneratedText(responseString);
        //        return await ConvertToHtml(generatedText, command.ProductName, command.AffiliateLink);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _globalHelper.Log(ex, currentClassName);
        //        throw new Exception("An error occurred while processing the request.", ex);
        //    }
        //}

        //#region Private
        //private async Task<ChatCompletionRequest> CreateRequestBody(GenerateBlogPostCommand command)
        //{
        //    try
        //    {
        //        var promptBuilder = new StringBuilder();

        //var primaryKeywords = await _context.BlogPostSEOKeyword
        //                    .Where(k => k.SEOKeyword.Type == KeywordType.Primary)
        //                    .Select(k => k.SEOKeyword.Keyword)
        //                    .ToListAsync();

        //var secondaryKeywords = await _context.BlogPostSEOKeyword
        //                    .Where(k => k.SEOKeyword.Type == KeywordType.Secondary)
        //                    .Select(k => k.SEOKeyword.Keyword)
        //                    .ToListAsync();

        //var longTailKeywords = await _context.BlogPostSEOKeyword
        //                    .Where(k => k.SEOKeyword.Type == KeywordType.LongTail)
        //                    .Select(k => k.SEOKeyword.Keyword)
        //                    .ToListAsync();

        //        promptBuilder.AppendLine("Please generate a blog post with the following structure:");
        //        promptBuilder.AppendLine("TITLE: Insert the title here.");
        //        promptBuilder.AppendLine("BODY: Insert the body content here, including paragraphs.");
        //        promptBuilder.AppendLine("AFFILIATE_LINK: Insert an affiliate link here.");

        //        if (!string.IsNullOrEmpty(command.Topic))
        //        {
        //            promptBuilder.AppendLine($"The blog post is about {command.Topic}.");
        //        }
        //        else
        //        {
        //            promptBuilder.AppendLine($"The blog post is anything related to {niche1} or {niche2}.");
        //        }

        //        if (!string.IsNullOrEmpty(command.Tone))
        //        {
        //            promptBuilder.AppendLine($"The tone should be {command.Tone}.");
        //        }
        //        else
        //        {
        //            promptBuilder.AppendLine($"The tone should be Informative.");
        //        }

        //        if (!string.IsNullOrEmpty(command.TargetAudience))
        //        {
        //            promptBuilder.AppendLine($"The target audience is {command.TargetAudience}.");
        //        }
        //        else
        //        {
        //            promptBuilder.AppendLine($"The target audience is Fitness Enthusiasts.");
        //        }

        //        if (command.Keywords != null && command.Keywords.Any())
        //        {
        //            promptBuilder.AppendLine($"Please include this/these SEO keyword(s): {command.Keywords}.Use this/these keyword(s) near the beginning of the content (ideally in the first paragraph)." +
        //                $" Ensure that this/these keyword(s) fit(s) seamlessly into the content and use this/these keyword(s)" +
        //                $"throughout the body in a way that maintains a natural flow. Also use this/these keyword(s) " +
        //                $"in the conclusion or closing paragraphs");
        //        }
        //        else
        //        {
        //            promptBuilder.AppendLine($"Please include this/these SEO keyword(s): {command.Keywords}.Use this/these keyword(s) near the beginning of the content (ideally in the first paragraph)." +
        //                $" Ensure that this/these keyword(s) fit(s) seamlessly into the content and use this/these keyword(s)" +
        //                $"throughout the body in a way that maintains a natural flow. Also use this/these keyword(s) " +
        //                $"in the conclusion or closing paragraphs");
        //        }

        //        if (!string.IsNullOrEmpty(command.CategoryId.ToString()))
        //        {
        //            var category = await _context.Category.FindAsync(command.CategoryId);
        //            if (category != null)
        //            {
        //                promptBuilder.AppendLine($"The category for this blog post is {category.Name}.");
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(command.ProductId.ToString()))
        //        {
        //            var product = await _context.Product.FindAsync(command.ProductId);
        //            if (product != null)
        //            {
        //                string vendor = product.VendorName;
        //                string affiliateLink = $"https://hop.clickbank.net/?affiliate=zzzzz&vendor={vendor}";
        //                string updatedAffiliateLink = affiliateLink.Replace("zzzzz", _affiliate);

        //                command.ProductName = product.ProductName;
        //                command.AffiliateLink = updatedAffiliateLink;

        //                promptBuilder.AppendLine($"Please include an affiliate promotion for {product.ProductName} using this link: {updatedAffiliateLink}.");
        //            }
        //        }

        //        int maxTokens = command.LengthPreference switch
        //        {
        //            BlogPostLength.Short => 1000,
        //            BlogPostLength.Medium => 2000,
        //            BlogPostLength.Long => 4000,
        //            _ => 2000,
        //        };

        //        return new ChatCompletionRequest
        //        {
        //            Model = _gptModel,
        //            Messages = new List<ChatMessage>
        //        {
        //            new ChatMessage { Role = "system", Content = promptBuilder.ToString() }
        //        },
        //            Temperature = command.CreativityLevel
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await _globalHelper.Log(ex, currentClassName);
        //        throw new Exception("An error occurred while processing the request.", ex);
        //    }
        //}


        //private async Task<string> PostOpenAiRequestAsync(ChatCompletionRequest chatCompletionRequest, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //        };
        //        var body = JsonSerializer.Serialize(chatCompletionRequest, options);
        //        var content = new StringContent(body, Encoding.UTF8, "application/json");
        //        var response = await _httpClient.PostAsync("v1/chat/completions", content, cancellationToken);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return await response.Content.ReadAsStringAsync(cancellationToken);
        //        }
        //        else
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        //            Debug.WriteLine(errorContent);
        //            switch (response.StatusCode)
        //            {
        //                case System.Net.HttpStatusCode.BadRequest:
        //                    throw new Exception("Bad request to the API. Details: " + errorContent);
        //                case System.Net.HttpStatusCode.Unauthorized:
        //                    throw new Exception("Unauthorized access. Make sure your API key is correct.");
        //                case System.Net.HttpStatusCode.Forbidden:
        //                    throw new Exception("Access forbidden. Check API key permissions.");
        //                case System.Net.HttpStatusCode.NotFound:
        //                    throw new Exception("The requested resource was not found.");
        //                case System.Net.HttpStatusCode.TooManyRequests:
        //                    throw new Exception("Rate limit exceeded. Try again later.");
        //                default:
        //                    throw new Exception($"Unexpected API error: {response.StatusCode}. Details: {errorContent}");
        //            }
        //        }
        //        throw new InvalidOperationException("Failed to generate content from OpenAI.");
        //    }
        //    catch (Exception ex)
        //    {
        //        await _globalHelper.Log(ex, currentClassName);
        //        throw new Exception("An error occurred while processing the request.", ex);
        //    }

        //}

        //private async Task<string> ExtractGeneratedText(string responseString)
        //{
        //    try
        //    {
        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        };

        //        var response = JsonSerializer.Deserialize<ChatCompletionResponse>(responseString, options);
        //        if (response == null || response.Choices == null || !response.Choices.Any())
        //        {
        //            return "Failed to extract content. No choices available.";
        //        }

        //        var firstChoiceContent = response.Choices.FirstOrDefault()?.Message?.Content;
        //        if (string.IsNullOrEmpty(firstChoiceContent))
        //        {
        //            return "Failed to extract content. The first choice is empty.";
        //        }

        //        return firstChoiceContent;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _globalHelper.Log(ex, currentClassName);
        //        throw new Exception("An error occurred while processing the request.", ex);
        //    }
        //}

        //private async Task<string> ConvertToHtml(string generatedText, string productName = "", string affiliateLink = "")
        //{
        //    try
        //    {
        //        var htmlBuilder = new StringBuilder();

        //        if (generatedText.Contains("TITLE:"))
        //        {
        //            var title = generatedText.Substring(generatedText.IndexOf("TITLE:") + "TITLE:".Length,
        //                                                generatedText.IndexOf("BODY:") - (generatedText.IndexOf("TITLE:") + "TITLE:".Length));
        //            htmlBuilder.AppendLine($"<h1>{title.Trim()}</h1>");
        //        }

        //        if (generatedText.Contains("BODY:"))
        //        {
        //            var body = generatedText.Substring(generatedText.IndexOf("BODY:") + "BODY:".Length,
        //                                                generatedText.IndexOf("AFFILIATE_LINK:") - (generatedText.IndexOf("BODY:") + "BODY:".Length)).Trim();

        //            var paragraphs = body.Split(new string[] { "\n\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        //            foreach (var paragraph in paragraphs)
        //            {
        //                var processedParagraph = paragraph.Trim();

        //                if (!string.IsNullOrEmpty(productName) && processedParagraph.Contains(productName))
        //                {
        //                    processedParagraph = processedParagraph.Replace(productName, $"<a href='{affiliateLink}'>{productName}</a>");
        //                }

        //                htmlBuilder.AppendLine($"<p>{processedParagraph}</p>");
        //            }
        //        }

        //        if (generatedText.Contains("AFFILIATE_LINK:") && string.IsNullOrEmpty(affiliateLink))
        //        {
        //            affiliateLink = generatedText.Substring(generatedText.IndexOf("AFFILIATE_LINK:") + "AFFILIATE_LINK:".Length).Trim();
        //            htmlBuilder.AppendLine($"<a href='{affiliateLink}'>Check out this product!</a>");
        //        }

        //        var finalHtml = htmlBuilder.ToString().Replace("[AFFILIATE_LINK]", "");

        //        finalHtml += InsertProductLinks(productName, affiliateLink);

        //        return finalHtml;
        //    }
        //    catch (Exception ex)
        //    {
        //        await _globalHelper.Log(ex, currentClassName);
        //        throw new Exception("An error occurred while processing the request.", ex);
        //    }
        //}


        //private string InsertProductLinks(string productName, string affiliateLink)
        //{
        //    return $"\n\n<p><a style=\"color:red;\" href=\"{affiliateLink}\">(SPECIAL PROMO) Click Here to Buy {productName} at a Special Discounted Price While Supplies Last.</a></p>" +
        //        $"\n\n <b>Disclaimer</b>: The above is a sponsored post, the views expressed do not represent the stand and views of TheWellnessJunction Editorial.\r\n\r\n";
        //}

        //#endregion Private

    }
}
