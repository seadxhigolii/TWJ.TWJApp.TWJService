using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPostSettings
    {
        public string TitlePrompt { get; set; }

        public string IntroductionPrompt { get; set; }

        public string ContentPrompt { get; set; }
        public string CallToActionPrompt { get; set; } = $"Here is the info about the product named '%%PRODUCT_NAME%%': %%PRODUCT_DESCRIPTION%%" +
            $"Create a problem that is related to the product and the title '%%TITLE%%'." +
            $"Provide the solution of that problem by suggesting this product '%%PRODUCT_NAME%%'." +
            $"Encourage readers to embrace the recommended product." +
            $"Detail the features and benefits of '%%PRODUCT_NAME%%', " +
            $"emphasizing its relevance to the blog's title '%%TITLE%%' without direct promotion or links. " +
            $"Construct the call to action in concise paragraphs, focusing on the value these recommendations add to a healthy lifestyle."+
            " Use the Markdown Syntax for italic, bold keywords and external backlinks for keywords. " +
            "Do not write references or sources at the end. ";
        public string BackLinkKeywordsPrompt { get; set; } = $"In an effort to boost SEO for a blog focused on healthy lifestyles and habits, " +
                            "and considering the latest post titled '%%TITLE%%', generate a list of 5-10 SEO-rich keywords or phrases. " +
                            "These should be unique, likely to draw high-quality backlinks, and relevant to the content's focus on wellness. " +
                            "Present the keywords in JSON format with 'keyword' as the key and a relevance score from 0 to 100 as the value." +
                            "Here is the template you must use: {{\"keyword\", \"score\"}} \r\n.";
        public string ImageConceptsPrompt { get; set; } = $"Extract the core concepts or themes from the blog title: '%%TITLE%%'." +
                            "List these concepts, separated by commas, to guide the creation of a visually appealing image that reflects " +
                            "the blog's content.";
        public string ImagePrompt { get; set; } = $"Based on this identified key concept '%%IMAGE_CONCEPTS%%', create a short and simple prompt for an " +
                            "image that represents these nutritional advice themes." +
                            "Make sure to not violate the OPENAI content policy.";
        public int NumberOfAdvertisements { get; set; } = 4;
        public Guid CategoryId { get; set; }

        public void FinalizeSettings()
        {
            TitlePrompt += $"Generate a straightforward, single-sentence blog post title." +
                            " Please avoid dramatic phrases and do not separate the title with a colon. For example, do not format it like 'Unlocking a Leaner, Healthier You: How Strength Training Optimizes Weight Loss'.";
            IntroductionPrompt += "Please do not write the title of the blog in the beginning of the content. " +
                                  "Please generate content that closely mimics the style and tone of a professional human writer, " +
                                  "focusing on a natural and engaging narrative that seamlessly blends facts with conversational language." +
                                  " Focus on delivering information in a straightforward manner, avoiding any dramatic language." +
                                  " Use the Markdown Syntax for italic, bold keywords and external backlinks for keywords. " +
                                  "Do not write references or sources at the end. ";
            //"Use [b] and [/b] for bold text, [i] and [/i] for italic text. " +
            //"Please structure the introduction into short paragraphs. " +
            //"(YOU MUST MAKE SURE TO OPEN AND CLOSE THE TAGS PERFECTLY CAREFULLY!!)";

            ContentPrompt += "Start directly with the main body of the content, skipping any introductory elements. Immediately dive into discussing the key points and facts. " +
                             "Do not mention the blog post title at the start. " +
                             "Focus on a straightforward delivery of information, avoiding any dramatic language." +
                             " Use the Markdown Syntax to Organize the content with distinct subtitles, italic keywords, " +
                             "bold keywords and external backlinks for keywords." +
                             "Write each section with long and informative content.";
            //"Include bullet points for clarity, and use text formatting such as bold ('[b]' and '[/b]') and italic ('[i]' and '[/i]') to emphasize important aspects. " +
            //"Ensure all formatting tags are correctly opened and closed. " +

            CallToActionPrompt += "Start directly with the main body of the content, skipping any introductory elements. Immediately dive into discussing the key points and facts.";
        }
    }


}
