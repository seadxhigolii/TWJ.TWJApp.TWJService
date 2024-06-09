using MediatR;
using System;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByAuthorFiltered
{
    public class GetByAuthorFilteredQuery : FilterRequest, IRequest<FilterResponse<GetByAuthorFilteredModel>>
    {
        public string AuthorName { get; set; }
    }
}
