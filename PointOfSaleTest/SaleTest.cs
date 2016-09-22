using NUnit.Framework;
using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json;
using PointOfSale;
using PointOfSale.Pricing;
using PointOfSale.Repository;

namespace PointOfSaleTest
{
    public class SaleTest
    {
        #region Setups
        /// <summary>
        /// Gets the minimum Mock required so that <see cref="Sale"/> can properly call <see cref="PricingStrategyFactory"/>.
        /// </summary>
        /// <returns></returns>
        private static Mock<IFileManager> SetupFileManagerWithNoPromotions()
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(new PromotionCatalog { OverallStrategy = "BestForCustomer" }));
            return mockFileManager;
        }

        /// <summary>
        /// Gets a Mock with the given line item discounts so that <see cref="Sale"/> can properly call <see cref="PricingStrategyFactory"/>.
        /// </summary>
        /// <returns></returns>
        private static Mock<IFileManager> SetupFileManagerWithLineItemDiscounts(List<SalesLineItemDiscount> discounts)
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(new PromotionCatalog() {SalesLineItemDiscounts = discounts}));
            return mockFileManager;
        }
        #endregion

        #region MakeLineItem
        [Test]
        public void MakeLineItemIdenticalProductUpdatesQuantity()
        {
            var sale = new Sale(SetupFileManagerWithNoPromotions().Object);            
            sale.AddLineItem(new SalesLineItem(new Product("Apple", 0.50m)));

            // Act
            sale.MakeLineItem(new Product("Apple", 0.50m), 1); // add same line item

            Assert.That(sale.LineItems.Count, Is.EqualTo(1),
                "Adding an identical line item should update the quantity of the existing line item.");
            Assert.That(sale.LineItems[0].Quantity, Is.EqualTo(2));
        }

        [Test]
        public void MakeLineItemIdenticalProductUpdatesQuantityByMultiples()
        {
            var sale = new Sale(SetupFileManagerWithNoPromotions().Object);
            sale.AddLineItem(new SalesLineItem(new Product("Oranges", 0.2m)));

            // Act
            sale.MakeLineItem(new Product("Oranges", 0.2m), quantity:2); // add same line item

            Assert.That(sale.LineItems.Count, Is.EqualTo(1),
                "Adding identical line item should update the quantity of the existing line item.");
            Assert.That(sale.LineItems[0].Quantity, Is.EqualTo(3));
        }
        #endregion

        #region GetTotal 

        // Only one dependency (Promotions file) on these tests right now.
        // Could get a lot more complicated very quickly.
        // Would need to make ISale interface and the mock the Sale class for testing pricing strategies directly.
        
        [Test]
        public void GetTotalAppliesSimpleItemQuantityDiscount()
        {
            var discounts = new List<SalesLineItemDiscount>
            {
                // Setup "Buy 3 get $1.00 off" discount.
                new SalesLineItemDiscount()
                {
                    Discount = "$1.00",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "B",
                    ThresholdQuantity = 3
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(discounts);

            var sale = new Sale(mockFileManager.Object);
            sale.MakeLineItem(new Product("B", price: 1.0m), quantity: 3); // Buy 3 @ $1/each = $3 regular price

            var result = sale.GetTotal();

            Assert.That(result, Is.EqualTo(2.0m),
                "Expect that the 'Buy 3 for $1 off' quantity discount is applied.");
        }

        [Test]
        public void GetTotalAppliesLargerLineItemQuantityDiscountsFirst()
        {
            var discounts = new List<SalesLineItemDiscount>
            {
                // Setup "Buy 3 get $1.00 off" discount.
                new SalesLineItemDiscount()
                {
                    Discount = "$1.00",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "B",
                    ThresholdQuantity = 3
                },
                // Setup "Buy 6 get $3.00 off discount", which is a better deal than the previous one applied twice.
                new SalesLineItemDiscount()
                {
                    Discount = "$3.00",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "B",
                    ThresholdQuantity = 6
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(discounts);

            var sale = new Sale(mockFileManager.Object);
            sale.MakeLineItem(new Product("B", price: 1.0m), quantity: 7); // Buy 7 @ $1/each = $7 regular price

            var result = sale.GetTotal();

            Assert.That(result, Is.EqualTo(4.0m),
                "Expect that the better 'Buy 6 for $3 off' discount is applied rather than the 'Buy 3 for $1 off' discount.");
        }

        [Test]
        public void GetTotalAppliesBuyOneGetOneDiscount()
        {
            var discounts = new List<SalesLineItemDiscount>
            {
                new SalesLineItemDiscount()
                {
                    Discount = "100%",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "B",
                    DiscountAppliedOnNextProduct = true,
                    ThresholdQuantity = 1
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(discounts);

            var sale = new Sale(mockFileManager.Object);
            sale.MakeLineItem(new Product("B", 1.0m), 3);

            var result = sale.GetTotal();

            Assert.That(result, Is.EqualTo(2.0m),
                "Purchasing 3 items @ $1/each as part of Buy One Get One discount, should get 1 for free.");
        }

        [Test]
        public void GetTotalAppliesMultipleBuyTwoGetOneDiscounts()
        {
            var discounts = new List<SalesLineItemDiscount>
            {
                // Setup "Buy Two Get One" Discount
                new SalesLineItemDiscount()
                {
                    Discount = "100%",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "B",
                    DiscountAppliedOnNextProduct = true,
                    ThresholdQuantity = 2
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(discounts);

            var sale = new Sale(mockFileManager.Object);

            // Purchase 8 items (8*$1=$8) which means 2 should be free (2*$1=$2).
            // Customer is entitled to a free 9th item but did not get it.
            sale.MakeLineItem(new Product("B", price: 1.0m), quantity: 8);

            var result = sale.GetTotal();

            Assert.That(result, Is.EqualTo(6.0m),
                "Purchasing 8 items @ $1/each as part of Buy Two Get One discount, should get 2 for free.");
        }

        [Test]
        public void GetTotalAppliesMultipleBuyOneGetOneDiscounts()
        {
            var discounts = new List<SalesLineItemDiscount>
            {
                new SalesLineItemDiscount()
                {
                    Discount = "100%",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "B",
                    DiscountAppliedOnNextProduct = true,
                    ThresholdQuantity = 1
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(discounts);

            var sale = new Sale(mockFileManager.Object);
            sale.MakeLineItem(new Product("B", price: 1.0m), quantity: 3);

            var result = sale.GetTotal();

            Assert.That(result, Is.EqualTo(2.0m),
                "Purchasing 3 items @ $1/each as part of Buy One Get One discount, should get 1 for free.");
        }

        [Test]
        public void GetTotalAppliesMixOfQuantityDiscounts()
        {
            var discounts = new List<SalesLineItemDiscount>
            {
                // Setup "Buy One Get One" Discount
                new SalesLineItemDiscount()
                {
                    Discount = "100%",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "B",
                    DiscountAppliedOnNextProduct = true,
                    ThresholdQuantity = 1
                },
                 // Setup "Buy 3 get $1.00 off" quantity discount.
                new SalesLineItemDiscount()
                {
                    Discount = "$1.00",
                    EffectiveFrom = DateTime.Now.AddHours(-1),
                    EffectiveTo = DateTime.Now.AddHours(1),
                    ProductId = "A",
                    ThresholdQuantity = 3
                }
            };

            var mockFileManager = SetupFileManagerWithLineItemDiscounts(discounts);

            var sale = new Sale(mockFileManager.Object);

            // Purchase 4 B items (4*$1=$4) which means 2 should be free (2*$1=$2).
            sale.MakeLineItem(new Product("B", price: 1.0m), quantity: 4);
            // Purchase 3 A items (3*$1=$3) which means a $1 discount.
            sale.MakeLineItem(new Product("A", price: 1.0m), quantity: 3);

            var result = sale.GetTotal();

            Assert.That(result, Is.EqualTo(4.0m),
                "Purchasing 4 items @ $1/each (2 free) + 3 items @ $1/each (+ $1 discount) = $4.");
        }
        #endregion

    }
}
