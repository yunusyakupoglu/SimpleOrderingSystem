using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Customer : BaseEntity, IEntity
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
