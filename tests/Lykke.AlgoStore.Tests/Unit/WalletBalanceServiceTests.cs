using System;
using System.Collections.Generic;
using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.RateCalculator.Client;
using Lykke.Service.RateCalculator.Client.AutorestClient.Models;
using AssetPair = Lykke.Service.Assets.Client.Models.AssetPair;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class WalletBalanceServiceTests
    {
        private const string WalletId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private const string BaseAssetId = "USD";
        private const string BaseAssetIdFromAssetPair = "BTC";
        private const string QuotingAssetIdFromAssetPair = "USD";
        private const string AssetPair = "BTCUSD";

        [Test]
        public void GetTotalWalletBalanceInBaseAsset_Returns_OK()
        {
            var assetPair = Given_AssetPair();
            var balanceClient = Given_Customized_BalancesClientMock();
            var rateCalculator = Given_Customized_RateCalculatorClientMock(OperationResult.Ok);

            var service = Given_WalletBalanceService(rateCalculator, balanceClient);

            var result = When_Invoke_GetTotalWalletBalanceInBaseAsset(service, WalletId, BaseAssetId, assetPair, out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
            Then_Data_ShouldNotBe_Empty(result);
        }

        [Test]
        public void GetTotalWalletBalanceInBaseAsset_Returns_Error_CouldNotCalculate()
        {
            var assetPair = Given_AssetPair();
            var balanceClient = Given_Customized_BalancesClientMock();
            var rateCalculator = Given_Customized_RateCalculatorClientMock(OperationResult.Unknown);
            var service = Given_WalletBalanceService(rateCalculator, balanceClient);

            var result = When_Invoke_GetTotalWalletBalanceInBaseAsset(service, WalletId, BaseAssetId, assetPair, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
            Then_Data_ShouldNotBe_Empty(result);
        }

        [Test]
        public void ValidateWallet_Returns_OK()
        {
            var assetPair = Given_AssetPair();
            var balanceClient = Given_Customized_BalancesClientMock();
            var rateCalculator = Given_Customized_RateCalculatorClientMock(OperationResult.Ok);

            var service = Given_WalletBalanceService(rateCalculator, balanceClient);

            When_Invoke_When_Invoke_ValidateWallet(service, WalletId, assetPair, out Exception exception);
            Then_Exception_ShouldBe_Null(exception);
        }

        [Test]
        public void ValidateWallet_Returns_Error_NoAssetsInWallet()
        {
            var assetPair = Given_AssetPair();
            var balanceClient = Given_Error_Empty_BalancesClientMock();
            var rateCalculator = Given_Customized_RateCalculatorClientMock(OperationResult.Ok);

            var service = Given_WalletBalanceService(rateCalculator, balanceClient);

            When_Invoke_When_Invoke_ValidateWallet(service, WalletId, assetPair, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        [Test]
        public void ValidateWallet_Returns_Error_MissingAssetsInWallet()
        {
            var assetPair = Given_AssetPair();
            var balanceClient = Given_Error_WrongAssets_BalancesClientMock();
            var rateCalculator = Given_Customized_RateCalculatorClientMock(OperationResult.Ok);

            var service = Given_WalletBalanceService(rateCalculator, balanceClient);

            When_Invoke_When_Invoke_ValidateWallet(service, WalletId, assetPair, out Exception exception);
            Then_Exception_ShouldBe_ServiceException(exception);
        }

        #region Private methods

        private static IBalancesClient Given_Customized_BalancesClientMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IBalancesClient>();

            result.Setup(service => service.GetClientBalances(It.IsAny<string>()))
                .ReturnsAsync(new List<ClientBalanceResponseModel>
                {
                    fixture.Build<ClientBalanceResponseModel>()
                        .With(w => w.AssetId, BaseAssetIdFromAssetPair)
                        .Create(),

                    fixture.Build<ClientBalanceResponseModel>()
                    .With(w => w.AssetId, QuotingAssetIdFromAssetPair)
                    .Create()
                });

            return result.Object;
        }

        private static IBalancesClient Given_Error_Empty_BalancesClientMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IBalancesClient>();

            result.Setup(service => service.GetClientBalances(It.IsAny<string>()))
                .ReturnsAsync(new List<ClientBalanceResponseModel>());

            return result.Object;
        }

        private static IBalancesClient Given_Error_WrongAssets_BalancesClientMock()
        {
            var fixture = new Fixture();
            var result = new Mock<IBalancesClient>();

            result.Setup(service => service.GetClientBalances(It.IsAny<string>()))
                .ReturnsAsync(new List<ClientBalanceResponseModel>
                {
                    fixture.Build<ClientBalanceResponseModel>()
                        .Create(),

                    fixture.Build<ClientBalanceResponseModel>()
                        .Create()
                });

            return result.Object;
        }

        private static IRateCalculatorClient Given_Customized_RateCalculatorClientMock(OperationResult operationResult)
        {
            var fixture = new Fixture();
            var result = new Mock<IRateCalculatorClient>();

            result.Setup(service => service.GetMarketAmountInBaseAsync(It.IsAny<List<AssetWithAmount>>(), It.IsAny<string>(), It.IsAny<OrderAction>()))
                .ReturnsAsync(new List<ConversionResult>
                {
                    fixture.Build<ConversionResult>()
                        .With(w => w.Result, operationResult)
                        .Create()
                });

            return result.Object;
        }

        private static WalletBalanceService Given_WalletBalanceService(IRateCalculatorClient rateCalculator, IBalancesClient balancesClient)
        {
            return new WalletBalanceService(rateCalculator, balancesClient, new LogMock());
        }

        private static double When_Invoke_GetTotalWalletBalanceInBaseAsset(WalletBalanceService service, string walletId,
            string baseAssetId, AssetPair assetPair, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetTotalWalletBalanceInBaseAssetAsync(walletId, baseAssetId, assetPair).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return 0;
            }
        }
     
        private static void When_Invoke_When_Invoke_ValidateWallet(WalletBalanceService service, string walletId, AssetPair assetPair, out Exception exception)
        {
            exception = null;
            try
            {
                service.ValidateWallet(walletId, assetPair);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        private static void Then_Exception_ShouldBe_Null(Exception exception)
        {
            Assert.Null(exception);
        }

        private static void Then_Data_ShouldNotBe_Empty(double data)
        {
            Assert.NotNull(data);
        }

        private static void Then_Exception_ShouldBe_ServiceException(Exception exception)
        {
            Exception temp = exception;

            var aggr = exception as AggregateException;
            if (aggr != null)
                temp = aggr.InnerExceptions[0];

            Assert.NotNull(temp);
            var serviceException = temp as AlgoStoreException;
            Assert.NotNull(serviceException);
        }

        private static AssetPair Given_AssetPair()
        {
            var fixture = new Fixture();
            return fixture.Build<AssetPair>()
                .With(a => a.Id, AssetPair)
                .With(a => a.BaseAssetId, BaseAssetIdFromAssetPair)
                .With(a => a.QuotingAssetId, QuotingAssetIdFromAssetPair)
                .Create();
        }

        #endregion
    }
}
