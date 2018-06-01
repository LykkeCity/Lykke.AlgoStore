using Common.Log;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.Services.Strings;
using Lykke.Service.PersonalData.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Newtonsoft.Json;
using IAlgoClientInstanceRepository = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories.IAlgoClientInstanceRepository;
using Lykke.AlgoStore.Services.Utils;

namespace Lykke.AlgoStore.Services
{
    public class AlgosService : BaseAlgoStoreService, IAlgosService
    {
        private readonly IAlgoRepository _algoRepository;
        private readonly IAlgoBlobRepository _blobRepository;
        private readonly IAlgoClientInstanceRepository _instanceRepository;
        private readonly IAlgoRatingsRepository _ratingsRepository;
        private readonly IPublicAlgosRepository _publicAlgosRepository;
        private readonly IPersonalDataService _personalDataService;
        private readonly ICodeBuildService _codeBuildService;

        private static Random rnd = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgosService"/> class.
        /// </summary>
        /// <param name="algoRepository">The algo repository.</param>
        /// <param name="blobRepository">The BLOB repository.</param>
        /// <param name="instanceRepository">The instance repository.</param>
        /// <param name="ratingsRepository">The ratings repository.</param>
        /// <param name="publicAlgosRepository">The public algos repository.</param>
        /// <param name="personalDataService">The personal Data Service</param>
        /// <param name="log">The log.</param>
        /// <param name="codeBuildService">Algo code validator</param>
        public AlgosService(IAlgoRepository algoRepository,
            IAlgoBlobRepository blobRepository,
            IAlgoClientInstanceRepository instanceRepository,
            IAlgoRatingsRepository ratingsRepository,
            IPublicAlgosRepository publicAlgosRepository,
            IPersonalDataService personalDataService,
            ILog log,
            ICodeBuildService codeBuildService) : base(log, nameof(AlgosService))
        {
            _algoRepository = algoRepository;
            _blobRepository = blobRepository;
            _instanceRepository = instanceRepository;
            _ratingsRepository = ratingsRepository;
            _publicAlgosRepository = publicAlgosRepository;
            _personalDataService = personalDataService;
            _codeBuildService = codeBuildService;
        }

        /// <summary>
        /// Gets all algos with rating asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<List<AlgoRatingMetaData>> GetAllAlgosWithRatingAsync()
        {
            return await LogTimedInfoAsync(nameof(GetAllAlgosWithRatingAsync), null, async () =>
            {
                var result = new List<AlgoRatingMetaData>();

                var algos = await _publicAlgosRepository.GetAllPublicAlgosAsync();

                if (algos.IsNullOrEmptyCollection())
                    return result;

                foreach (var publicAlgo in algos)
                {
                    var currentAlgo = await _algoRepository.GetAlgoAsync(publicAlgo.ClientId, publicAlgo.AlgoId);

                    if (currentAlgo == null)
                        continue;

                    string authorName;

                    if (String.IsNullOrEmpty(currentAlgo.ClientId))
                        authorName = "Administrator";
                    else
                    {
                        var authorPersonalData = await _personalDataService.GetAsync(currentAlgo.ClientId);
                        authorName = !String.IsNullOrEmpty(authorPersonalData.FullName)
                                                        ? authorPersonalData.FullName
                                                        : authorPersonalData.Email;
                    }

                    var ratingMetaData = new AlgoRatingMetaData
                    {
                        ClientId = currentAlgo.ClientId,
                        AlgoId = currentAlgo.AlgoId,
                        Name = currentAlgo.Name,
                        Description = currentAlgo.Description,
                        Date = currentAlgo.Date,
                        Author = authorName
                    };

                    var rating = await _ratingsRepository.GetAlgoRatingsAsync(currentAlgo.AlgoId);
                    if (rating != null && rating.Count > 0)
                    {
                        ratingMetaData.Rating = Math.Round(rating.Average(item => item.Rating), 2);
                        ratingMetaData.RatedUsersCount = rating.Count;
                    }
                    else
                    {
                        ratingMetaData.Rating = 0;
                        ratingMetaData.RatedUsersCount = 0;
                    }

                    ratingMetaData.UsersCount = rnd.Next(1, 500); // TODO hardcoded until real count is displayed                        
                    result.Add(ratingMetaData);
                }

                return result;
            });
        }

