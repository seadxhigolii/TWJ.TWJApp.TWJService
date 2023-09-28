using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Delete
{
    public class DeleteBaseCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
