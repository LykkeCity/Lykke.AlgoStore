// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Adapts a ConfigMap into a volume.
    ///
    /// The contents of the target ConfigMap's Data field will be presented in
    /// a volume as files using the keys in the Data field as the file names,
    /// unless the items element is populated with specific mappings of keys to
    /// paths. ConfigMap volumes support ownership management and SELinux
    /// relabeling.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1ConfigMapVolumeSource
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ConfigMapVolumeSource class.
        /// </summary>
        public Iok8skubernetespkgapiv1ConfigMapVolumeSource()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ConfigMapVolumeSource class.
        /// </summary>
        /// <param name="defaultMode">Optional: mode bits to use on created
        /// files by default. Must be a value between 0 and 0777. Defaults to
        /// 0644. Directories within the path are not affected by this setting.
        /// This might be in conflict with other options that affect the file
        /// mode, like fsGroup, and the result can be other mode bits
        /// set.</param>
        /// <param name="items">If unspecified, each key-value pair in the Data
        /// field of the referenced ConfigMap will be projected into the volume
        /// as a file whose name is the key and content is the value. If
        /// specified, the listed keys will be projected into the specified
        /// paths, and unlisted keys will not be present. If a key is specified
        /// which is not present in the ConfigMap, the volume setup will error
        /// unless it is marked optional. Paths must be relative and may not
        /// contain the '..' path or start with '..'.</param>
        /// <param name="name">Name of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#names</param>
        /// <param name="optional">Specify whether the ConfigMap or it's keys
        /// must be defined</param>
        public Iok8skubernetespkgapiv1ConfigMapVolumeSource(int? defaultMode = default(int?), IList<Iok8skubernetespkgapiv1KeyToPath> items = default(IList<Iok8skubernetespkgapiv1KeyToPath>), string name = default(string), bool? optional = default(bool?))
        {
            DefaultMode = defaultMode;
            Items = items;
            Name = name;
            Optional = optional;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets optional: mode bits to use on created files by
        /// default. Must be a value between 0 and 0777. Defaults to 0644.
        /// Directories within the path are not affected by this setting. This
        /// might be in conflict with other options that affect the file mode,
        /// like fsGroup, and the result can be other mode bits set.
        /// </summary>
        [JsonProperty(PropertyName = "defaultMode")]
        public int? DefaultMode { get; set; }

        /// <summary>
        /// Gets or sets if unspecified, each key-value pair in the Data field
        /// of the referenced ConfigMap will be projected into the volume as a
        /// file whose name is the key and content is the value. If specified,
        /// the listed keys will be projected into the specified paths, and
        /// unlisted keys will not be present. If a key is specified which is
        /// not present in the ConfigMap, the volume setup will error unless it
        /// is marked optional. Paths must be relative and may not contain the
        /// '..' path or start with '..'.
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<Iok8skubernetespkgapiv1KeyToPath> Items { get; set; }

        /// <summary>
        /// Gets or sets name of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#names
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets specify whether the ConfigMap or it's keys must be
        /// defined
        /// </summary>
        [JsonProperty(PropertyName = "optional")]
        public bool? Optional { get; set; }

    }
}
