using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetFiltered
{
    public class GetFilteredTemplateModel : IProfile
    {
        public Guid Id { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public bool isDefault { get; set; }
        public bool isActive { get; set; }
        public Guid TemplateSettingId { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Template, GetFilteredTemplateModel>(
                (src, options) =>
                {
                    return new GetFilteredTemplateModel
                    {
                        Id = src.Id,
                        Description = src.Description,
                        DisplayText = src.DisplayText,
                        isDefault = src.IsDefault,
                        isActive = src.IsActive,
                        TemplateSettingId = src.TemplateSettingId
                    };
                });
        }
    }
}
