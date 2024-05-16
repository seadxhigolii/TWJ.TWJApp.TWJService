using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetById
{
    public class GetCategoryByIdQuery : IRequest<GetCategoryByIdModel>
    {
        public Guid Id { get; set; }
    }
}