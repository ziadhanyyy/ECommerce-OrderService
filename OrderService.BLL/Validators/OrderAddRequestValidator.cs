using FluentValidation;
using OrderService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Validators
{
    public class OrderAddRequestValidator : AbstractValidator<OrderAddRequest>
    {
        public OrderAddRequestValidator()
        {
            //UserID
            RuleFor(temp => temp.userid)
              .NotEmpty().WithErrorCode("User ID can't be blank");

            //OrderDate
            RuleFor(temp => temp.orderdate)
              .NotEmpty().WithErrorCode("Order Date can't be blank");

            //OrderItems
            RuleFor(temp => temp.items)
              .NotEmpty().WithErrorCode("Order Items can't be blank");
        }
    }
}
