using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.AlgoStore.Services.Strings;
using Lykke.AlgoStore.Services.Utils;
using Lykke.AlgoStore.TeamCityClient;
using Lykke.AlgoStore.TeamCityClient.Models;
using Newtonsoft.Json;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;
using IAlgoClientInstanceRepository = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories.IAlgoClientInstanceRepository;

namespace Lykke.AlgoStore.Services
{
    public class AlgoStoreService : BaseAlgoStoreService, IAlgoStoreService
    {
        private readonly ILoggingClient _loggingClient;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAlgoReadOnlyRepository _algoMetaDataRepository;
        private readonly IAlgoBlobReadOnlyRepository _algoBlobRepository;
        private readonly IAlgoClientInstanceRepository _algoInstanceRepository;

        private readonly IStorageConnectionManager _storageConnectionManager;
        private readonly ITeamCityClient _teamCityClient;

        private readonly IAlgoInstanceStoppingClient _algoInstanceStoppingClient;

        private readonly IPublicAlgosRepository _publicAlgosRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgoStoreService"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="algoBlobRepository">The algo BLOB repository.</param>
        /// <param name="algoMetaDataRepository">The algo meta data repository.</param>
        /// <param name="storageConnectionManager">The storage connection manager.</param>
        /// <param name="teamCityClient">The team city client.</param>
        /// <param name="algoInstanceStoppingClient">The algo instance stopping service client.</param>
        /// <param name="algoInstanceRepository">The algo instance repository.</param>
        /// <param name="publicAlgosRepository">The public algo repository.</param>
        /// <param name="statisticsRepository">The statistics repository.</param>
        /// <param name="loggingClient">The user log repository.</param>
        public AlgoStoreService(
            ILog log,
            IAlgoBlobReadOnlyRepository algoBlobRepository,
            IAlgoReadOnlyRepository algoMetaDataRepository,
            IStorageConnectionManager storageConnectionManager,
            ITeamCityClient teamCityClient,
            IAlgoInstanceStoppingClient algoInstanceStoppingClient,
            IAlgoClientInstanceRepository algoInstanceRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IStatisticsRepository statisticsRepository,
            ILoggingClient loggingClient) : base(log, nameof(AlgoStoreService))
        {
            _algoBlobRepository = algoBlobRepository;
            _algoMetaDataRepository = algoMetaDataRepository;
            _storageConnectionManager = storageConnectionManager;
            _teamCityClient = teamCityClient;
            _algoInstanceStoppingClient = algoInstanceStoppingClient;
            _algoInstanceRepository = algoInstanceRepository;
            _publicAlgosRepository = publicAlgosRepository;
            _statisticsRepository = statisticsRepository;
            _loggingClient = loggingClient;
        }

        /// <summary>
        /// Deploys the image asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<bool> DeployImageAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(DeployImageAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _algoBlobRepository.BlobExistsAsync(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoBinaryDataNotFound, "No blob for provided id");

                await Check.Algo.Exists(_algoMetaDataRepository, data.AlgoClientId, data.AlgoId);
                await Check.Algo.IsVisibleForClient(_publicAlgosRepository, data.AlgoId, data.ClientId, data.AlgoClientId);

