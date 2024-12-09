using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Import
{
    public class ImportSEOKeywordCommand : IRequest<Unit>
    {
        public string FilePath { get; }

        public ImportSEOKeywordCommand(string filePath)
        {
            FilePath = filePath;
        }
    }
}
