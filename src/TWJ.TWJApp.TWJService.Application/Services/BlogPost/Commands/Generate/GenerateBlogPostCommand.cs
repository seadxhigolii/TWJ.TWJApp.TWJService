using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Generate
{
    public enum BlogPostLength
    {
        Short,
        Medium,
        Long
    }

    public class GenerateBlogPostCommand : IRequest<string>
    {
        public string Topic { get; set; }
        public string Tone { get; set; }
        public string TargetAudience { get; set; }
        public double CreativityLevel { get; set; }
        public Guid BlogPostCategoryId { get; set; }
        public Guid ProductId { get; set; }
        public List<string> Keywords { get; set; }
        public BlogPostLength LengthPreference { get; set; }
        public string ProductName { get; set; }
        public string AffiliateLink { get; set; }

        public GenerateBlogPostCommand()
        {
            Keywords = new List<string>();
        }
    }

}
