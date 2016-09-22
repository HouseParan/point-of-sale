using NUnit.Framework;
using PointOfSale;

namespace PointOfSaleTest
{
    public class ProductTest
    {
        [Test]
        public void EqualsShouldBeOverridenToCompareIds()
        {
            var p1 = new Product("Product A", 1.0m);
            var p2 = new Product("Product A", 2.0m);

            var result = p1.Equals(p2);

            Assert.That(result, Is.True, "Products should be equal when they share the exact ID.");
        }

        [Test]
        public void EqualsShouldBeCaseSensitiveOnId()
        {
            var p1 = new Product("PRODUCT A", 1.0m);
            var p2 = new Product("product a", 1.0m);

            var result = p1.Equals(p2);

            Assert.That(result, Is.False, "Product IDs should be case sensitive.");
        }
    }
}
