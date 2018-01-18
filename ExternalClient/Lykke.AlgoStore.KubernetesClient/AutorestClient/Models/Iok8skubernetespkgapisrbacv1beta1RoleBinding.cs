// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// RoleBinding references a role, but does not contain it.  It can
    /// reference a Role in the same namespace or a ClusterRole in the global
    /// namespace. It adds who information via Subjects and namespace
    /// information by which namespace it exists in.  RoleBindings in a given
    /// namespace only have effect in that namespace.
    /// </summary>
    public partial class Iok8skubernetespkgapisrbacv1beta1RoleBinding
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisrbacv1beta1RoleBinding class.
        /// </summary>
        public Iok8skubernetespkgapisrbacv1beta1RoleBinding()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisrbacv1beta1RoleBinding class.
        /// </summary>
        /// <param name="roleRef">RoleRef can reference a Role in the current
        /// namespace or a ClusterRole in the global namespace. If the RoleRef
        /// cannot be resolved, the Authorizer must return an error.</param>
        /// <param name="subjects">Subjects holds references to the objects the
        /// role applies to.</param>
        /// <param name="apiVersion">APIVersion defines the versioned schema of
        /// this representation of an object. Servers should convert recognized
        /// schemas to the latest internal value, and may reject unrecognized
        /// values. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#resources</param>
        /// <param name="kind">Kind is a string value representing the REST
        /// resource this object represents. Servers may infer this from the
        /// endpoint the client submits requests to. Cannot be updated. In
        /// CamelCase. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds</param>
        /// <param name="metadata">Standard object's metadata.</param>
        public Iok8skubernetespkgapisrbacv1beta1RoleBinding(Iok8skubernetespkgapisrbacv1beta1RoleRef roleRef, IList<Iok8skubernetespkgapisrbacv1beta1Subject> subjects, string apiVersion = default(string), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta))
        {
            ApiVersion = apiVersion;
            Kind = kind;
            Metadata = metadata;
            RoleRef = roleRef;
            Subjects = subjects;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets aPIVersion defines the versioned schema of this
        /// representation of an object. Servers should convert recognized
        /// schemas to the latest internal value, and may reject unrecognized
        /// values. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#resources
        /// </summary>
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets kind is a string value representing the REST resource
        /// this object represents. Servers may infer this from the endpoint
        /// the client submits requests to. Cannot be updated. In CamelCase.
        /// More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets standard object's metadata.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Iok8sapimachinerypkgapismetav1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// Gets or sets roleRef can reference a Role in the current namespace
        /// or a ClusterRole in the global namespace. If the RoleRef cannot be
        /// resolved, the Authorizer must return an error.
        /// </summary>
        [JsonProperty(PropertyName = "roleRef")]
        public Iok8skubernetespkgapisrbacv1beta1RoleRef RoleRef { get; set; }

        /// <summary>
        /// Gets or sets subjects holds references to the objects the role
        /// applies to.
        /// </summary>
        [JsonProperty(PropertyName = "subjects")]
        public IList<Iok8skubernetespkgapisrbacv1beta1Subject> Subjects { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (RoleRef == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "RoleRef");
            }
            if (Subjects == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Subjects");
            }
            if (Metadata != null)
            {
                Metadata.Validate();
            }
            if (RoleRef != null)
            {
                RoleRef.Validate();
            }
            if (Subjects != null)
            {
                foreach (var element in Subjects)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
