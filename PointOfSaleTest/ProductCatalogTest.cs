using System;
using System.Collections.Generic;
using NUnit.Framework;
using PointOfSale;

namespace PointOfSaleTest
{
    public class ProductCatalogTest
    {
        #region Setups
        private static ProductCatalog SetupSimpleProductCatalog()
        {
            return new ProductCatalog
            {
                Products = new List<Product>
                {
                    new Product("123", 1.0m),
                    new Product("ABC", 2.0m),
                    new Product("DEF", 3.0m),
                    new Product("GHI", 4.0m),
                    new Product("JKL", 5.0m),
                }
            };
        }
        #endregion

        #region FindProduct
        [Test]
        public void FindProductShouldBeCaseSensitive()
        {
            var result = SetupSimpleProductCatalog().FindProduct("abc");

            Assert.That(result, Is.Null, "Product ID is case sensitive, no result should be returned.");
        }

        [Test]
        public void FindProductShouldReturnSingleMatch()
        {
            var result = SetupSimpleProductCatalog().FindProduct("ABC");

            Assert.That(result, Is.Not.Null, "Matching product should be returned.");
            Assert.That(result.Id, Is.EqualTo("ABC"));
        }

        [Test]
        public void FindProductShouldTrimIdParameter()
        {
            var result = SetupSimpleProductCatalog().FindProduct("ABC ");

            Assert.That(result, Is.Not.Null, "Matching product should be returned.");
            Assert.That(result.Id, Is.EqualTo("ABC"));
        }

        [Test]
        public void FindProductWithEmptyIdParamterShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => SetupSimpleProductCatalog().FindProduct(""));
        }

        [Test]
        public void FindProductWithNullIdParamterShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => SetupSimpleProductCatalog().FindProduct(null));
        }
        #endregion

        #region RemoveDuplicates
        [Test]
        public void RemoveDuplicatesShouldRemoveSingleProductDuplicate()
        {
            var catalog = SetupSimpleProductCatalog();
            catalog.Products.Add(new Product("ABC", 100.23m));

            var countBeforeRemoval = catalog.Products.Count;

            catalog.RemoveDuplicates();

            Assert.That(catalog.Products.Count, Is.EqualTo(countBeforeRemoval - 1),
                "Expect that a single ABC duplicate is removed.");

            var originalProduct = catalog.FindProduct("ABC");

            Assert.That(originalProduct, Is.Not.Null, "Expect that a single ABC remains in catalog.");
            Assert.That(originalProduct.Price, Is.EqualTo(2.0m),
                "Expect that the first ABC instance remains in catalog.");
        }

        [Test]
        public void RemoveDuplicatesShouldRemoveMultipleProductDuplicates()
        {
            var catalog = SetupSimpleProductCatalog();
            catalog.Products.Add(new Product("ABC", 100.23m));
            catalog.Products.Add(new Product("DEF", 100.23m));

            var countBeforeRemoval = catalog.Products.Count;

            catalog.RemoveDuplicates();

            Assert.That(catalog.Products.Count, Is.EqualTo(countBeforeRemoval - 2),
                "Expect that ABC and DEF duplicates are removed.");

            var originalProduct = catalog.FindProduct("ABC");

            Assert.That(originalProduct, Is.Not.Null, "Expect that a single ABC remains in catalog.");
            Assert.That(originalProduct.Price, Is.EqualTo(2.0m),
                "Expect that the first ABC instance remains in catalog.");

            originalProduct = catalog.FindProduct("DEF");

            Assert.That(originalProduct, Is.Not.Null, "Expect that a single DEF remains in catalog.");
            Assert.That(originalProduct.Price, Is.EqualTo(3.0m),
                "Expect that the first DEF instance remains in catalog.");
        }
        #endregion
    }
}
