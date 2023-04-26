using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class ProductWithStoreDto : IDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public bool isDeleted { get; set; }
        public bool Status { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public bool IsReady { get; set; }
    }
}
