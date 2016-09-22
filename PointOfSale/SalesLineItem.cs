using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale
{
    public class SalesLineItem
    {
        #region Members
        /// <summary>
        /// An absolute dollar amount to subtract from the line item's subtotal.
        /// </summary>
        private decimal _discount;

        /// <summary>
        /// Gets the quantity of the line time that the customer wishes to purchase.
        /// </summary>
        public int Quantity { get; private set; }
        /// <summary>
        /// Gets the underlying <see cref="Product"/> of the line item.
        /// </summary>
        private Product Product { get; }

        public string ProductId => Product.Id;
        public decimal ProductPrice => Product.Price;

        #endregion

        #region Constructors
        public SalesLineItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public SalesLineItem(Product product) : this (product, 1)
        {
            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Apply an absolute dollar amount discount to the current sales line item.
        /// </summary>
        /// <param name="discount">Absolute dollar amount discount.</param>
        public void SetDiscount(decimal discount)
        {
            if (discount <= 0)
                throw new ArgumentOutOfRangeException(nameof(discount), discount,
                    "Cannot set the line item's discount to a non-positive value.");
            _discount = discount;
        }

        /// <summary>
        /// <para>Set discount using a formatted string.</para>
        /// <para>Accepted formats are "$#.##"/"#.##" or "#%".</para>
        /// </summary>
        /// <param name="discount"></param>
        public void SetDiscount(string discount)
        {
            if (string.IsNullOrEmpty(discount))
                throw new ArgumentNullException(nameof(discount));
            
            SetDiscount(Discount.FromString(discount));
        }

        public void SetDiscount(Discount discount)
        {
            switch (discount.DiscountType)
            {
                case DiscountType.Absolute:
                    SetDiscount(discount.Value);
                    break;
                case DiscountType.Percentage:
                    SetDiscount(discount.Value / 100m * Product.Price * Quantity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the absolute discount currently applied to the line item.
        /// </summary>
        /// <returns></returns>
        public decimal GetDiscount()
        {
            return _discount;
        }

        public void AddQuantity(int quantity)
        {
            var result = Quantity + quantity;

            if (result < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), quantity,
                    "Value would set line item quantity to a negative value.");

            Quantity = result;
        }

        public decimal GetSubtotal()
        {
            return Product.Price*Quantity-_discount;
        }

        public string GetLineItemString()
        {
            var sb = new StringBuilder();

            var subtotalPerQuantity = Quantity == 0 ? 0.0m : GetSubtotal()/Quantity;
            var discountPerQuantity = Quantity == 0 ? 0.0m : _discount/Quantity;

            for (var i = 0; i < Quantity; i++)
            {
                sb.Append($"{Product.Id}".PadRight(30));
                sb.Append($"{subtotalPerQuantity:c}".PadRight(8));
                if (_discount > 0)
                {
                    sb.Append($" ({Product.Price:c}".PadRight(8) + "R)");
                    sb.AppendLine();
                    sb.Append(" You saved".PadRight(12));
                    sb.Append($"{discountPerQuantity:c}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        
        public override bool Equals(object obj)
        {
            var salesLineItem = obj as SalesLineItem;
            return salesLineItem != null && Product.Equals(salesLineItem.Product);
        }

        public override int GetHashCode()
        {
            return ProductId.GetHashCode();
        }

        public override string ToString()
        {
            return _discount > 0
                ? $"{ProductId} @ {ProductPrice:c} * {Quantity} - {_discount:c}"
                : $"{ProductId} @ {ProductPrice:c} * {Quantity}";
        }
        #endregion
    }
}
