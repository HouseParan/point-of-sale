using System;
using System.Collections.Generic;
using NUnit.Framework;
using PointOfSale;

namespace PointOfSaleTest
{
    public class PromotionCatalogTest
    {
        [Test]
        public void RemoveInapplicabePromotionsShouldRemoveAllInapplicablePromotions()
        {
            var catalog = new PromotionCatalog()
            {
                SalesLineItemDiscounts = new List<SalesLineItemDiscount>()
                {
                    new SalesLineItemDiscount()
                    {
                        Discount = "$1",
                        EffectiveFrom = DateTime.Now.AddHours(-10),
                        EffectiveTo = DateTime.Now.AddHours(-1), // Expired
                        ProductId = "Tylenol",
                        ThresholdQuantity = 1
                    },
                    new SalesLineItemDiscount()
                    {
                        Discount = "$1",
                        EffectiveFrom = DateTime.Now.AddHours(-10),
                        EffectiveTo = DateTime.Now.AddHours(-1), // Expired
                        ProductId = "Advil",
                        ThresholdQuantity = 1
                    },
                    new SalesLineItemDiscount()
                    {
                        Discount = "$1",
                        EffectiveFrom = DateTime.Now.AddHours(-10),
                        EffectiveTo = DateTime.Now.AddHours(10),
                        ProductId = "Motrin",
                        ThresholdQuantity = 1
                    }
                }
            };

            catalog.RemoveInapplicablePromotions(DateTime.Now);

            Assert.That(catalog.SalesLineItemDiscounts.Count, Is.EqualTo(1),
                "Expect that the only a single applicable promotion is left in the catalog.");
        }

        [Test]
        public void RemoveInvalidPromotionsShouldRemoveAllInvalidPromotions()
        {
            var catalog = new PromotionCatalog()
            {
                SalesLineItemDiscounts = new List<SalesLineItemDiscount>()
                {
                    new SalesLineItemDiscount()
                    {
                        Discount = "$1",
                        EffectiveFrom = DateTime.Now.AddHours(-10),
                        EffectiveTo = DateTime.Now.AddHours(10),
                        ProductId = "Tylenol",
                        ThresholdQuantity = 0 // invalid
                    },
                    new SalesLineItemDiscount()
                    {
                        Discount = "~~~$1~~~", // invalid
                        EffectiveFrom = DateTime.Now.AddHours(-10),
                        EffectiveTo = DateTime.Now.AddHours(10), 
                        ProductId = "Advil",
                        ThresholdQuantity = 1
                    },
                    new SalesLineItemDiscount()
                    {
                        Discount = "$1",
                        EffectiveFrom = DateTime.Now.AddHours(-10),
                        EffectiveTo = DateTime.Now.AddHours(10),
                        ProductId = "Motrin",
                        ThresholdQuantity = 1
                    }
                }
            };

            catalog.RemoveInvalidPromotions();

            Assert.That(catalog.SalesLineItemDiscounts.Count, Is.EqualTo(1),
                "Expect that the only a single valid promotion is left in the catalog.");
        }


    }
}
