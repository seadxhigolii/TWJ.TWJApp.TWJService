using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Add;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Import
{
    public class ImportProductCommandHandler : IRequestHandler<ImportProductCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public ImportProductCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(ImportProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(request.FilePath, cancellationToken);
                var importedProducts = JsonSerializer.Deserialize<List<TWJ.TWJApp.TWJService.Domain.Entities.Product>>(jsonString);

                if (importedProducts != null)
                {
                    foreach (var importedProduct in importedProducts)
                    {
                        var existingProduct = await _context.Products
                            .FirstOrDefaultAsync(p => p.VendorName == importedProduct.VendorName, cancellationToken);

                        if (existingProduct != null)
                        {
                            existingProduct.AvgRating = importedProduct.AvgRating;
                            existingProduct.Price = importedProduct.Price;
                            _context.Products.Update(existingProduct);
                        }
                        else
                        {
                            await _context.Products.AddAsync(importedProduct);
                        }
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
