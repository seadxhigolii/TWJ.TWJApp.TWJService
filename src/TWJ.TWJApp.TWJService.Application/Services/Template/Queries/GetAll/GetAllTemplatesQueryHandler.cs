using Asp.Nappox.School.Common.Extensions;
using MapperSegregator.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Helpers;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Queries.GetAll
{
    public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, IList<GetAllTemplatesModel>>
    {
        private readonly ITWJAppDbContext _context;
        public GetAllTemplatesQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IList<GetAllTemplatesModel>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var templates = await _context.Templates
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.DisplayText)
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
                return mappedTemplates;
            }
            catch (Exception e)
            {
                throw e;
            }
          
        }
    }
}
