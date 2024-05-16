using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetById
{
    public class GetBlogPostCategoryByIdQueryHandler : IRequestHandler<GetBlogPostCategoryByIdQuery, GetBlogPostCategoryByIdModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";
        
        public GetBlogPostCategoryByIdQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<GetBlogPostCategoryByIdModel> Handle(GetBlogPostCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.BlogPostCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("BlogPostCategory"));

                return await _mapper.MapAsync<Domain.Entities.BlogPostCategory, GetBlogPostCategoryByIdModel>(data);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}