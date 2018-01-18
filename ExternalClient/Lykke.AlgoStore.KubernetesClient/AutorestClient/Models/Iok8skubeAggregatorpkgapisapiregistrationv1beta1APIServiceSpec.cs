// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// APIServiceSpec contains information for locating and communicating with
    /// a server. Only https is supported, though you are able to disable
    /// certificate verification.
    /// </summary>
    public partial class Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceSpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceSpec
        /// class.
        /// </summary>
        public Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceSpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceSpec
        /// class.
        /// </summary>
        /// <param name="caBundle">CABundle is a PEM encoded CA bundle which
        /// will be used to validate an API server's serving
        /// certificate.</param>
        /// <param name="groupPriorityMinimum">GroupPriorityMininum is the
        /// priority this group should have at least. Higher priority means
        /// that the group is prefered by clients over lower priority ones.
        /// Note that other versions of this group might specify even higher
        /// GroupPriorityMininum values such that the whole group gets a higher
        /// priority. The primary sort is based on GroupPriorityMinimum,
        /// ordered highest number to lowest (20 before 10). The secondary sort
        /// is based on the alphabetical comparison of the name of the object.
        /// (v1.bar before v1.foo) We'd recommend something like: *.k8s.io
        /// (except extensions) at 18000 and PaaSes (OpenShift, Deis) are
        /// recommended to be in the 2000s</param>
        /// <param name="service">Service is a reference to the service for
        /// this API server.  It must communicate on port 443 If the Service is
        /// nil, that means the handling for the API groupversion is handled
        /// locally on this server. The call will simply delegate to the normal
        /// handler chain to be fulfilled.</param>
        /// <param name="versionPriority">VersionPriority controls the ordering
        /// of this API version inside of its group.  Must be greater than
        /// zero. The primary sort is based on VersionPriority, ordered highest
        /// to lowest (20 before 10). The secondary sort is based on the
        /// alphabetical comparison of the name of the object.  (v1.bar before
        /// v1.foo) Since it's inside of a group, the number can be small,
        /// probably in the 10s.</param>
        /// <param name="group">Group is the API group name this server
        /// hosts</param>
        /// <param name="insecureSkipTLSVerify">InsecureSkipTLSVerify disables
        /// TLS certificate verification when communicating with this server.
        /// This is strongly discouraged.  You should use the CABundle
        /// instead.</param>
        /// <param name="version">Version is the API version this server hosts.
        /// For example, "v1"</param>
        public Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceSpec(byte[] caBundle, int groupPriorityMinimum, Iok8skubeAggregatorpkgapisapiregistrationv1beta1ServiceReference service, int versionPriority, string group = default(string), bool? insecureSkipTLSVerify = default(bool?), string version = default(string))
        {
            CaBundle = caBundle;
            Group = group;
            GroupPriorityMinimum = groupPriorityMinimum;
            InsecureSkipTLSVerify = insecureSkipTLSVerify;
            Service = service;
            Version = version;
            VersionPriority = versionPriority;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets cABundle is a PEM encoded CA bundle which will be used
        /// to validate an API server's serving certificate.
        /// </summary>
        [JsonProperty(PropertyName = "caBundle")]
        public byte[] CaBundle { get; set; }

        /// <summary>
        /// Gets or sets group is the API group name this server hosts
        /// </summary>
        [JsonProperty(PropertyName = "group")]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets groupPriorityMininum is the priority this group should
        /// have at least. Higher priority means that the group is prefered by
        /// clients over lower priority ones. Note that other versions of this
        /// group might specify even higher GroupPriorityMininum values such
        /// that the whole group gets a higher priority. The primary sort is
        /// based on GroupPriorityMinimum, ordered highest number to lowest (20
        /// before 10). The secondary sort is based on the alphabetical
        /// comparison of the name of the object.  (v1.bar before v1.foo) We'd
        /// recommend something like: *.k8s.io (except extensions) at 18000 and
        /// PaaSes (OpenShift, Deis) are recommended to be in the 2000s
        /// </summary>
        [JsonProperty(PropertyName = "groupPriorityMinimum")]
        public int GroupPriorityMinimum { get; set; }

        /// <summary>
        /// Gets or sets insecureSkipTLSVerify disables TLS certificate
        /// verification when communicating with this server. This is strongly
        /// discouraged.  You should use the CABundle instead.
        /// </summary>
        [JsonProperty(PropertyName = "insecureSkipTLSVerify")]
        public bool? InsecureSkipTLSVerify { get; set; }

        /// <summary>
        /// Gets or sets service is a reference to the service for this API
        /// server.  It must communicate on port 443 If the Service is nil,
        /// that means the handling for the API groupversion is handled locally
        /// on this server. The call will simply delegate to the normal handler
        /// chain to be fulfilled.
        /// </summary>
        [JsonProperty(PropertyName = "service")]
        public Iok8skubeAggregatorpkgapisapiregistrationv1beta1ServiceReference Service { get; set; }

        /// <summary>
        /// Gets or sets version is the API version this server hosts.  For
        /// example, "v1"
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets versionPriority controls the ordering of this API
        /// version inside of its group.  Must be greater than zero. The
        /// primary sort is based on VersionPriority, ordered highest to lowest
        /// (20 before 10). The secondary sort is based on the alphabetical
        /// comparison of the name of the object.  (v1.bar before v1.foo) Since
        /// it's inside of a group, the number can be small, probably in the
        /// 10s.
        /// </summary>
        [JsonProperty(PropertyName = "versionPriority")]
        public int VersionPriority { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (CaBundle == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "CaBundle");
            }
            if (Service == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Service");
            }
        }
    }
}
