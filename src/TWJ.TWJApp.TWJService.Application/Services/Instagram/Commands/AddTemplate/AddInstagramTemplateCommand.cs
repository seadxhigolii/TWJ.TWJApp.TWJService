using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.AddTemplate
{
    public class AddInstagramTemplateCommand : IRequest<Unit>
    {
        public string Image { get; set; }
        public int Type { get; set; }
        public bool IsVideo { get; set; }


        public Domain.Entities.InstagramPost AddInstagramTemplate()
        {
            return new Domain.Entities.InstagramPost
            {
                Image = Image,
                Type = Type,
                IsVideo = IsVideo
            };
        }
    }
}
