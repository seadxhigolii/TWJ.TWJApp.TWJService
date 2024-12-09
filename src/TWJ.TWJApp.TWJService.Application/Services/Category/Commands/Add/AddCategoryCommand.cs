using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Models;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Add
{
    public class AddCategoryCommand : IRequest<Response<bool>>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.Category AddCategory()
        {
            return new TWJ.TWJApp.TWJService.Domain.Entities.Category
            {
                Name = Name,
                Description = Description
            };
        }
    }
}