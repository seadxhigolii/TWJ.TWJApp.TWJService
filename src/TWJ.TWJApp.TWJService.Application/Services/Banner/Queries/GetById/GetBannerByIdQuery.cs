using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetById
{
    public class GetBannerByIdQuery : IRequest<GetBannerByIdModel>
    {
        public Guid Id { get; set; }
    }
}