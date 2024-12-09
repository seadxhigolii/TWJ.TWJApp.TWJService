using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetById
{
    public class GetProductByIdQuery : IRequest<GetProductByIdModel>
    {
        public Guid Id { get; set; }
    }
}