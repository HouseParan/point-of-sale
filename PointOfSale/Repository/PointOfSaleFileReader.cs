using System;
using Newtonsoft.Json;

namespace PointOfSale.Repository
{
    /// <summary>
    /// <para>Simple class for deserializing the PoS's two system files.</para>
    /// <para>More involved file parsing/IO/etc. could be handled by separate, dedicated classes.</para>
    /// </summary>
    public class PointOfSaleFileReader
    {
        #region Members
        private readonly IFileManager _fileManager;
        #endregion

        #region Constructors
        public PointOfSaleFileReader() : this(new SystemFileManager())
        {

        }

        public PointOfSaleFileReader(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="ProductCatalog"/> instance that is constructed from the file found at the given <see cref="path"/>.
        /// </summary>
        /// <param name="path">The product catalog file.</param>
        /// <returns></returns>
        public ProductCatalog ReadProductCatalog(string path)
        {
            string catalogJsonText;

            try
            {
                catalogJsonText = _fileManager.GetFileText(path);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new ProductCatalogFileException(
                    $"Product catalog was not found ({path}). Needs to be located the EXE.", null);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ProductCatalogFileException(
                    $"Product catalog was not read ({path}). It is unaccessible to the current user.", ex);
            }

            try
            {
                var catalog = JsonConvert.DeserializeObject<ProductCatalog>(catalogJsonText);
                catalog.RemoveDuplicates();
                catalog.Validate();
                return catalog;
            }
            catch (JsonSerializationException jse) when (jse.Message.Contains("Required property"))
            {
                throw new MissingProductFieldException(
                    $"A required field is missing from the product catalog ({path}). {jse.Message}", jse);
            }
            catch (JsonReaderException jre)
            {
                throw new ProductCatalogFileException(
                    $"Product catalog could not be read ({path}). {jre.Message}", jre);
            }
        }

        /// <summary>
        /// Returns a <see cref="PromotionCatalog"/> instance that is constructed from the file found at the given <see cref="path"/>.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PromotionCatalog ReadPromotionCatalog(string path)
        {
            string promotionsJsonText;

            try
            {
                promotionsJsonText = _fileManager.GetFileText(path);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new PromotionCatalogFileException(
                    $"Promotions catalog was not found ({path}). Needs to be located the EXE.", null);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PromotionCatalogFileException(
                    $"Promotions catalog was not read ({path}). It is unaccessible to the current user.", ex);
            }

            try
            {
                var catalog = JsonConvert.DeserializeObject<PromotionCatalog>(promotionsJsonText);
                catalog.RemoveInapplicablePromotions(DateTime.Now);
                catalog.RemoveInvalidPromotions(); // Remove invalid promotions, assumed that badly configured promotions should not halt execution.
                return catalog;
            }
            catch (JsonSerializationException jse) when (jse.Message.Contains("Required property"))
            {
                throw new MissingSalesLineItemDiscountFieldException(
                    $"A required field is missing from a sales line item discount ({path}). {jse.Message}", jse);
            }
            catch (JsonReaderException jre)
            {
                throw new PromotionCatalogFileException(
                    $"Promotions catalog could not be read ({path}). {jre.Message}", jre);
            }
        } 
        #endregion
    }
}
