using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Dto.Models
{
    public class KeywordScoreWrapper
    {
        public List<KeywordScore> Keywords { get; set; }
    }
    public class KeywordScore
    {
        public string Keyword { get; set; }
        public double Score { get; set; }
    }

}
