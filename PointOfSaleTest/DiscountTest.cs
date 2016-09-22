using System;
using NUnit.Framework;
using PointOfSale;

namespace PointOfSaleTest
{
    public class DiscountTest
    {
        #region FromString
        [Test]
        public void FromStringParsesNumericsToAbsoluteDiscount()
        {
            var result = Discount.FromString("1");
            Assert.That(result.DiscountType, Is.EqualTo(DiscountType.Absolute));
            Assert.That(result.Value, Is.EqualTo(1m));
        }

        [Test]
        public void FromStringParsesDollarSignFormatToAbsoluteDiscount()
        {
            var result = Discount.FromString("$1");
            Assert.That(result.DiscountType, Is.EqualTo(DiscountType.Absolute));
            Assert.That(result.Value, Is.EqualTo(1m));
        }

        [Test]
        public void FromStringParsesDollarSignFormatToAbsoluteDiscountWithPrecision()
        {
            var result = Discount.FromString("$1.0001");
            Assert.That(result.DiscountType, Is.EqualTo(DiscountType.Absolute));
            Assert.That(result.Value, Is.EqualTo(1.0001m));
        }

        [Test]
        public void FromStringParsesDollarSignFormatIgnoresWhiteSpace()
        {
            var result = Discount.FromString("$ 1.0001 ");
            Assert.That(result.DiscountType, Is.EqualTo(DiscountType.Absolute));
            Assert.That(result.Value, Is.EqualTo(1.0001m));
        }

        [Test]
        public void FromStringParsesPercentageFormatToPercentageDiscount()
        {
            var result = Discount.FromString("1%");
            Assert.That(result.DiscountType, Is.EqualTo(DiscountType.Percentage));
            Assert.That(result.Value, Is.EqualTo(1m));
        }

        [Test]
        public void FromStringParsesPercentageFormatToPercentageDiscountWithPrecision()
        {
            var result = Discount.FromString("1.0001%");
            Assert.That(result.DiscountType, Is.EqualTo(DiscountType.Percentage));
            Assert.That(result.Value, Is.EqualTo(1.0001m));
        }
        
        [Test]
        public void FromStringParsesPercentageFormatIgnoresWhiteSpace()
        {
            var result = Discount.FromString(" 100 %");
            Assert.That(result.DiscountType, Is.EqualTo(DiscountType.Percentage));
            Assert.That(result.Value, Is.EqualTo(100m));
        }

        [Test]
        public void FromStringWithPercentageFormatOver100ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Discount.FromString("100.1%"),
                "Stores rarely want to pay customers to take inventory.");
        }

        [Test]
        public void FromStringWithNegativePercentageFormatThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Discount.FromString("-0.1%"),
                "More of a fee than a discount.");
        }

        [Test]
        public void FromStringWithZeroPercentageFormatThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Discount.FromString("0%"),
                "0% discount doesn't mean anything.");
        }

        [Test]
        public void FromStringWithInvalidPercentageFormatThrowsException()
        {
            Assert.Throws<FormatException>(() => Discount.FromString("x1%"));
        }

        [Test]
        public void FromStringWithNegativeAbsoluteFormatThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Discount.FromString("-1"),
                "More of a fee than a discount.");
        }

        [Test]
        public void FromStringWithZeroAbsoluteFormatThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Discount.FromString("$0.00"),
                "$0 discount doesn't mean anything.");
        }

        [Test]
        public void FromStringWithInvalidAbsoluteFormatThrowsException()
        {
            Assert.Throws<FormatException>(() => Discount.FromString("$x1"));
        }

        #endregion
    }
}
