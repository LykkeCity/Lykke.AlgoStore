// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.DeploymentApiClient
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for GetUsingGET.
    /// </summary>
    public static partial class GetUsingGETExtensions
    {
            /// <summary>
            /// get
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// id
            /// </param>
            public static AlgoService One(this IGetUsingGET operations, long id)
            {
                return operations.OneAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// get
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// id
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<AlgoService> OneAsync(this IGetUsingGET operations, long id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.OneWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// get
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// id
            /// </param>
            public static AlgoTest Two(this IGetUsingGET operations, long id)
            {
                return operations.TwoAsync(id).GetAwaiter().GetResult();
            }

            /// <summary>
            /// get
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// id
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<AlgoTest> TwoAsync(this IGetUsingGET operations, long id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.TwoWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// get
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='name'>
            /// name
            /// </param>
            public static object Three(this IGetUsingGET operations, string name)
            {
                return operations.ThreeAsync(name).GetAwaiter().GetResult();
            }

            /// <summary>
            /// get
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='name'>
            /// name
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> ThreeAsync(this IGetUsingGET operations, string name, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ThreeWithHttpMessagesAsync(name, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}