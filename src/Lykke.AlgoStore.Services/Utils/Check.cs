using System;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Services.Strings;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services.Utils
{
    /// <summary>
    /// Contains common validations used across the system
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Validates that a string parameter is not empty
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        /// <param name="parameterName">The parameter name to use for the error</param>
        public static void IsEmpty(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new AlgoStoreException(AlgoStoreErrorCodes.ValidationError,
                    string.Format(Phrases.StringParameterMissing, parameterName),
                    Phrases.StringParameterMissingDisplayMessage);
            }
        }

        /// <summary>
        /// Contains common validations related to algos
        /// </summary>
        public static class Algo
        {
            /// <summary>
            /// Verifies that an algo exists
            /// </summary>
            /// <param name="repository">The repository to check for the algo</param>
            /// <param name="clientId">The algo owner ID</param>
            /// <param name="algoId">The algo ID</param>
            /// <returns></returns>
            public static async Task Exists(IAlgoReadOnlyRepository repository, string algoId)
            {
                if (!await repository.ExistsAlgoAsync(algoId))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotFound,
                        $"No algo for id {algoId}",
                        string.Format(Phrases.ParamNotFoundDisplayMessage, "algo"));
                }
            }

            /// <summary>
            /// Verifies that a user can access a given algo
            /// </summary>
            /// <param name="repository">The repository to check for algo visibility</param>
            /// <param name="algoRepository">The repository to get algo by algo id</param>
            /// <param name="algoId">The algo ID</param>
            /// <param name="clientId">The client ID to verify</param>
            /// <returns></returns>
            public static async Task IsVisibleForClient(
                IPublicAlgosRepository repository,
                IAlgoRepository algoRepository,
                string algoId,
                string clientId)
            {
                var algo = await algoRepository.GetAlgoByAlgoIdAsync(algoId);

                if (algo != null && algo.ClientId != clientId &&
                    !await repository.ExistsPublicAlgoAsync(algo.ClientId, algoId))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.NotFound,
                        $"Algo {algo.ClientId} not public for client {clientId}",
                        Phrases.NotFoundAlgo);
                }
            }
        }

        /// <summary>
        /// Contains common validations related to algo instances
        /// </summary>
        public static class AlgoInstance
        {
            /// <summary>
            /// Validate available instances count of the user.
            /// </summary>
            /// <param name="repository">The repository to check for the algo instances</param>
            /// <param name="clientId">The algo owner ID</param>
            /// <param name="algoId">The algo ID</param>
            public static async Task InstancesOverDeploymentLimit(IAlgoClientInstanceRepository repository,
                string clientId)
            {
                var count = (await repository.GetAllAlgoInstancesByClientAsync(clientId)).Count(i =>
                    i.AlgoInstanceStatus == AlgoInstanceStatus.Started
                    || i.AlgoInstanceStatus == AlgoInstanceStatus.Deploying);

                if (count >= AlgoStoreConstants.RunningAlgoInstancesCountLimit)
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoInstancesCountLimit,
                        string.Format(Phrases.NotAvailableCreationOfInstances, count, clientId),
                        Phrases.LimitOfRunningInsatcnesReached);
                }
            }
        }

        /// <summary>
        /// Check if provided date is in the past
        /// </summary>
        /// <param name="dateToCheck">Date to check</param>
        /// <param name="justCheckDatePart">If set to TRUE, ONLY date part is checked. Otherwise, both date and time parts of provided date are checked</param>
        /// <returns>TRUE is provided date is in the past. otherwise FALSE</returns>
        public static bool IsDateInThePast(DateTime dateToCheck, bool justCheckDatePart = false)
        {
            return justCheckDatePart ? DateTime.UtcNow.Date > dateToCheck.Date : DateTime.UtcNow > dateToCheck;
        }
    }
}
