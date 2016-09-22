using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale
{
    public enum DiscountType
    {
        Absolute,
        Percentage
    }

    /// <summary>
    /// Helper structure to represent a discount as an absolute or percentage value.
    /// </summary>
    public struct Discount
    {
        #region Members
        public DiscountType DiscountType { get; private set; }
        public decimal Value { get; private set; }
        #endregion

        #region Constructors
        public Discount(DiscountType type, decimal value)
        {
            DiscountType = type;
            Value = value;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// <para>Parses given string to <see cref="Discount"/> instance.</para>
        /// <para>Can be absolute dollar amounts "<example>$1.10</example>" or a percentage "<example>15%</example>".</para>
        /// <para>Otherwise, plain numerics will be treated as an absolute dollar amount "<example>1.10</example>" => $1.10.</para>
        /// </summary>
        /// <param name="discount"></param>
        /// <returns></returns>
        public static Discount FromString(string discount)
        {
            if (string.IsNullOrEmpty(discount))
                throw new ArgumentNullException(nameof(discount));

            var value = 0.0m;
            var type = DiscountType.Absolute;

            if (discount.EndsWith("%"))
            {
                type = DiscountType.Percentage;
                if (!decimal.TryParse(discount.Substring(0, discount.Length - 1), out value))
                    throw new FormatException($"Discount string '{discount}' is an unrecognized format.");

                if (value <= 0 || value > 100)
                    throw new ArgumentOutOfRangeException(
                        $"Discount percentage '{value}%' is invalid, must be positive value less than or equal to 100.");
            }
            else
            {
                if (!decimal.TryParse(discount.StartsWith("$") ? discount.Substring(1) : discount, out value))
                    throw new FormatException($"Discount string '{discount}' is an unrecognized format.");

                if (value <= 0)
                    throw new ArgumentOutOfRangeException(
                        $"Absolute discounts cannot be negative or zero, {value} is invalid.");
            }

            return new Discount(type, value);
        }
        #endregion
    }
}
