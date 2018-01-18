// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Represents an empty directory for a pod. Empty directory volumes
    /// support ownership management and SELinux relabeling.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1EmptyDirVolumeSource
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1EmptyDirVolumeSource class.
        /// </summary>
        public Iok8skubernetespkgapiv1EmptyDirVolumeSource()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1EmptyDirVolumeSource class.
        /// </summary>
        /// <param name="medium">What type of storage medium should back this
        /// directory. The default is "" which means to use the node's default
        /// medium. Must be an empty string (default) or Memory. More info:
        /// https://kubernetes.io/docs/concepts/storage/volumes#emptydir</param>
        /// <param name="sizeLimit">Total amount of local storage required for
        /// this EmptyDir volume. The size limit is also applicable for memory
        /// medium. The maximum usage on memory medium EmptyDir would be the
        /// minimum value between the SizeLimit specified here and the sum of
        /// memory limits of all containers in a pod. The default is nil which
        /// means that the limit is undefined. More info:
        /// http://kubernetes.io/docs/user-guide/volumes#emptydir</param>
        public Iok8skubernetespkgapiv1EmptyDirVolumeSource(string medium = default(string), string sizeLimit = default(string))
        {
            Medium = medium;
            SizeLimit = sizeLimit;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets what type of storage medium should back this
        /// directory. The default is "" which means to use the node's default
        /// medium. Must be an empty string (default) or Memory. More info:
        /// https://kubernetes.io/docs/concepts/storage/volumes#emptydir
        /// </summary>
        [JsonProperty(PropertyName = "medium")]
        public string Medium { get; set; }

        /// <summary>
        /// Gets or sets total amount of local storage required for this
        /// EmptyDir volume. The size limit is also applicable for memory
        /// medium. The maximum usage on memory medium EmptyDir would be the
        /// minimum value between the SizeLimit specified here and the sum of
        /// memory limits of all containers in a pod. The default is nil which
        /// means that the limit is undefined. More info:
        /// http://kubernetes.io/docs/user-guide/volumes#emptydir
        /// </summary>
        [JsonProperty(PropertyName = "sizeLimit")]
        public string SizeLimit { get; set; }

    }
}
