using MediatR;
using OpenQA.Selenium;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Add
{
    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, Response<bool>>
    {
        private readonly ITWJAppDbContext _context;

        public AddCategoryCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Response<bool>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.ProductCategories.AddAsync(request.AddCategory(), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var response = new Response<bool>
                {
                    Data = true,
                    Message = "Category added successfully.",
                    StatusCode = 200,
                    Success = true
                };
                return response;
            }
            catch (Exception ex)
            {

                var errorResponse = new Response<bool>
                {
                    Data = false,
                    Message = ex.Message,
                    StatusCode = 500,
                    Success = false
                };
                return errorResponse;
            }
        }
    }
}