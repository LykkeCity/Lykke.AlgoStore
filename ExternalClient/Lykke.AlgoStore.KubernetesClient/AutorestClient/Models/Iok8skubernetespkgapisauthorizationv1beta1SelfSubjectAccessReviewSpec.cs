// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// SelfSubjectAccessReviewSpec is a description of the access request.
    /// Exactly one of ResourceAuthorizationAttributes and
    /// NonResourceAuthorizationAttributes must be set
    /// </summary>
    public partial class Iok8skubernetespkgapisauthorizationv1beta1SelfSubjectAccessReviewSpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisauthorizationv1beta1SelfSubjectAccessReviewSpec
        /// class.
        /// </summary>
        public Iok8skubernetespkgapisauthorizationv1beta1SelfSubjectAccessReviewSpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisauthorizationv1beta1SelfSubjectAccessReviewSpec
        /// class.
        /// </summary>
        /// <param name="nonResourceAttributes">NonResourceAttributes describes
        /// information for a non-resource access request</param>
        /// <param name="resourceAttributes">ResourceAuthorizationAttributes
        /// describes information for a resource access request</param>
        public Iok8skubernetespkgapisauthorizationv1beta1SelfSubjectAccessReviewSpec(Iok8skubernetespkgapisauthorizationv1beta1NonResourceAttributes nonResourceAttributes = default(Iok8skubernetespkgapisauthorizationv1beta1NonResourceAttributes), Iok8skubernetespkgapisauthorizationv1beta1ResourceAttributes resourceAttributes = default(Iok8skubernetespkgapisauthorizationv1beta1ResourceAttributes))
        {
            NonResourceAttributes = nonResourceAttributes;
            ResourceAttributes = resourceAttributes;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets nonResourceAttributes describes information for a
        /// non-resource access request
        /// </summary>
        [JsonProperty(PropertyName = "nonResourceAttributes")]
        public Iok8skubernetespkgapisauthorizationv1beta1NonResourceAttributes NonResourceAttributes { get; set; }

        /// <summary>
        /// Gets or sets resourceAuthorizationAttributes describes information
        /// for a resource access request
        /// </summary>
        [JsonProperty(PropertyName = "resourceAttributes")]
        public Iok8skubernetespkgapisauthorizationv1beta1ResourceAttributes ResourceAttributes { get; set; }

    }
}
