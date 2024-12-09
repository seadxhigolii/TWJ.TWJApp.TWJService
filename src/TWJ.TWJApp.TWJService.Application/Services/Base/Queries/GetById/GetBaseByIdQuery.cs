using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetById
{
    public class GetBaseByIdQuery : IRequest<GetBaseByIdModel>
    {
        public Guid Id { get; set; }
    }
}
