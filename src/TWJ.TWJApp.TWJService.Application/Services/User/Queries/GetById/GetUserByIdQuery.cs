using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetById;

namespace TWJ.TWJApp.TWJService.Application.Services.User.Queries.GetById
{
    public class GetUserByIdQuery : IRequest<GetUserByIdModel>
    {
        public Guid Id { get; set; }
    }
}
