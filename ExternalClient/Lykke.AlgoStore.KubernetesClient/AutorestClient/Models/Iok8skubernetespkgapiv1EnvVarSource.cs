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
    /// EnvVarSource represents a source for the value of an EnvVar.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1EnvVarSource
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1EnvVarSource class.
        /// </summary>
        public Iok8skubernetespkgapiv1EnvVarSource()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1EnvVarSource class.
        /// </summary>
        /// <param name="configMapKeyRef">Selects a key of a ConfigMap.</param>
        /// <param name="fieldRef">Selects a field of the pod: supports
        /// metadata.name, metadata.namespace, metadata.labels,
        /// metadata.annotations, spec.nodeName, spec.serviceAccountName,
        /// status.hostIP, status.podIP.</param>
        /// <param name="resourceFieldRef">Selects a resource of the container:
        /// only resources limits and requests (limits.cpu, limits.memory,
        /// requests.cpu and requests.memory) are currently supported.</param>
        /// <param name="secretKeyRef">Selects a key of a secret in the pod's
        /// namespace</param>
        public Iok8skubernetespkgapiv1EnvVarSource(Iok8skubernetespkgapiv1ConfigMapKeySelector configMapKeyRef = default(Iok8skubernetespkgapiv1ConfigMapKeySelector), Iok8skubernetespkgapiv1ObjectFieldSelector fieldRef = default(Iok8skubernetespkgapiv1ObjectFieldSelector), Iok8skubernetespkgapiv1ResourceFieldSelector resourceFieldRef = default(Iok8skubernetespkgapiv1ResourceFieldSelector), Iok8skubernetespkgapiv1SecretKeySelector secretKeyRef = default(Iok8skubernetespkgapiv1SecretKeySelector))
        {
            ConfigMapKeyRef = configMapKeyRef;
            FieldRef = fieldRef;
            ResourceFieldRef = resourceFieldRef;
            SecretKeyRef = secretKeyRef;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets selects a key of a ConfigMap.
        /// </summary>
        [JsonProperty(PropertyName = "configMapKeyRef")]
        public Iok8skubernetespkgapiv1ConfigMapKeySelector ConfigMapKeyRef { get; set; }

        /// <summary>
        /// Gets or sets selects a field of the pod: supports metadata.name,
        /// metadata.namespace, metadata.labels, metadata.annotations,
        /// spec.nodeName, spec.serviceAccountName, status.hostIP,
        /// status.podIP.
        /// </summary>
        [JsonProperty(PropertyName = "fieldRef")]
        public Iok8skubernetespkgapiv1ObjectFieldSelector FieldRef { get; set; }

        /// <summary>
        /// Gets or sets selects a resource of the container: only resources
        /// limits and requests (limits.cpu, limits.memory, requests.cpu and
        /// requests.memory) are currently supported.
        /// </summary>
        [JsonProperty(PropertyName = "resourceFieldRef")]
        public Iok8skubernetespkgapiv1ResourceFieldSelector ResourceFieldRef { get; set; }

        /// <summary>
        /// Gets or sets selects a key of a secret in the pod's namespace
        /// </summary>
        [JsonProperty(PropertyName = "secretKeyRef")]
        public Iok8skubernetespkgapiv1SecretKeySelector SecretKeyRef { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ConfigMapKeyRef != null)
            {
                ConfigMapKeyRef.Validate();
            }
            if (FieldRef != null)
            {
                FieldRef.Validate();
            }
            if (ResourceFieldRef != null)
            {
                ResourceFieldRef.Validate();
            }
            if (SecretKeyRef != null)
            {
                SecretKeyRef.Validate();
            }
        }
    }
}
