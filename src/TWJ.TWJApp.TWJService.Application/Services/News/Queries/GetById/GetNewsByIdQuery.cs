using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetById
{
    public class GetNewsByIdQuery : IRequest<GetNewsByIdModel>
    {
        public Guid Id { get; set; }
    }
}