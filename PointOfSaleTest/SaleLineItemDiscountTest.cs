using System;
using NUnit.Framework;
using PointOfSale;

namespace PointOfSaleTest
{
    public class SaleLineItemDiscountTest
    {
        #region IsValid

        [Test]
        public void IsValidNegativePercentageDiscountShouldReturnFalse()
        {
            var discount = new SalesLineItemDiscount()
            {
                Discount = "-10%", // cannot have negative discounts
                EffectiveFrom = DateTime.Now.AddHours(-1),
                EffectiveTo = DateTime.Now.AddHours(1),
                ProductId = "Granola",
                ThresholdQuantity = 3
            };

            Assert.That(discount.IsValid(), Is.False, "Negative discounts should be invalid.");
        }

        [Test]
        public void IsValidNegativeAbsoluteDiscountShouldReturnFalse()
        {
            var discount = new SalesLineItemDiscount()
            {
                Discount = "-10", // cannot have negative discounts
                EffectiveFrom = DateTime.Now.AddHours(-1),
                EffectiveTo = DateTime.Now.AddHours(1),
                ProductId = "Granola",
                ThresholdQuantity = 3
            };

            Assert.That(discount.IsValid(), Is.False, "Negative discounts should be invalid.");
        }

        [Test]
        public void IsValidZeroThresholdQuantityShouldReturnFalse()
        {
            var discount = new SalesLineItemDiscount()
            {
                Discount = "$10",
                EffectiveFrom = DateTime.Now.AddHours(-1),
                EffectiveTo = DateTime.Now.AddHours(1),
                ProductId = "Greek Salad",
                ThresholdQuantity = 0 // cannot have non-positive thresholds
            };

            Assert.That(discount.IsValid(), Is.False, "Non-positive threshold quantities should be invalid.");
        }

        [Test]
        public void IsValidNegativeThresholdQuantityShouldReturnFalse()
        {
            var discount = new SalesLineItemDiscount()
            {
                Discount = "$10",
                EffectiveFrom = DateTime.Now.AddHours(-1),
                EffectiveTo = DateTime.Now.AddHours(1),
                ProductId = "Greek Salad",
                ThresholdQuantity = -1 // cannot have non-positive thresholds
            };

            Assert.That(discount.IsValid(), Is.False, "Non-positive threshold quantities should be invalid.");
        }

        [Test]
        public void IsValidEffectiveToLessThanEffectiveFromShouldReturnFalse()
        {
            var discount = new SalesLineItemDiscount()
            {
                Discount = "$10",
                EffectiveFrom = DateTime.Now.AddHours(5),
                EffectiveTo = DateTime.Now.AddHours(-5), // Cannot have To < From
                ProductId = "Tuna Salad",
                ThresholdQuantity = 1
            };

            Assert.That(discount.IsValid(), Is.False,
                "'Effective' time period should start at 'From' and end at to 'To'.");
        }

        [Test]
        public void IsValidEmptyProductIdShouldReturnFalse()
        {
            var discount = new SalesLineItemDiscount()
            {
                Discount = "$10",
                EffectiveFrom = DateTime.Now.AddHours(-1),
                EffectiveTo = DateTime.Now.AddHours(1),
                ProductId = "", // cannot have empty product ids
                ThresholdQuantity = 1
            };

            Assert.That(discount.IsValid(), Is.False, "Empty product ID should be invalid.");
        }

        [Test]
        public void IsValidNullProductIdShouldReturnFalse()
        {
            var discount = new SalesLineItemDiscount()
            {
                Discount = "$10",
                EffectiveFrom = DateTime.Now.AddHours(-1),
                EffectiveTo = DateTime.Now.AddHours(1),
                ProductId = null, // cannot have null product ids
                ThresholdQuantity = 1
            };

            Assert.That(discount.IsValid(), Is.False, "Null product ID should be invalid.");
        }

        #endregion

        #region IsApplicable
        private static SalesLineItemDiscount SetupIsApplicableDiscount()
        {
            return new SalesLineItemDiscount()
            {
                Discount = "$1",
                EffectiveFrom = new DateTime(2010, 10, 1, 10, 0, 0),
                EffectiveTo = new DateTime(2010, 10, 1, 11, 0, 0),
                ProductId = "Tylenol",
                ThresholdQuantity = 1
            };
        }

        [Test]
        public void IsApplicableDateBeforeEffectiveFromShouldReturnFalse()
        {
            var currentTime = new DateTime(2010, 10, 1, 9, 59, 59);
            var result = SetupIsApplicableDiscount().IsApplicable(currentTime);

            Assert.That(result, Is.False,
                "Current time is before EffectiveFrom and the discount should not be applicable.");
        }

        [Test]
        public void IsApplicableDateAfterEffectiveToShouldReturnFalse()
        {
            var currentTime = new DateTime(2010, 10, 1, 11, 0, 1);
            var result = SetupIsApplicableDiscount().IsApplicable(currentTime);

            Assert.That(result, Is.False, "Current time is after EffectiveTo and the discount should not be applicable.");
        }

        [Test]
        public void IsApplicableDateEqualToEffectiveToShouldReturnTrue()
        {
            var currentTime = new DateTime(2010, 10, 1, 11, 0, 0);
            var result = SetupIsApplicableDiscount().IsApplicable(currentTime);

            Assert.That(result, Is.True, "Current time is equals to EffectiveTo and the discount should be applicable.");
        }

        [Test]
        public void IsApplicableDateEqualToEffectiveFromShouldReturnTrue()
        {
            var currentTime = new DateTime(2010, 10, 1, 10, 0, 0);
            var result = SetupIsApplicableDiscount().IsApplicable(currentTime);

            Assert.That(result, Is.True,
                "Current time is equals to EffectiveFrom and the discount should be applicable.");
        }

        [Test]
        public void IsApplicableDateBetweenEffectiveFromAndEffectiveToShouldReturnTrue()
        {
            var currentTime = new DateTime(2010, 10, 1, 10, 30, 0);
            var result = SetupIsApplicableDiscount().IsApplicable(currentTime);

            Assert.That(result, Is.True,
                "Current time is between EffectiveFrom and EffectiveTo so the discount should be applicable.");
        }
        #endregion
    }
}