        /// <summary>
        /// Save the rating for a specific algo for a specific user
        /// </summary>
        /// <param name="data">Ratings data</param>
        /// <returns></returns>
        public async Task<AlgoRatingData> SaveAlgoRatingAsync(AlgoRatingData data)
        {
            return await LogTimedInfoAsync(nameof(SaveAlgoRatingAsync), data.ClientId, async () =>
            {
                Check.IsEmpty(data.ClientId, nameof(data.ClientId));
                Check.IsEmpty(data.AlgoId, nameof(data.AlgoId));

                if (double.IsNaN(data.Rating))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        string.Format(Phrases.ParamInvalid, "rating"),
                        string.Format(Phrases.ParamInvalid, "rating"));
                }

                await _ratingsRepository.SaveAlgoRatingAsync(data);

                var newRatingInformation = await _ratingsRepository.GetAlgoRatingsAsync(data.AlgoId);
                var newRatingData = new AlgoRatingData
                {
                    AlgoId = data.AlgoId,
                    ClientId = data.ClientId,
                    RatedUsersCount = newRatingInformation.Count,
                    Rating = Math.Round(newRatingInformation.Average(item => item.Rating), 2)
                };

                return newRatingData;
            });
        }

        /// <summary>
        /// Get the rating of a specific client for an algo
        /// </summary>
        /// <param name="algoId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<AlgoRatingData> GetAlgoRatingForClientAsync(string algoId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoRatingForClientAsync), clientId, async () =>
            {
                Check.IsEmpty(clientId, nameof(clientId));
                Check.IsEmpty(algoId, nameof(algoId));

                var result = await _ratingsRepository.GetAlgoRatingForClientAsync(algoId, clientId);

                return result;
            });
        }

        /// <summary>
        /// Get the average rating for an algo
        /// </summary>
        /// <param name="algoId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<AlgoRatingData> GetAlgoRatingAsync(string algoId, string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoRatingAsync), clientId, async () =>
            {
                Check.IsEmpty(algoId, nameof(algoId));

                var ratings = await _ratingsRepository.GetAlgoRatingsAsync(algoId);

                var result = new AlgoRatingData
                {
                    AlgoId = algoId,
                    ClientId = clientId,
                };


                if (ratings != null && ratings.Count > 0)
                {
                    result.Rating = Math.Round(ratings.Average(item => item.Rating), 2);
                    result.RatedUsersCount = ratings.Count;
                }
                else
                {
                    result.Rating = 0;
                    result.RatedUsersCount = 0;
                }

                return result;
            });
        }

        /// <summary>
        /// Create an algo from provided code
        /// </summary>
        /// <param name="data">Algo data</param>
        /// <param name="algoContent">Algo code content</param>
        /// <returns></returns>
        public async Task<AlgoData> CreateAlgoAsync(AlgoData data, string algoContent)
        {
            return await LogTimedInfoAsync(nameof(CreateAlgoAsync), data.ClientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.ClientIdEmpty,
                        Phrases.CreateAlgoFailedOnValidationDisplayMessage);

                if (string.IsNullOrEmpty(algoContent))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.AlgoContentEmpty,
                        Phrases.CreateAlgoFailedOnValidationDisplayMessage);

                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    data.AlgoId = Guid.NewGuid().ToString();

                if (!data.ValidateData(out var exception))
                    throw exception.ToBaseException(Phrases.CreateAlgoFailedOnValidationDisplayMessage);

                //Validate algo code
                var validationSession = _codeBuildService.StartSession(algoContent);
                var validationResult = await validationSession.Validate();

                if (!validationResult.IsSuccessful)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        string.Format(Phrases.AlgoDataSaveFailedOnCodeValidation, Environment.NewLine, data.ClientId,
                            data.AlgoId, validationResult),
                        string.Format(Phrases.CreateAlgoFailedOnCodeValidationDisplayMessage, validationResult));

                //Extract algo metadata (parameters)
                var extractedMetadata = await validationSession.ExtractMetadata();

                data.AlgoMetaDataInformationJSON = JsonConvert.SerializeObject(extractedMetadata);

                var algoToSave = AutoMapper.Mapper.Map<IAlgo>(data);

                await _algoRepository.SaveAlgoAsync(algoToSave);

                var res = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                if (res == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.InternalError,
                        string.Format(Phrases.AlgoDataSaveFailed, data.ClientId, data.AlgoId),
                        Phrases.CreateAlgoFailedOnDataSaveDisplayMessage);

                await _blobRepository.SaveBlobAsync(data.AlgoId, algoContent);

                return AutoMapper.Mapper.Map<AlgoData>(res);
            });
        }

        /// <summary>
        /// Edit (update) an algo from provided code
        /// </summary>
        /// <param name="data">Algo data</param>
        /// <param name="algoContent">Algo code content</param>
        /// <returns>Algo data that is updated</returns>
        public async Task<AlgoData> EditAlgoAsync(AlgoData data, string algoContent)
        {
            return await LogTimedInfoAsync(nameof(CreateAlgoAsync), data.ClientId, async () =>
            {
                if (string.IsNullOrWhiteSpace(data.ClientId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.ClientIdEmpty,
                        Phrases.EditAlgoFailedOnValidationDisplayMessage);

                if (string.IsNullOrEmpty(algoContent))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.AlgoContentEmpty,
                        Phrases.EditAlgoFailedOnValidationDisplayMessage);

                if (string.IsNullOrWhiteSpace(data.AlgoId))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.AlgoIdEmpty,
                        Phrases.EditAlgoFailedOnValidationDisplayMessage);

                if (!data.ValidateData(out var exception))
                    throw exception.ToBaseException(Phrases.EditAlgoFailedOnValidationDisplayMessage);

                var res = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                //Algo should not be public in order to edit it
                if (res == null || res.AlgoVisibility == AlgoVisibility.Public)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        string.Format(Phrases.NoAlgoData, data.ClientId, data.AlgoId),
                        Phrases.NoAlgoDataDisplayMessage);

                //Check if there are running algo instances
                var instances = await _instanceRepository.GetAllAlgoInstancesByAlgoAsync(data.AlgoId);

                if (instances.Any(x => x.AlgoInstanceStatus == AlgoInstanceStatus.Started))
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError, Phrases.RunningAlgoInstanceExists,
                        Phrases.RunningAlgoInstanceExistsDisplayMessage);

                //Validate algo code
                var validationSession = _codeBuildService.StartSession(algoContent);
                var validationResult = await validationSession.Validate();

                if (!validationResult.IsSuccessful)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                        string.Format(Phrases.AlgoDataSaveFailedOnCodeValidation, Environment.NewLine, data.ClientId,
                            data.AlgoId, validationResult),
                        string.Format(Phrases.EditAlgoFailedOnCodeValidationDisplayMessage, validationResult));

                //Extract algo metadata (parameters)
                var extractedMetadata = await validationSession.ExtractMetadata();

                data.AlgoMetaDataInformationJSON = JsonConvert.SerializeObject(extractedMetadata);

                var algoToSave = AutoMapper.Mapper.Map<IAlgo>(data);

                await _algoRepository.SaveAlgoAsync(algoToSave);
                await _blobRepository.SaveBlobAsync(data.AlgoId, algoContent);

                res = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                return AutoMapper.Mapper.Map<AlgoData>(res);
            });
        }

        /// <summary>
        /// Gets all the user Algos asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public async Task<List<AlgoData>> GetAllUserAlgosAsync(string clientId)
        {
            return await LogTimedInfoAsync(nameof(GetAllUserAlgosAsync), clientId, async () =>
            {
                Check.IsEmpty(clientId, nameof(clientId));

                var userAlgos = await _algoRepository.GetAllClientAlgosAsync(clientId);

                var userAlgosresult = AutoMapper.Mapper.Map<List<AlgoData>>(userAlgos);

                return userAlgosresult;
            });
        }

        /// <summary>
        /// Gets the information for an algo
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <param name="algoId">The algo id</param>
        /// <returns></returns>
        public async Task<AlgoDataInformation> GetAlgoDataInformationAsync(string clientId, string algoId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoDataInformationAsync), clientId, async () =>
            {
                Check.IsEmpty(clientId, nameof(clientId));
                Check.IsEmpty(algoId, nameof(algoId));

                var algoInformation = await _algoRepository.GetAlgoDataInformationAsync(clientId, algoId);

                var rating = await _ratingsRepository.GetAlgoRatingsAsync(algoId);

                if (algoInformation != null)
                {

                    if (rating != null && rating.Count > 0)
                    {
                        algoInformation.Rating = Math.Round(rating.Average(item => item.Rating), 2);
                        algoInformation.RatedUsersCount = rating.Count;
                    }
                    else
                    {
                        algoInformation.Rating = 0;
                        algoInformation.RatedUsersCount = 0;
                    }

                    algoInformation.UsersCount = rnd.Next(1, 500); // TODO hardcoded until real count is displayed                        

                    algoInformation.Author = (await _personalDataService.GetAsync(clientId))?.FullName;
                }
                return algoInformation;
            });
        }

        /// <summary>
        /// Adds algo to public algos asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="clientId">The id of the logget user</param>
        /// <returns></returns>
        public async Task<PublicAlgoData> AddToPublicAsync(PublicAlgoData data, string clientId)
        {
            return await LogTimedInfoAsync(nameof(AddToPublicAsync), clientId, async () =>
            {
                Check.IsEmpty(data.ClientId, nameof(data.ClientId));
                Check.IsEmpty(data.AlgoId, nameof(data.AlgoId));
                if (data.ClientId != clientId)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.Unauthorized,
                        $"User with id {clientId} cannot publish algo because he/she is not the author.",
                        Phrases.UserNotAuthorOfAlgo);

                await _publicAlgosRepository.SavePublicAlgoAsync(data);

                var algo = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                algo.AlgoVisibility = AlgoVisibility.Public;

                await _algoRepository.SaveAlgoAsync(algo);

                return data;
            });
        }

        /// <summary>
        /// Remove algo from public algos asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="clientId">The id of the logget user</param>
        /// <returns></returns>
        public async Task<PublicAlgoData> RemoveFromPublicAsync(PublicAlgoData data, string clientId)
        {
            return await LogTimedInfoAsync(nameof(RemoveFromPublicAsync), clientId, async () =>
            {
                Check.IsEmpty(data.ClientId, nameof(data.ClientId));
                Check.IsEmpty(data.AlgoId, nameof(data.AlgoId));
                if (data.ClientId != clientId)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.Unauthorized,
                        $"User with id {clientId} cannot unpublish algo because he/she is not the author.",
                        Phrases.UserNotAuthorOfAlgo);

                bool algoExists = await _publicAlgosRepository.ExistsPublicAlgoAsync(data.ClientId, data.AlgoId);

                if (!algoExists)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotPublic,
                        $"There is no public algo with AlgoId {data.AlgoId}, Client id {data.ClientId}",
                        Phrases.PublicAlgoNotFound);

                var algoInstances = await _instanceRepository.GetAllAlgoInstancesByAlgoAsync(data.AlgoId);

                if (algoInstances.Any())
                    throw new AlgoStoreException(AlgoStoreErrorCodes.UnableToDeleteData,
                        $"Cannot unpublish algo because it has algo instances. Algo id {data.AlgoId}, Client id {data.ClientId}",
                        Phrases.AlgoInstancesExist);

                await _publicAlgosRepository.DeletePublicAlgoAsync(data);

                var algo = await _algoRepository.GetAlgoAsync(data.ClientId, data.AlgoId);

                algo.AlgoVisibility = AlgoVisibility.Private;

                await _algoRepository.SaveAlgoAsync(algo);

                return data;
            });
        }

        /// <summary>
        /// Deletes the algo asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task DeleteAsync(ManageImageData data)
        {
            await LogTimedInfoAsync(nameof(DeleteAsync), data?.ClientId, async () =>
            {
                if (await _publicAlgosRepository.ExistsPublicAlgoAsync(data.ClientId, data.AlgoId) ||
                   await _instanceRepository.HasInstanceData(data.ClientId, data.AlgoId))
                    return;

                if (await _blobRepository.BlobExistsAsync(data.AlgoId))
                    await _blobRepository.DeleteBlobAsync(data.AlgoId);

                await _algoRepository.DeleteAlgoAsync(data.ClientId, data.AlgoId);
            });
        }

        /// <summary>
        /// Saves the algo as binary asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="dataModel">The data model.</param>
        /// <returns></returns>
        public async Task SaveAlgoAsBinaryAsync(string clientId, UploadAlgoBinaryData dataModel)
        {
            await LogTimedInfoAsync(nameof(SaveAlgoAsBinaryAsync), clientId, async () =>
            {
                if (!dataModel.ValidateData(out var exception))
                    throw exception;

                var algo = await _algoRepository.GetAlgoAsync(clientId, dataModel.AlgoId);
                if (algo == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {dataModel.AlgoId} is not found! Cant save file for a non existing algo.",
                        string.Format(Phrases.PublicAlgoNotFound, "algo"));

                await _blobRepository.SaveBlobAsync(dataModel.AlgoId, dataModel.Data.OpenReadStream());
            });
        }
        /// <summary>
        /// Saves the algo as string asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="dataModel">The data model.</param>
        /// <returns></returns>
        public async Task SaveAlgoAsStringAsync(string clientId, UploadAlgoStringData dataModel)
        {
            await LogTimedInfoAsync(nameof(SaveAlgoAsStringAsync), clientId, async () =>
            {
                if (!dataModel.ValidateData(out var exception))
                    throw exception;

                var algo = await _algoRepository.GetAlgoAsync(clientId, dataModel.AlgoId);
                if (algo == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {dataModel.AlgoId} is not found! Can't save string for a non existing algo.",
                        string.Format(Phrases.PublicAlgoNotFound, "algo"));

                await _blobRepository.SaveBlobAsync(dataModel.AlgoId, dataModel.Data);
            });
        }
        /// <summary>
        /// Gets the algo as string asynchronous.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="algoId">The algo identifier.</param>
        /// <returns></returns>
        public async Task<string> GetAlgoAsStringAsync(string clientId, string algoId)
        {
            return await LogTimedInfoAsync(nameof(GetAlgoAsStringAsync), clientId, async () =>
            {
                Check.IsEmpty(clientId, nameof(clientId));
                Check.IsEmpty(algoId, nameof(algoId));

                var algo = await _algoRepository.GetAlgoAsync(clientId, algoId);
                if (algo == null)
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"Specified algo id {algoId} is not found!",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "algo"));

                return await _blobRepository.GetBlobStringAsync(algoId);
            });
        }   
    }
}
