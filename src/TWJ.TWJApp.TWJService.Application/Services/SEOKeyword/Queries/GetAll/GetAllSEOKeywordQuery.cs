using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetAll
{
    public class GetAllSEOKeywordQuery : IRequest<IList<GetAllSEOKeywordModel>>
    {
    }
}