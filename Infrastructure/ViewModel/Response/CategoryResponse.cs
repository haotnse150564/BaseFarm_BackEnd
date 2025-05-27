using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class CategoryResponse
    {
        public class CategoryView
        {
            public long CategoryId { get; set; }
            public string? CategoryName { get; set; }

        }
        
    }
}
