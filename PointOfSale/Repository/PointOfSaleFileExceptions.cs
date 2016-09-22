using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Repository
{
    /// <summary>
    /// Exception when a required field is missing from the <see cref="Product"/> definition.
    /// </summary>
    public class MissingProductFieldException : Exception
    {
        public MissingProductFieldException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception when a required field is missing from the <see cref="SalesLineItemDiscount"/> definition.
    /// </summary>
    public class MissingSalesLineItemDiscountFieldException : Exception
    {
        public MissingSalesLineItemDiscountFieldException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception when a field in the <see cref="Product"/> definition has an invalid value.
    /// </summary>
    public class InvalidProductValueException : Exception
    {
        public InvalidProductValueException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception when a field in the <see cref="SalesLineItemDiscount"/> definition has an invalid value.
    /// </summary>
    public class InvalidSalesLineItemDiscountValueException : Exception
    {
        public InvalidSalesLineItemDiscountValueException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception when the <see cref="ProductCatalog"/> cannot be read.
    /// </summary>
    public class ProductCatalogFileException : Exception
    {
        public ProductCatalogFileException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception when the <see cref="PromotionCatalog"/> cannot be read.
    /// </summary>
    public class PromotionCatalogFileException : Exception
    {
        public PromotionCatalogFileException(string message, Exception innerException) : base(message, innerException) { }
    }
}