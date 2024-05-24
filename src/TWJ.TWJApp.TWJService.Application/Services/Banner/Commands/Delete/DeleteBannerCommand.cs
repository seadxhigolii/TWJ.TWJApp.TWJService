using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Delete
{
    public class DeleteBannerCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}