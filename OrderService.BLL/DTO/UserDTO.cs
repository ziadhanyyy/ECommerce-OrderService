using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTO
{
    public record UserDTO(Guid id, string Name, string Email);
}
