using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.DeploymentApiClient;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreService : BaseAlgoStoreService, IAlgoStoreService
    {
        private readonly IAlgoBlobReadOnlyRepository _algoBlobRepository;
        private readonly IAlgoMetaDataReadOnlyRepository _algoMetaDataRepository;
        private readonly IAlgoRuntimeDataRepository _algoRuntimeDataRepository;
        private readonly IDeploymentApiClient _externalClient;

        public AlgoStoreService(
            IDeploymentApiClient externalClient,
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoMetaDataReadOnlyRepository algoMetaDataRepository,
            IAlgoRuntimeDataRepository algoRuntimeDataRepository) : base(log, nameof(AlgoStoreService))
        {
            _externalClient = externalClient;
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _algoRuntimeDataRepository = algoRuntimeDataRepository;
        }

        public async Task<bool> DeployImage(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(DeployImage), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _algoBlobRepository.BlobExists(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoBinaryDataNotFound, "No blob for provided id");

                if (!await _algoMetaDataRepository.ExistsAlgoMetaData(data.ClientId, data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo for provided id");

                var algo = await _algoMetaDataRepository.GetAlgoMetaData(data.ClientId, data.AlgoId);
                var algoMetaData = algo.AlgoMetaData?.FirstOrDefault();

                if (algoMetaData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound, "No algo meta data for provided id");

                var blob = await _algoBlobRepository.GetBlobAsync(data.AlgoId);

                var response =
                    await _externalClient.BuildAlgoImageFromBinary(blob, data.ClientId, algoMetaData.AlgoId);

                var imageId = int.Parse(response);
                var testId = await _externalClient.CreateTestAlgo(imageId, algoMetaData.AlgoId);
                if (testId < 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, "Error creating test");

                var runtimeData = new AlgoClientRuntimeData
                {
                    ClientId = data.ClientId,
                    AlgoId = data.AlgoId,
                    ImageId = testId,
                    BuildImageId = imageId
                };

                await _algoRuntimeDataRepository.SaveAlgoRuntimeData(runtimeData);

                return true;
            });
        }
    }
}
