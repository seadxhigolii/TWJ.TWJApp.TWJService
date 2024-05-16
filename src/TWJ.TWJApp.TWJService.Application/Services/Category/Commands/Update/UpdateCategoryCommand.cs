using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Update
{
    public class UpdateCategoryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.Category Update(TWJ.TWJApp.TWJService.Domain.Entities.Category category)
        {
            category.Name = Name;
            category.Description = Description;
            return category;
        }
    }
}