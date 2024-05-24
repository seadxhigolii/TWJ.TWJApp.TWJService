﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.GenerateRandom
{
    public class GenerateRandomBlogPostCommand : IRequest<BlogPostResponse>
    {
    }
}