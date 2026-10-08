using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTO
{
    public record OrderAddRequest(Guid userid , DateTime orderdate , List<OrderItemAddRequest> items)
    {
        public OrderAddRequest() : this(default, default, default) { }
    }
    
}
