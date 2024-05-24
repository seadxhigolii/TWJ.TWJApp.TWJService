using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Update
{
    public class UpdateBannerCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        //public string Property { get; set; }

        public Domain.Entities.Banner Update(Domain.Entities.Banner banner)
        {
            //banner.Property = Property;
            return banner;
        }
    }
}