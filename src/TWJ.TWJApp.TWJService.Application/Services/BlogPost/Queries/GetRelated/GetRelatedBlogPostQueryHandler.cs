using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

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
                var blogPost = await _context.BlogPosts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.URL == request.URL && x.Published == true, cancellationToken);

                if (blogPost == null) return new List<GetRelatedBlogPostModel>();

                var tagIds = await _context.BlogPostTags
                    .Where(bpt => bpt.BlogPostID == blogPost.Id)
                    .Select(bpt => bpt.TagID)
                    .ToListAsync(cancellationToken);

                var relatedBlogPostList = await _context.BlogPosts
                    .Where(bp => bp.Id != blogPost.Id && bp.Published == true)
                    .Where(bp => _context.BlogPostTags
                        .Where(bpt => tagIds.Contains(bpt.TagID))
                        .Select(bpt => bpt.BlogPostID)
                        .Distinct()
                        .Contains(bp.Id))
                    .OrderByDescending(bp => _context.BlogPostTags
                        .Count(bpt => bpt.BlogPostID == bp.Id && tagIds.Contains(bpt.TagID)))
                    .Take(3)
                    .ToListAsync(cancellationToken);

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
