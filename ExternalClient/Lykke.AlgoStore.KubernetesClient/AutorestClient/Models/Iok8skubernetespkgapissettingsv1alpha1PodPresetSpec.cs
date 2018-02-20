// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// PodPresetSpec is a description of a pod preset.
    /// </summary>
    public partial class Iok8skubernetespkgapissettingsv1alpha1PodPresetSpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapissettingsv1alpha1PodPresetSpec class.
        /// </summary>
        public Iok8skubernetespkgapissettingsv1alpha1PodPresetSpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapissettingsv1alpha1PodPresetSpec class.
        /// </summary>
        /// <param name="env">Env defines the collection of EnvVar to inject
        /// into containers.</param>
        /// <param name="envFrom">EnvFrom defines the collection of
        /// EnvFromSource to inject into containers.</param>
        /// <param name="selector">Selector is a label query over a set of
        /// resources, in this case pods. Required.</param>
        /// <param name="volumeMounts">VolumeMounts defines the collection of
        /// VolumeMount to inject into containers.</param>
        /// <param name="volumes">Volumes defines the collection of Volume to
        /// inject into the pod.</param>
        public Iok8skubernetespkgapissettingsv1alpha1PodPresetSpec(IList<Iok8skubernetespkgapiv1EnvVar> env = default(IList<Iok8skubernetespkgapiv1EnvVar>), IList<Iok8skubernetespkgapiv1EnvFromSource> envFrom = default(IList<Iok8skubernetespkgapiv1EnvFromSource>), Iok8sapimachinerypkgapismetav1LabelSelector selector = default(Iok8sapimachinerypkgapismetav1LabelSelector), IList<Iok8skubernetespkgapiv1VolumeMount> volumeMounts = default(IList<Iok8skubernetespkgapiv1VolumeMount>), IList<Iok8skubernetespkgapiv1Volume> volumes = default(IList<Iok8skubernetespkgapiv1Volume>))
        {
            Env = env;
            EnvFrom = envFrom;
            Selector = selector;
            VolumeMounts = volumeMounts;
            Volumes = volumes;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets env defines the collection of EnvVar to inject into
        /// containers.
        /// </summary>
        [JsonProperty(PropertyName = "env")]
        public IList<Iok8skubernetespkgapiv1EnvVar> Env { get; set; }

        /// <summary>
        /// Gets or sets envFrom defines the collection of EnvFromSource to
        /// inject into containers.
        /// </summary>
        [JsonProperty(PropertyName = "envFrom")]
        public IList<Iok8skubernetespkgapiv1EnvFromSource> EnvFrom { get; set; }

        /// <summary>
        /// Gets or sets selector is a label query over a set of resources, in
        /// this case pods. Required.
        /// </summary>
        [JsonProperty(PropertyName = "selector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector Selector { get; set; }

        /// <summary>
        /// Gets or sets volumeMounts defines the collection of VolumeMount to
        /// inject into containers.
        /// </summary>
        [JsonProperty(PropertyName = "volumeMounts")]
        public IList<Iok8skubernetespkgapiv1VolumeMount> VolumeMounts { get; set; }

        /// <summary>
        /// Gets or sets volumes defines the collection of Volume to inject
        /// into the pod.
        /// </summary>
        [JsonProperty(PropertyName = "volumes")]
        public IList<Iok8skubernetespkgapiv1Volume> Volumes { get; set; }

    }
}
