using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTO
{
    public record ProductDto(Guid id, string? name, decimal price);


}
