using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByUrl;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using static Google.Rpc.Context.AttributeContext.Types;
using TWJ.TWJApp.TWJService.Domain.Entities;
using Google.Type;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetRelated
{
    public class GetRelatedBlogPostQueryHandler : IRequestHandler<GetRelatedBlogPostQuery, IList<GetRelatedBlogPostModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;

        public GetRelatedBlogPostQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IList<GetRelatedBlogPostModel>> Handle(GetRelatedBlogPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var blogPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(x => x.URL == request.URL, cancellationToken);

                var relatedBlogPostList = await _context.BlogPosts.Where(x=>x.BlogPostCategoryId == blogPost.BlogPostCategoryId && x.Id != blogPost.Id).Take(3).ToListAsync();
                
                if (relatedBlogPostList == null) throw new BadRequestException(ValidatorMessages.NotFound("Record"));

                var relatedBlogPostListModel = new List<GetRelatedBlogPostModel>();

                foreach (var data in relatedBlogPostList)
                {
                    var author = await _context.User.FindAsync(data.UserId);
                    if (author != null)
                    {
                        var bP = new GetRelatedBlogPostModel
                        {
                            Id = data.Id,
                            Title = data.Title,
                            URL = data.URL,
                            UserId = data.UserId,
                            CreatedAt = data.CreatedAt,
                            ProductCategoryId = data.ProductCategoryId,
                            Image = data.Image,
                            Views = data.Views,
                            Content = data.Content,
                            Tags = data.Tags,
                            AuthorImage = author.Image,
                            AuthorName = author.FirstName + " " + author.LastName,
                        };

                        relatedBlogPostListModel.Add(bP);
                    }
                }
                
                return relatedBlogPostListModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
