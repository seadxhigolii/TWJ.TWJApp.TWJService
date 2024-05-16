using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetById;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByTagName
{
    public class GetBlogPostByTagNameQueryHandler : IRequestHandler<GetBlogPostByTagNameQuery, IList<GetBlogPostByTagNameModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;

        public GetBlogPostByTagNameQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IList<GetBlogPostByTagNameModel>> Handle(GetBlogPostByTagNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.BlogPosts.AsNoTracking().Where(x => x.Tags.Contains(request.Tag)).ToListAsync(cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("Record"));

                var blogPostList = new List<GetBlogPostByTagNameModel>();

                foreach (var blogPost in data)
                {
                    var bp = new GetBlogPostByTagNameModel();
                    bp.Id = blogPost.Id;
                    bp.Title = blogPost.Title;
                    bp.URL = blogPost.URL;
                    bp.Image = blogPost.Image;
                    bp.UserId = blogPost.UserId;
                    bp.CreatedAt = blogPost.CreatedAt;

                    blogPostList.Add(bp);
                }

                return blogPostList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
