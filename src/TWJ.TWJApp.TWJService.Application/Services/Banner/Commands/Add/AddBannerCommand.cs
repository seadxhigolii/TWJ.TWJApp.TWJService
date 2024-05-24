using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Add
{
    public class AddBannerCommand : IRequest<Unit>
    {
        public string AdvertiserName { get; set; }
        public Guid? ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string DestinationUrl { get; set; }
        public string Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Domain.Entities.Banner AddBanner()
        {
            return new Domain.Entities.Banner
            {
                AdvertiserName = AdvertiserName,
                ProductId = ProductId,
                ImageUrl = ImageUrl,
                DestinationUrl = DestinationUrl,
                Position = Position,
                Width = Width,
                Height = Height
            };
        }
    }
}