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
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedDateByStore { get; set; }
        public int CreatedUserIdByStore { get; set; }
        public bool isDeletedByStore { get; set; }
        public bool StatusByStore { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public bool IsReady { get; set; }
    }
}
