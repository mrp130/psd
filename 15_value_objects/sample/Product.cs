using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueObject.Sample
{
    public class Product
    {
        public Guid Id { get; }
        private string name { get; set;  }
        private Money price { get; set; }

        public Product(string name, Money price)
        {
            this.Id = Guid.NewGuid();
            this.name = name;
            this.price = price;
        }

        public override bool Equals(object obj)
        {
            var p = obj as Product;
            if (p == null) return false;

            return p.Id.Equals(this.Id);
        }
    }
}
