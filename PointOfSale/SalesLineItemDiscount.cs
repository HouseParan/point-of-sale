using Newtonsoft.Json;
using System;

namespace PointOfSale
{
    public class SalesLineItemDiscount
    {
        #region Members
        /// <summary>
        /// Gets/sets the start date/time when the discount comes into effect.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DateTime EffectiveFrom;

        /// <summary>
        /// Gets/sets the end date/time when the discount no longer applies.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DateTime EffectiveTo;

        /// <summary>
        /// Gets/sets the required quantity of products that the customer must buy before the discount is applied.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int ThresholdQuantity;

        /// <summary>
        /// Gets/sets the <see cref="Product"/> ID that the discount is applied to.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ProductId;

        /// <summary>
        /// <para>Gets/sets a friendly string that represents the discount to apply.</para>
        /// <para>Can be absolute dollar amounts "<example>$1.10</example>" or a percentage "<example>15%</example>".</para>
        /// <para>Otherwise, plain numerics will be treated as an absolute dollar amount "<example>1.10</example>" => $1.10.</para>
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Discount;

        /// <summary>
        /// <para>Gets/sets whether the discount is applied on the next item quantity after <see cref="ThresholdQuantity"/>.</para>
        /// <para>E.g. a "Buy One, Get One" discount would have the following configuration:</para>
        /// <list type="bullet">
        ///     <item><description><see cref="ThresholdQuantity"/> = 1,</description></item>
        ///     <item><description><see cref="DiscountAppliedOnNextProduct"/> = true, and</description></item>
        ///     <item><description><see cref="Discount"/> = "100%".</description></item> 
        /// </list>
        /// </summary>
        public bool DiscountAppliedOnNextProduct;
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns whether the discount is in effect for the given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsApplicable(DateTime date)
        {
            return EffectiveFrom <= date && date <= EffectiveTo;
        }

        /// <summary>
        /// Returns whether the discount has a valid configuration.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (EffectiveTo < EffectiveFrom)
                return false;
            
            if (string.IsNullOrEmpty(Discount))
                return false;
            
            if (string.IsNullOrEmpty(ProductId))
                return false;
            
            if (ThresholdQuantity <= 0)
                return false;
            
            try
            {
                PointOfSale.Discount.FromString(Discount);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
