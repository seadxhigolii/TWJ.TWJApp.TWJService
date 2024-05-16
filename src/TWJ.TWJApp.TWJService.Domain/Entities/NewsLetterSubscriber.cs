using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class NewsLetterSubscriber : BaseEntity<Guid>
    {
        public string Email { get; set; }
        public bool Subscribed { get; set; }
        public DateTime? DateUnsubscribed { get; set; }
    }
}