                var instanceData = await _algoInstanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"No instance data for algo id {data.AlgoId}",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "algo instance"));

                if (!instanceData.ValidateData(out var instanceException))
                    throw instanceException;

                string blobKey = data.AlgoId + _algoBlobRepository.SourceExtension;
                var headers = _storageConnectionManager.GetData(blobKey);

                dynamic algoInstanceParameters = new ExpandoObject();
                algoInstanceParameters.AlgoId = data.AlgoId;
                algoInstanceParameters.InstanceId = data.InstanceId;
                algoInstanceParameters.InstanceType = instanceData.AlgoInstanceType.ToString();
                algoInstanceParameters.AuthToken = instanceData.AuthToken;

                var buildData = new TeamCityClientBuildData
                {
                    BlobAuthorizationHeader = headers.AuthorizationHeader,
                    BlobUrl = headers.Url,
                    BlobVersionHeader = headers.VersionHeader,
                    BlobDateHeader = headers.DateHeader,
                    AlgoId = data.InstanceId,
                    TradedAsset = instanceData.TradedAssetId,
                    AssetPair = instanceData.AssetPairId,
                    Volume = instanceData.Volume,
                    Margin = instanceData.Margin,
                    HftApiKey = "Dummy HFT Key",
                    HftApiUrl = "Dummy HFT Url",
                    WalletApiKey = "Dummy Wallet Key",
                    AlgoInstanceParameters = JsonConvert.SerializeObject(algoInstanceParameters)
                };

                var response = await _teamCityClient.StartBuild(buildData);

                return response.GetBuildState() != BuildStates.Undefined;
            });
        }

        /// <summary>
        /// Stops the test image asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<string> StopTestImageAsync(ManageImageData data)
        {
            return await LogTimedInfoAsync(nameof(StopTestImageAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                var algoId = data.AlgoId;

                await Check.Algo.Exists(_algoMetaDataRepository, data.AlgoClientId, algoId);
                await Check.Algo.IsVisibleForClient(_publicAlgosRepository, data.AlgoId, data.ClientId, data.AlgoClientId);

                var instanceData = await _algoInstanceRepository.GetAlgoInstanceDataByAlgoIdAsync(data.AlgoId, data.InstanceId);
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"No instance data for algo id {data.AlgoId}",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "algo instance"));

                var pods = await _algoInstanceStoppingClient.GetPodsAsync(data.InstanceId, instanceData.AuthToken);

                if (!string.IsNullOrEmpty(pods.Error?.ErrorMessage))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, string.Format(Phrases.ErrorGettingPod, pods.Error?.ErrorMessage));

                if (pods.Records.IsNullOrEmptyCollection())
                    return AlgoInstanceStatus.Deploying.ToString();

                if (pods.Records.Count > 1)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.MoreThanOnePodFound, $"More than one pod for algoId {algoId}");

                var pod = pods.Records[0];
                if (pod == null)
                    return AlgoInstanceStatus.Deploying.ToString();

                var result = await _algoInstanceStoppingClient.DeleteAlgoInstanceByInstanceIdAndPodAsync(data.InstanceId, pod.NamespaceProperty, instanceData.AuthToken);

                if (!result.IsSuccessfulDeletion)
                {
                    await _loggingClient.WriteAsync(instanceData.InstanceId, string.Format(Phrases.DeleteKubernetesDeploymentError, result.ErrorMessage));
                    return pod.Phase.ToUpper();
                }

                instanceData.AlgoInstanceStatus = AlgoInstanceStatus.Stopped;
                await _algoInstanceRepository.SaveAlgoInstanceDataAsync(instanceData);

                return AlgoInstanceStatus.Stopped.ToString();
            });
        }

        public async Task DeleteInstanceAsync(AlgoClientInstanceData instanceData)
        {
            await LogTimedInfoAsync(nameof(DeleteInstanceAsync), instanceData?.ClientId, async () =>
            {
                if (instanceData == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound, $"Bad instance data",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "algo instance"));

                var pods = await _algoInstanceStoppingClient.GetPodsAsync(instanceData.InstanceId, instanceData.AuthToken);

                if (!string.IsNullOrEmpty(pods.Error?.ErrorMessage))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError, string.Format(Phrases.ErrorGettingPod, pods.Error?.ErrorMessage));

                if (!pods.Records.IsNullOrEmptyCollection() && pods.Records[0] != null)
                {
                    var pod = pods.Records[0];

                    var result = await _algoInstanceStoppingClient.DeleteAlgoInstanceByInstanceIdAndPodAsync(instanceData.InstanceId, pod.NamespaceProperty, instanceData.AuthToken);

                    if (!result.IsSuccessfulDeletion)
                    {
                        await _loggingClient.WriteAsync(instanceData.InstanceId, string.Format(Phrases.DeleteKubernetesDeploymentError, result.ErrorMessage));
                        throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                            $"Cannot delete image id {instanceData.InstanceId} for algo id {instanceData.AlgoId}");
                    }
                }

                await _algoInstanceRepository.DeleteAlgoInstanceDataAsync(instanceData);

                //REMARK: Two lines below are commented out until we reach final decision on deletion
                //await _userLogRepository.DeleteAllAsync(instanceData.InstanceId);
                //await _statisticsRepository.DeleteAllAsync(instanceData.InstanceId);
            });
        }

        /// <summary>
        /// Gets the test tail log asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task<string[]> GetTestTailLogAsync(TailLogData data)
        {
            return await LogTimedInfoAsync(nameof(GetTestTailLogAsync), data.ClientId, async () =>
            {
                if (!data.ValidateData(out var exception))
                    throw exception;

                if (!await _algoInstanceRepository.ExistsAlgoInstanceDataWithClientIdAsync(data.ClientId, data.InstanceId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstanceDataNotFound,
                        $"Instance data not found data for clientId {data.ClientId}, algo {data.AlgoId} and instanceId {data.InstanceId}",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "algo instance"));

                var userLogs = await _loggingClient.GetTailLog(data.Tail, data.InstanceId);
                return userLogs.Select(l => $"[{l.Date.ToString(AlgoStoreConstants.CustomDateTimeFormat)}] {l.Message}").ToArray();
            });
        }
    }
}
