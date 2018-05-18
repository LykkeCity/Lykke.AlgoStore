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
    /// APIGroupList is a list of APIGroup, to allow clients to discover the
    /// API at /apis.
    /// </summary>
    public partial class Iok8sapimachinerypkgapismetav1APIGroupList
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapimachinerypkgapismetav1APIGroupList class.
        /// </summary>
        public Iok8sapimachinerypkgapismetav1APIGroupList()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapimachinerypkgapismetav1APIGroupList class.
        /// </summary>
        /// <param name="groups">groups is a list of APIGroup.</param>
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
        public Iok8sapimachinerypkgapismetav1APIGroupList(IList<Iok8sapimachinerypkgapismetav1APIGroup> groups, string apiVersion = default(string), string kind = default(string))
        {
            ApiVersion = apiVersion;
            Groups = groups;
            Kind = kind;
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
        /// Gets or sets groups is a list of APIGroup.
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        public IList<Iok8sapimachinerypkgapismetav1APIGroup> Groups { get; set; }

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
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Groups == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Groups");
            }
            if (Groups != null)
            {
                foreach (var element in Groups)
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
