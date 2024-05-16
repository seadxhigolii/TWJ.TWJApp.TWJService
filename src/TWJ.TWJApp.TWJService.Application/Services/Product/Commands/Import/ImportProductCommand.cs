using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Import
{
    public class ImportProductCommand : IRequest<Unit>
    {
        public string FilePath { get; }

        public ImportProductCommand(string filePath)
        {
            FilePath = filePath;
        }
    }
}
