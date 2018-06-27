// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// SELinuxOptions are the labels to be applied to the container
    /// </summary>
    public partial class Iok8skubernetespkgapiv1SELinuxOptions
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1SELinuxOptions class.
        /// </summary>
        public Iok8skubernetespkgapiv1SELinuxOptions()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1SELinuxOptions class.
        /// </summary>
        /// <param name="level">Level is SELinux level label that applies to
        /// the container.</param>
        /// <param name="role">Role is a SELinux role label that applies to the
        /// container.</param>
        /// <param name="type">Type is a SELinux type label that applies to the
        /// container.</param>
        /// <param name="user">User is a SELinux user label that applies to the
        /// container.</param>
        public Iok8skubernetespkgapiv1SELinuxOptions(string level = default(string), string role = default(string), string type = default(string), string user = default(string))
        {
            Level = level;
            Role = role;
            Type = type;
            User = user;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets level is SELinux level label that applies to the
        /// container.
        /// </summary>
        [JsonProperty(PropertyName = "level")]
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets role is a SELinux role label that applies to the
        /// container.
        /// </summary>
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets type is a SELinux type label that applies to the
        /// container.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets user is a SELinux user label that applies to the
        /// container.
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }

    }
}