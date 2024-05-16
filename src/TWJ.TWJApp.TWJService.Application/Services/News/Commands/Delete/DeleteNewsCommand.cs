using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Delete
{
    public class DeleteNewsCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}