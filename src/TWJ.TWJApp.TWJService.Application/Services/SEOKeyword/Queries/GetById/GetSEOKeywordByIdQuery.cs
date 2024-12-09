using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetById
{
    public class GetSEOKeywordByIdQuery : IRequest<GetSEOKeywordByIdModel>
    {
        public Guid Id { get; set; }
    }
}