using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Delete
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}