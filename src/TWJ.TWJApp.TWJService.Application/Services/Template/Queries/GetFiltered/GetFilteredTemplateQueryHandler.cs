using Asp.Nappox.School.Common.Extensions;
using MapperSegregator.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetFiltered
{
    public class GetFilteredTemplateQueryHandler : IRequestHandler<GetFilteredTemplateQuery, FilterResponse<GetFilteredTemplateModel>>
    {
        private readonly ITWJAppDbContext _context;
        public GetFilteredTemplateQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<FilterResponse<GetFilteredTemplateModel>> Handle(GetFilteredTemplateQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Templates
                            .Skip((request.Page - 1) * request.PageSize)
                            .Take(request.PageSize);

                var totalItems = _context.Templates.AsNoTracking().ToListAsync().Result.Count();

                var mappedData = query.Select(x => new GetFilteredTemplateModel
                {
                    Id = x.Id,
                    DisplayText = x.DisplayText,
                    Description = x.Description,
                    isDefault = x.IsDefault, 
                    isActive = x.IsActive, 
                    TemplateSettingId = x.TemplateSettingId
                }).ToListAsync();

                return new FilterResponse<GetFilteredTemplateModel>
                {
                    Data = await mappedData,
                    TotalPages = (int)Math.Ceiling((double)totalItems / request.PageSize),
                    TotalItems = totalItems
                };
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace();
                var stackFrame = stackTrace.GetFrame(0);
                Log log = new Log()
                {
                    Class = this.GetType().Name,
                    Method = stackFrame.GetMethod().ToString(),
                    Message = ex.Message,
                    CreatedAt = DateTime.Now
                };
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

    }
}
