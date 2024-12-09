using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Delete
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}