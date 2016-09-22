using System;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PointOfSale;
using PointOfSale.Pricing;
using PointOfSale.Repository;

namespace PointOfSaleTest.Pricing
{
    public class PricingStrategyFactoryTest
    {
        /// <summary>
        /// Get a mock that returns the overall strategy property to the factory.
        /// </summary>
        /// <param name="overallStrategy"></param>
        /// <returns></returns>
        private static Mock<IFileManager> SetupFileManagerWithOverallStrategy(string overallStrategy)
        {
            var mockFileManager = new Mock<IFileManager>();
            mockFileManager.Setup(fm => fm.GetFileText(It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(new PromotionCatalog() { OverallStrategy = overallStrategy}));
            return mockFileManager;
        }

        #region GetSalePricingStrategy
        [Test]
        public void GetSalePricingStrategyWithBestForCustomerShouldReturnBestForCustomerPricingStrategy()
        {
            var result =
                new PricingStrategyFactory(SetupFileManagerWithOverallStrategy("BestForCustomer").Object)
                    .GetSalePricingStrategy();

            Assert.That(result, Is.Not.Null, "Should get back a pricing strategy from factory.");
            Assert.That(result, Is.TypeOf<BestForCustomerPricingStrategy>(),
                $"Should get back a strategy of type {typeof(BestForCustomerPricingStrategy).Name}");
        }

        [Test]
        public void GetSalePricingStrategyWithBestForStoreShouldReturnBestForStorePricingStrategy()
        {
            var result =
                new PricingStrategyFactory(SetupFileManagerWithOverallStrategy("BestForStore").Object)
                    .GetSalePricingStrategy();

            Assert.That(result, Is.Not.Null, "Should get back a pricing strategy from factory.");
            Assert.That(result, Is.TypeOf<BestForStorePricingStrategy>(),
                $"Should get back a strategy of type {typeof(BestForStorePricingStrategy).Name}");
        }

        [Test]
        public void GetSalePricingStrategyWithUnknownOverallStrategyShouldThrowException()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                    new PricingStrategyFactory(
                        SetupFileManagerWithOverallStrategy("StrategyThatDoesNotExist").Object).GetSalePricingStrategy());
        }
        #endregion
    }
}
