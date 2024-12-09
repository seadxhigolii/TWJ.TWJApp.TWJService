using MediatR;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add
{
    public class AddBaseCommand : IRequest<Unit>
    {
        public string Property { get; set; }

        public BaseModel AddBase()
        {
            return new BaseModel
            {
                Property = Property
            };
        }
    }
}
