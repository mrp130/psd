using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueObject.Sample
{
    public class Money
    {
        private string currency;
        private float value;

        public Money(string currency, float value) 
        {
            if (currency.Length != 3)
                throw new Exception("currency must 3 char");

            this.currency = currency;
            this.value = value;
        }

        public Money Add(Money money)
        {
            if (this.currency != money.currency)
                throw new Exception("currency must be same");
            return new Money(this.currency, this.value + money.value);
        }

        public Money Add(float value)
        {
            return new Money(this.currency, this.value + value);
        }

        public Money Substract(float value)
        {
            return new Money(this.currency, this.value - value);
        }

        public override bool Equals(object obj)
        {
            var m = obj as Money;
            if (m == null) return false; //jika obj bukan bertipe Money, langsung false

            //jika currency tidak sama, langsung false
            if (this.currency != m.currency) return false;

            //jika value tidak sama, langsung false
            if (this.value != m.value) return false;

            //jika semua field member sama, return true
            return true;
        }
    }
}
