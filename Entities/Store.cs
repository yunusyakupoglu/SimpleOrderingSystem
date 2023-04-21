using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Store : BaseEntity, IEntity
    {
        public int ProductId { get; set; }
        public int Stock { get; set; }
        public bool isReady { get; set; }
    }
}
