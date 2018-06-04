﻿using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Services.Strings;
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
            public static async Task Exists(IAlgoReadOnlyRepository repository, string clientId, string algoId)
            {
                if (!await repository.ExistsAlgoAsync(clientId, algoId))
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
            /// <param name="algoId">The algo ID</param>
            /// <param name="clientId">The client ID to verify</param>
            /// <param name="algoOwnerId">The algo owner ID</param>
            /// <returns></returns>
            public static async Task IsVisibleForClient(
                IPublicAlgosRepository repository, 
                string algoId, 
                string clientId,
                string algoOwnerId)
            {
                if (algoOwnerId != clientId && !await repository.ExistsPublicAlgoAsync(algoOwnerId, algoId))
                {
                    throw new AlgoStoreException(AlgoStoreErrorCodes.AlgoNotPublic,
                        $"Algo {algoOwnerId} not public for client {clientId}",
                        Phrases.UserCantSeeAlgo);
                }
            }
        }
    }
}