using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PointOfSale;
using PointOfSale.Repository;
using System;
using System.Collections.Generic;
using System.IO;

namespace PointOfSaleTest.Repository
{
    public class PointOfSaleFileReaderTest
    {
        #region Setups
        /// <summary>
        /// Sets up a mock that returns the serialized JSON of the given <see cref="products"/>.
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private static Mock<IFileManager> SetupFileManagerWithProductCatalog(List<Product> products)
        {
            return
                SetupFileManagerWithJson(
                    JsonConvert.SerializeObject(new ProductCatalog {Products = products}));
        }

        /// <summary>
        /// Sets up a mock that returns the serialized JSON of the given <see cref="discounts"/>.
        /// </summary>
        /// <param name="discounts"></param>
        /// <returns></returns>
        private static Mock<IFileManager> SetupFileManagerWithLineItemDiscounts(List<SalesLineItemDiscount> discounts)
        {
            return
                SetupFileManagerWithJson(
                    JsonConvert.SerializeObject(new PromotionCatalog() {SalesLineItemDiscounts = discounts}));
        }
        
        /// <summary>
        /// Sets up a mock that returns the given JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static Mock<IFileManager> SetupFileManagerWithJson(string json)
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Returns(json);
            return mockFileManager;
        }
        #endregion

        #region ReadProductCatalog
        [Test]
        public void ReadProductCatalogShouldDeserializeMultipleProducts()
        {
            var products = new List<Product>()
            {
                new Product("Macintosh Apple", 1.0m),
                new Product("Delicious Apple", 1.23m),
            };

            var mockFileManager = SetupFileManagerWithProductCatalog(products);

            var result = new PointOfSaleFileReader(mockFileManager.Object)
                .ReadProductCatalog("mock.json");

            Assert.That(result.Products, Is.Not.Null);
            Assert.That(result.Products.Count, Is.EqualTo(products.Count));

            // Verify deserialization of JSON
            Assert.That(result.Products[0].Id, Is.EqualTo(products[0].Id));
            Assert.That(result.Products[0].Price, Is.EqualTo(products[0].Price));

            Assert.That(result.Products[1].Id, Is.EqualTo(products[1].Id));
            Assert.That(result.Products[1].Price, Is.EqualTo(products[1].Price));

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogWithMissingProductIdShouldThrowException()
        {
            var mockFileManager = SetupFileManagerWithJson(@"{""Products"": [{ ""Price"": 1.0 }]}");

            Assert.Throws<MissingProductFieldException>(
                () => new PointOfSaleFileReader(mockFileManager.Object).ReadProductCatalog("mock.json"),
                "Product catalog read should fail when any product is missing an 'ID' field.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogWithMissingProductPriceShouldThrowException()
        {
            var mockFileManager = SetupFileManagerWithJson(@"{""Products"": [{ ""Id"": ""Product A"" }]}");

            Assert.Throws<MissingProductFieldException>(
                () => new PointOfSaleFileReader(mockFileManager.Object).ReadProductCatalog("mock.json"),
                "Product catalog read should fail when any product is missing a 'Price' field.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogWithNegativeProductPriceShouldThrowException()
        {
            var mockFileManager =
                SetupFileManagerWithJson(@"{""Products"": [{ ""Id"": ""Product A"", ""Price"": -1.0 }]}");

            Assert.Throws<InvalidProductValueException>(
                () => new PointOfSaleFileReader(mockFileManager.Object).ReadProductCatalog("mock.json"),
                "Product catalog read should fail when any product has an invalid 'Price' field.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogWithZeroProductPriceShouldThrowException()
        {
            var mockFileManager =
                SetupFileManagerWithJson(@"{""Products"": [{ ""Id"": ""Product A"", ""Price"": 0.0 }]}");

            Assert.Throws<InvalidProductValueException>(
                () => new PointOfSaleFileReader(mockFileManager.Object).ReadProductCatalog("mock.json"),
                "Product catalog read should fail when any product has an invalid 'Price' field.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogWithInvalidValueShouldThrowException()
        {
            var mockFileManager =
                SetupFileManagerWithJson(@"{""Products"": [{ ""Id"": ""Product A"", ""Price"": not_decimal }]}");

            Assert.Throws<ProductCatalogFileException>(
                () => new PointOfSaleFileReader(mockFileManager.Object).ReadProductCatalog("mock.json"),
                "Product catalog read should fail if field value cannot be typed.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogWithInvalidJsonShouldThrowException()
        {
            var mockFileManager = SetupFileManagerWithJson(@"{""Products"": [{ ""Id"": ""Product A"", ""Price"": 1.0 }}");
                // missing ] bracket

            Assert.Throws<ProductCatalogFileException>(
                () => new PointOfSaleFileReader(mockFileManager.Object).ReadProductCatalog("mock.json"),
                "Product catalog read should fail if JSON string is invalid.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogShouldIgnoreDuplicateProducts()
        {
            var products = new List<Product>()
            {
                new Product("Macintosh Apple", 1.0m),
                new Product("Delicious Apple", 2.0m),
                new Product("Macintosh Apple", 3.0m),
                new Product("Delicious Apple", 4.0m),
            };

            var mockFileManager = SetupFileManagerWithProductCatalog(products);

            var result = new PointOfSaleFileReader(mockFileManager.Object).ReadProductCatalog("mock.json");

            Assert.That(result.Products, Is.Not.Null);
            Assert.That(result.Products.Count, Is.EqualTo(2),
                "Duplicate products should be ignored, only the first instance is used.");
            Assert.That(result.Products[0].Price, Is.EqualTo(products[0].Price));
            Assert.That(result.Products[1].Price, Is.EqualTo(products[1].Price));

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogShouldTrimProductId()
        {
            var products = new List<Product>()
            {
                new Product(" Macintosh Apple ", 1.0m)
            };

            var mockFileManager = SetupFileManagerWithProductCatalog(products);

            var result = new PointOfSaleFileReader(mockFileManager.Object)
                .ReadProductCatalog("mock.json");

            Assert.That(result.Products, Is.Not.Null);
            Assert.That(result.Products.Count, Is.EqualTo(1));
            Assert.That(result.Products[0].Id, Is.EqualTo("Macintosh Apple"),
                "File reader should trim the product Id field.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadProductCatalogFileDoesNotExistShouldThrowException()
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Throws(new FileNotFoundException());

            Assert.Throws<ProductCatalogFileException>(() => new PointOfSaleFileReader(mockFileManager.Object)
                .ReadProductCatalog("mock.json"));
        }

        [Test]
        public void ReadProductCatalogFileDoesNotHaveAccessShouldThrowException()
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Throws(new UnauthorizedAccessException());

            Assert.Throws<ProductCatalogFileException>(() => new PointOfSaleFileReader(mockFileManager.Object)
                .ReadProductCatalog("mock.json"));
        }
        #endregion

        #region ReadPromotionCatalog

        [Test]
        public void ReadPromotionCatalogShouldDeserializeMultipleProducts()
        {
            var discounts = new List<SalesLineItemDiscount>()
            {
                new SalesLineItemDiscount()
                {
                    Discount = "10%",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "Granola",
                    ThresholdQuantity = 3
                },
                new SalesLineItemDiscount()
                {
                    Discount = "$10",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "Greek Salad",
                    ThresholdQuantity = 5
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(discounts);

            var result = new PointOfSaleFileReader(mockFileManager.Object)
                .ReadPromotionCatalog("mock.json");

            Assert.That(result.SalesLineItemDiscounts, Is.Not.Null);
            Assert.That(result.SalesLineItemDiscounts.Count, Is.EqualTo(discounts.Count));

            // Verify deserialization of JSON
            Assert.That(result.SalesLineItemDiscounts[0].Discount, Is.EqualTo(discounts[0].Discount));
            Assert.That(result.SalesLineItemDiscounts[0].EffectiveFrom, Is.EqualTo(discounts[0].EffectiveFrom));
            Assert.That(result.SalesLineItemDiscounts[0].EffectiveTo, Is.EqualTo(discounts[0].EffectiveTo));
            Assert.That(result.SalesLineItemDiscounts[0].ProductId, Is.EqualTo(discounts[0].ProductId));
            Assert.That(result.SalesLineItemDiscounts[0].ThresholdQuantity, Is.EqualTo(discounts[0].ThresholdQuantity));

            mockFileManager.Verify();
        }

        [Test]
        public void ReadPromotionCatalogWithMissingRequiredSalesLineItemDiscountFieldShouldThrowException(
            [Values("Discount", "EffectiveFrom", "EffectiveFrom", "EffectiveTo", "ThresholdQuantity", "ProductId")] string field)
        {
            var fields = new List<string>
            {
                "\"Discount\": \"10%\"",
                "\"EffectiveFrom\": \"2016-06-01 05:00\"",
                "\"EffectiveTo\": \"2016-11-30 12:00\"",
                "\"ThresholdQuantity\": 3",
                "\"ProductId\": \"Macintosh Apple\""
            };

            fields.RemoveAll(l => l.StartsWith($"\"{field}"));

            var mockFileManager =
                SetupFileManagerWithJson(@"{""SalesLineItemDiscounts"": [{" + string.Join(", ", fields) + "}]}");

            Assert.Throws<MissingSalesLineItemDiscountFieldException>(
                () => new PointOfSaleFileReader(mockFileManager.Object).ReadPromotionCatalog("mock.json"),
                $"Promotions read should fail when any sales line item discount is missing a '{field}' field.");

            mockFileManager.Verify();
        }

        [Test]
        public void ReadPromotionCatalogShouldRemoveInvalidSalesLineItemDiscounts()
        {
            var invalidDiscounts = new List<SalesLineItemDiscount>()
            {
                new SalesLineItemDiscount()
                {
                    Discount = "-10%", // cannot have negative discounts
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "Granola",
                    ThresholdQuantity = 3
                },
                new SalesLineItemDiscount()
                {
                    Discount = "$10",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "Greek Salad",
                    ThresholdQuantity = 0 // cannot have 0 thresholds
                },
                new SalesLineItemDiscount()
                {
                    Discount = "$10",
                    EffectiveFrom = DateTime.Now.AddHours(1),
                    EffectiveTo = DateTime.Now.AddHours(-1), // Cannot have To < From
                    ProductId = "Tuna Salad",
                    ThresholdQuantity = 1 
                },
                new SalesLineItemDiscount()
                {
                    Discount = "$10",
                    EffectiveFrom = DateTime.Now.AddHours(1),
                    EffectiveTo = DateTime.Now.AddHours(-1), // Cannot have To < From
                    ProductId = "", // cannot have empty product ids
                    ThresholdQuantity = 1
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(invalidDiscounts);

            var result = new PointOfSaleFileReader(mockFileManager.Object)
                .ReadPromotionCatalog("mock.json");

            foreach (var d in invalidDiscounts)
                Assert.That(d.IsValid(), Is.False, "Expected discount to be invalid.");

            Assert.That(result.SalesLineItemDiscounts.Count, Is.EqualTo(0),
                "All invalid discounts should have been filtered out.");
        }

        [Test]
        public void ReadPromotionCatalogFileDoesNotExistShouldThrowException()
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Throws(new FileNotFoundException());

            Assert.Throws<PromotionCatalogFileException>(() => new PointOfSaleFileReader(mockFileManager.Object)
                .ReadPromotionCatalog("mock.json"));
        }

        [Test]
        public void ReadPromotionCatalogFileDoesNotHaveAccessShouldThrowException()
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Throws(new UnauthorizedAccessException());

            Assert.Throws<PromotionCatalogFileException>(() => new PointOfSaleFileReader(mockFileManager.Object)
                .ReadPromotionCatalog("mock.json"));
        }
        #endregion
    }
}
