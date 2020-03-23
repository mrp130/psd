using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ValueObject.Sample.UnitTest
{
    [TestClass]
    public class MoneyUnitTest
    {
        [TestMethod]
        public void CreateExpectFailed()
        {
            try
            {
                Money m = new Money("a", 60000);
                Money m = new Money("abcde", 60000);
                Assert.Fail("expect exception");
            } catch (Exception) {}
        }

        [TestMethod]
        public void CreateExpectSuccess()
        {
            try
            {
                Money m = new Money("IDR", 60000);
            }
            catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void Add()
        {
            Money m1 = new Money("IDR", 60000);
            Money m2 = new Money("IDR", 30000);
            Money m3 = new Money("IDR", 90000);
            if (!m1.Add(m2).Equals(m3))
            {
                Assert.Fail("expect equals");
            }
        }
    }
}
