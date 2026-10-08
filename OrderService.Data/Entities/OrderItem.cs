using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Data.Entities
{
    public class OrderItem
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid _id { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid ProductId { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public decimal UnitPrice { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public decimal TotalPrice { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public int Quantity { get; set; }
    }
}
