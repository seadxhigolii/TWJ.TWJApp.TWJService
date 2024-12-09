using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Delete
{
    public class DeleteSEOKeywordCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}