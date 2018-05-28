// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// ObjectReference contains enough information to let you inspect or
    /// modify the referred object.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1ObjectReference
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ObjectReference class.
        /// </summary>
        public Iok8skubernetespkgapiv1ObjectReference()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ObjectReference class.
        /// </summary>
        /// <param name="apiVersion">API version of the referent.</param>
        /// <param name="fieldPath">If referring to a piece of an object
        /// instead of an entire object, this string should contain a valid
        /// JSON/Go field access statement, such as
        /// desiredState.manifest.containers[2]. For example, if the object
        /// reference is to a container within a pod, this would take on a
        /// value like: "spec.containers{name}" (where "name" refers to the
        /// name of the container that triggered the event) or if no container
        /// name is specified "spec.containers[2]" (container with index 2 in
        /// this pod). This syntax is chosen only to have some well-defined way
        /// of referencing a part of an object.</param>
        /// <param name="kind">Kind of the referent. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds</param>
        /// <param name="name">Name of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#names</param>
        /// <param name="namespaceProperty">Namespace of the referent. More
        /// info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/</param>
        /// <param name="resourceVersion">Specific resourceVersion to which
        /// this reference is made, if any. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#concurrency-control-and-consistency</param>
        /// <param name="uid">UID of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#uids</param>
        public Iok8skubernetespkgapiv1ObjectReference(string apiVersion = default(string), string fieldPath = default(string), string kind = default(string), string name = default(string), string namespaceProperty = default(string), string resourceVersion = default(string), string uid = default(string))
        {
            ApiVersion = apiVersion;
            FieldPath = fieldPath;
            Kind = kind;
            Name = name;
            NamespaceProperty = namespaceProperty;
            ResourceVersion = resourceVersion;
            Uid = uid;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets API version of the referent.
        /// </summary>
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets if referring to a piece of an object instead of an
        /// entire object, this string should contain a valid JSON/Go field
        /// access statement, such as desiredState.manifest.containers[2]. For
        /// example, if the object reference is to a container within a pod,
        /// this would take on a value like: "spec.containers{name}" (where
        /// "name" refers to the name of the container that triggered the
        /// event) or if no container name is specified "spec.containers[2]"
        /// (container with index 2 in this pod). This syntax is chosen only to
        /// have some well-defined way of referencing a part of an object.
        /// </summary>
        [JsonProperty(PropertyName = "fieldPath")]
        public string FieldPath { get; set; }

        /// <summary>
        /// Gets or sets kind of the referent. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets name of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#names
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets namespace of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/
        /// </summary>
        [JsonProperty(PropertyName = "namespace")]
        public string NamespaceProperty { get; set; }

        /// <summary>
        /// Gets or sets specific resourceVersion to which this reference is
        /// made, if any. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#concurrency-control-and-consistency
        /// </summary>
        [JsonProperty(PropertyName = "resourceVersion")]
        public string ResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets UID of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#uids
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

    }
}
