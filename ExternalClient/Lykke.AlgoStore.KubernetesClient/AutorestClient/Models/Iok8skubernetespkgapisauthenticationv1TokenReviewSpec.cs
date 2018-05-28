// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// TokenReviewSpec is a description of the token authentication request.
    /// </summary>
    public partial class Iok8skubernetespkgapisauthenticationv1TokenReviewSpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisauthenticationv1TokenReviewSpec class.
        /// </summary>
        public Iok8skubernetespkgapisauthenticationv1TokenReviewSpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisauthenticationv1TokenReviewSpec class.
        /// </summary>
        /// <param name="token">Token is the opaque bearer token.</param>
        public Iok8skubernetespkgapisauthenticationv1TokenReviewSpec(string token = default(string))
        {
            Token = token;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets token is the opaque bearer token.
        /// </summary>
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

    }
}
