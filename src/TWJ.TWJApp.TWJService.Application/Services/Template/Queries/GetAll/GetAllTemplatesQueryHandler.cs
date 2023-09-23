using Asp.Nappox.School.Common.Extensions;
using MapperSegregator.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Helpers;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Queries.GetAll
{
    public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, FilterResponse<GetAllTemplatesModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IConfiguration _configuration;
        public GetAllTemplatesQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<FilterResponse<GetAllTemplatesModel>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Page <= 0) request.Page = 1;

                var templates = await _context.Template
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.DisplayText)
                                    .Skip((request.Page - 1) * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToListAsync(cancellationToken);

                var mappedTemplates = templates.Select(t => new GetAllTemplatesModel
                {
                    Id = t.Id,
                    TemplateSettingId = t.TemplateSettingId,
                    DisplayText = t.DisplayText,
                    Description = t.Description,
                    IsActive = t.IsActive,
                    IsDefault = t.IsDefault,
                    ParentId = t.ParentId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy
                }).ToList();

                var totalItems = await _context.Template.CountAsync(cancellationToken);
                var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

                return new FilterResponse<GetAllTemplatesModel>
                {
                    Data = mappedTemplates,
                    TotalPages = totalPages,
                    TotalItems = totalItems
                };

            }
            catch (Exception e)
            {
                throw e;
            }
          
        }
    }
}
