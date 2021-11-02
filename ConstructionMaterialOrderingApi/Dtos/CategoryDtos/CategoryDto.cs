using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionMaterialOrderingApi.Dtos.CategoryDtos
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public int HardwareStoreId { get; set; }
        public string CategoryName { get; set; }
    }
}
