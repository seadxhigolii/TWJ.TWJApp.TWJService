using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class SEOKeyword : BaseEntity<Guid>
    {
        #region Properites
        public string Keyword { get; set; }
        public int SearchVolume { get; set; }
        public int CompetitionLevel { get; set; }
        public int ClickThroughRate { get; set; }
        #endregion

        #region Entity Models
        public ICollection<BlogPostSEOKeyword> BlogPostSEOKeywords { get; set; }
        #endregion
    }
}
