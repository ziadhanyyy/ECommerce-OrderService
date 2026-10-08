using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Data.Entities
{
    public class Order
    {
       [BsonId]
       [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid _id { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid UserId { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public  DateTime OrderDate { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public decimal Total {  get; set; } 
      public required IEnumerable<OrderItem> Items { get; set; }

    }
}
