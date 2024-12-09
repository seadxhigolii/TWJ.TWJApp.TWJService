using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetAll;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetById;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByUrl
{
    public class GetBlogPostByUrlQueryHandler : IRequestHandler<GetBlogPostByUrlQuery, GetBlogPostByUrlModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;

        public GetBlogPostByUrlQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetBlogPostByUrlModel> Handle(GetBlogPostByUrlQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(x => x.URL == request.URL && x.Published == true, cancellationToken);

                if (data == null) throw new NotFoundException(ValidatorMessages.NotFound("Record"));

                var author = await _context.User.FindAsync(data.UserId);

                var blogPost = new GetBlogPostByUrlModel
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
                    AuthorName = author.FirstName + " " +author.LastName,
                };

                return blogPost;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
