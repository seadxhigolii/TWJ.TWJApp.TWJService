using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;


namespace TWJ.TWJApp.TWJService.Application.Services.Template.Queries.GetAll
{
    public class GetAllTemplatesModel : IProfile
    {
        public Guid Id { get; set; }
        public Guid TemplateSettingId { get; set; }
        public string DisplayText { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int? ParentId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; } 
        public string CreatedBy { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Template, GetAllTemplatesModel>(
                (src, options) =>
                {
                    return new GetAllTemplatesModel
                    {
                        Id = src.Id,
                        TemplateSettingId = src.TemplateSettingId,
                        DisplayText = src.DisplayText,
                        Description = src.Description,
                        IsActive = src.IsActive,
                        IsDefault = src.IsDefault,
                        ParentId = src.ParentId,
                        CreatedAt = src.CreatedAt,
                        UpdatedAt = src.UpdatedAt,
                        CreatedBy = src.CreatedBy
                    };
                });
        }
    }
}
