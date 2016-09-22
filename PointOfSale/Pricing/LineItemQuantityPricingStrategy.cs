using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSale.Repository;

namespace PointOfSale.Pricing
{
    /// <summary>
    /// Applies quantity discounts to the <see cref="Sale"/>.
    /// </summary>
    public class LineItemQuantityPricingStrategy : ISalePricingStrategy
    {
        #region Members
        private readonly List<SalesLineItemDiscount> _discounts;
        #endregion

        #region Constructors
        public LineItemQuantityPricingStrategy(List<SalesLineItemDiscount> discounts)
        {
            _discounts = discounts;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Mutates the sale with quantity type discounts and gets the total.
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public decimal GetTotal(Sale sale)
        {
            var discountedLineItems = new List<SalesLineItem>();
            
            foreach (var lineItem in sale.LineItems)
            {
                // Apply discounts directly to line item, if any.

                foreach (
                    var discount in
                    _discounts
                        .Where(d => d.IsValid() && !d.DiscountAppliedOnNextProduct)
                        .Where(d => d.ProductId == lineItem.ProductId && lineItem.Quantity >= d.ThresholdQuantity)
                        .OrderByDescending(d => d.ThresholdQuantity))
                {
                    // There may be eligible discount(s) to apply to current line item based on quantity.
                    // Apply the discounts with the largest quantities first as they generally have the better discounts.
                    
                    while (lineItem.Quantity >= discount.ThresholdQuantity)
                    {
                        // Create new line item with the quantity that gets the discount.
                        var discountedLineItem =
                            new SalesLineItem(new Product(lineItem.ProductId, lineItem.ProductPrice), discount.ThresholdQuantity);
                        discountedLineItem.SetDiscount(discount.Discount);

                        // Store in temporary list.
                        discountedLineItems.Add(discountedLineItem);
                        
                        // Decrement the threshold quantity from the original line item.
                        lineItem.AddQuantity(-1* discount.ThresholdQuantity);

                        // TODO: Add a limit, i.e. only get the discount a certain number of times.
                    }
                }
            }

            sale.RemoveZeroQuantityLineItems();
           
            // Add newly created line items that have the discount directly to the sale.
            foreach (var item in discountedLineItems)
                sale.AddLineItem(item);    

            var total = sale.LineItems.Sum(i => i.GetSubtotal());
            return total;
        }
        #endregion
    }
}
