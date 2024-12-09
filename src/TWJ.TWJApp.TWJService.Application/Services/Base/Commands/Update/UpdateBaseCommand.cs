using MediatR;
using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Update
{
    public class UpdateBaseCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Property { get; set; }

        public BaseModel Update(BaseModel baseModel)
        {
            baseModel.Property = Property;
            return baseModel;
        }
    }
}
