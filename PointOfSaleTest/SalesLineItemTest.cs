using System;
using NUnit.Framework;
using PointOfSale;

namespace PointOfSaleTest
{
    public class SalesLineItemTest
    {
        #region SetDiscount_String
        [Test]
        public void SetDiscountStringAppliesWholePercentageDiscountOnLineItemWithSingleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.SetDiscount("50%");
            
            Assert.That(lineItem.GetDiscount(), Is.EqualTo(0.5m));
        }

        [Test]
        public void SetDiscountStringAppliesPercentageDiscountOnLineItemWithSingleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.SetDiscount("33%");
            
            Assert.That(lineItem.GetDiscount(), Is.EqualTo(0.33m));
        }

        [Test]
        public void SetDiscountStringAppliesPercentageDiscountOnLineItemWithMultipleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            lineItem.SetDiscount("50%");
            
            Assert.That(lineItem.GetDiscount(), Is.EqualTo(5.00m));
        }

        [Test]
        public void SetDiscountStringAppliesAbsoluteDiscountOnLineItemWithSingleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.SetDiscount("$0.50");

            Assert.That(lineItem.GetDiscount(), Is.EqualTo(0.5m));
        }

        [Test]
        public void SetDiscountStringAppliesAbsoluteDiscountOnLineItemWithMultipleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            lineItem.SetDiscount("$5.00");

            Assert.That(lineItem.GetDiscount(), Is.EqualTo(5.00m));
        }

        [Test]
        public void SetDiscountStringWithNegativePercentageDiscountShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => lineItem.SetDiscount("-50%"),
                "Attempting to set a negative discount should result in an exception.");
        }

        [Test]
        public void SetDiscountStringWithNegativeAbsoluteDiscountShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => lineItem.SetDiscount("-5.00"),
               "Attempting to set a negative discount should result in an exception.");
        }

        [Test]
        public void SetDiscountStringWithZeroAbsoluteDiscountShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => lineItem.SetDiscount("0.00"),
               "Attempting to set a negative discount should result in an exception.");
        }

        [Test]
        public void SetDiscountStringWithNegativeMoneyDiscountShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => lineItem.SetDiscount("$-5.00"),
               "Attempting to set a negative discount should result in an exception.");
        }

        [Test]
        public void SetDiscountStringWithInvalidDiscountStringShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            Assert.Throws<FormatException>(() => lineItem.SetDiscount("$x"),
               "Attempting to set a invalid discount should result in an exception.");
        }

        [Test]
        public void SetDiscountStringWithNullOrEmptyDiscountStringShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            Assert.Throws<ArgumentNullException>(() => lineItem.SetDiscount(""),
               "Attempting to set a empty discount should result in an exception.");
            Assert.Throws<ArgumentNullException>(() => lineItem.SetDiscount(null),
               "Attempting to set a null discount should result in an exception.");
        }
        #endregion

        #region SetDiscount_Decimal
        [Test]
        public void SetDiscountDecimalWithNegativeValueShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => lineItem.SetDiscount(-10m));
        }

        [Test]
        public void SetDiscountDecimalWithZeroValueShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => lineItem.SetDiscount(0m));
        }
        #endregion

        #region SetDiscount_Discount
        [Test]
        public void SetDiscountDiscountAppliesWholePercentageDiscountOnLineItemWithSingleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.SetDiscount(new Discount(DiscountType.Percentage, 50));
            
            Assert.That(lineItem.GetDiscount(), Is.EqualTo(0.5m));
        }

        [Test]
        public void SetDiscountDiscountAppliesPercentageDiscountOnLineItemWithSingleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.SetDiscount(new Discount(DiscountType.Percentage, 33));
            
            Assert.That(lineItem.GetDiscount(), Is.EqualTo(0.33m));
        }

        [Test]
        public void SetDiscountDiscountAppliesPercentageDiscountOnLineItemWithMultipleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            lineItem.SetDiscount(new Discount(DiscountType.Percentage, 50));
            
            Assert.That(lineItem.GetDiscount(), Is.EqualTo(5.00m));
        }

        [Test]
        public void SetDiscountDiscountAppliesAbsoluteDiscountOnLineItemWithSingleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.SetDiscount(new Discount(DiscountType.Absolute, 0.5m));

            Assert.That(lineItem.GetDiscount(), Is.EqualTo(0.5m));
        }

        [Test]
        public void SetDiscountDiscountAppliesAbsoluteDiscountOnLineItemWithMultipleQuantity()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 10);

            lineItem.SetDiscount(new Discount(DiscountType.Absolute, 5.0m));

            Assert.That(lineItem.GetDiscount(), Is.EqualTo(5.00m));
        }
        #endregion

        #region AddQuantity
        [Test]
        public void AddQuantityPositiveValueShouldIncrement()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.AddQuantity(1);

            Assert.That(lineItem.Quantity, Is.EqualTo(2));
        }

        [Test]
        public void AddQuantityNegativeValueShouldDecrement()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 1);

            lineItem.AddQuantity(-1);

            Assert.That(lineItem.Quantity, Is.EqualTo(0));
        }

        [Test]
        public void AddQuantityNegativeValueThatDecrementsQuantityToNegativeValueShouldThrowException()
        {
            var lineItem = new SalesLineItem(new Product("A", price: 1.0m), quantity: 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => lineItem.AddQuantity(-1));
        }
        #endregion
    }
}
