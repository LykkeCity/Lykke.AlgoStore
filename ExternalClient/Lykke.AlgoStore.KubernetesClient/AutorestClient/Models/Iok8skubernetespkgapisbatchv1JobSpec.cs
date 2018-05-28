// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;

    /// <summary>
    /// JobSpec describes how the job execution will look like.
    /// </summary>
    public partial class Iok8skubernetespkgapisbatchv1JobSpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisbatchv1JobSpec class.
        /// </summary>
        public Iok8skubernetespkgapisbatchv1JobSpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisbatchv1JobSpec class.
        /// </summary>
        /// <param name="template">Describes the pod that will be created when
        /// executing a job. More info:
        /// https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/</param>
        /// <param name="activeDeadlineSeconds">Optional duration in seconds
        /// relative to the startTime that the job may be active before the
        /// system tries to terminate it; value must be positive
        /// integer</param>
        /// <param name="completions">Specifies the desired number of
        /// successfully finished pods the job should be run with.  Setting to
        /// nil means that the success of any pod signals the success of all
        /// pods, and allows parallelism to have any positive value.  Setting
        /// to 1 means that parallelism is limited to 1 and the success of that
        /// pod signals the success of the job. More info:
        /// https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/</param>
        /// <param name="manualSelector">manualSelector controls generation of
        /// pod labels and pod selectors. Leave `manualSelector` unset unless
        /// you are certain what you are doing. When false or unset, the system
        /// pick labels unique to this job and appends those labels to the pod
        /// template.  When true, the user is responsible for picking unique
        /// labels and specifying the selector.  Failure to pick a unique label
        /// may cause this and other jobs to not function correctly.  However,
        /// You may see `manualSelector=true` in jobs that were created with
        /// the old `extensions/v1beta1` API. More info:
        /// https://git.k8s.io/community/contributors/design-proposals/selector-generation.md</param>
        /// <param name="parallelism">Specifies the maximum desired number of
        /// pods the job should run at any given time. The actual number of
        /// pods running in steady state will be less than this number when
        /// ((.spec.completions - .status.successful) &lt; .spec.parallelism),
        /// i.e. when the work left to do is less than max parallelism. More
        /// info:
        /// https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/</param>
        /// <param name="selector">A label query over pods that should match
        /// the pod count. Normally, the system sets this field for you. More
        /// info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors</param>
        public Iok8skubernetespkgapisbatchv1JobSpec(Iok8skubernetespkgapiv1PodTemplateSpec template, long? activeDeadlineSeconds = default(long?), int? completions = default(int?), bool? manualSelector = default(bool?), int? parallelism = default(int?), Iok8sapimachinerypkgapismetav1LabelSelector selector = default(Iok8sapimachinerypkgapismetav1LabelSelector))
        {
            ActiveDeadlineSeconds = activeDeadlineSeconds;
            Completions = completions;
            ManualSelector = manualSelector;
            Parallelism = parallelism;
            Selector = selector;
            Template = template;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets optional duration in seconds relative to the startTime
        /// that the job may be active before the system tries to terminate it;
        /// value must be positive integer
        /// </summary>
        [JsonProperty(PropertyName = "activeDeadlineSeconds")]
        public long? ActiveDeadlineSeconds { get; set; }

        /// <summary>
        /// Gets or sets specifies the desired number of successfully finished
        /// pods the job should be run with.  Setting to nil means that the
        /// success of any pod signals the success of all pods, and allows
        /// parallelism to have any positive value.  Setting to 1 means that
        /// parallelism is limited to 1 and the success of that pod signals the
        /// success of the job. More info:
        /// https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/
        /// </summary>
        [JsonProperty(PropertyName = "completions")]
        public int? Completions { get; set; }

        /// <summary>
        /// Gets or sets manualSelector controls generation of pod labels and
        /// pod selectors. Leave `manualSelector` unset unless you are certain
        /// what you are doing. When false or unset, the system pick labels
        /// unique to this job and appends those labels to the pod template.
        /// When true, the user is responsible for picking unique labels and
        /// specifying the selector.  Failure to pick a unique label may cause
        /// this and other jobs to not function correctly.  However, You may
        /// see `manualSelector=true` in jobs that were created with the old
        /// `extensions/v1beta1` API. More info:
        /// https://git.k8s.io/community/contributors/design-proposals/selector-generation.md
        /// </summary>
        [JsonProperty(PropertyName = "manualSelector")]
        public bool? ManualSelector { get; set; }

        /// <summary>
        /// Gets or sets specifies the maximum desired number of pods the job
        /// should run at any given time. The actual number of pods running in
        /// steady state will be less than this number when ((.spec.completions
        /// - .status.successful) &amp;lt; .spec.parallelism), i.e. when the
        /// work left to do is less than max parallelism. More info:
        /// https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/
        /// </summary>
        [JsonProperty(PropertyName = "parallelism")]
        public int? Parallelism { get; set; }

        /// <summary>
        /// Gets or sets a label query over pods that should match the pod
        /// count. Normally, the system sets this field for you. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors
        /// </summary>
        [JsonProperty(PropertyName = "selector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector Selector { get; set; }

        /// <summary>
        /// Gets or sets describes the pod that will be created when executing
        /// a job. More info:
        /// https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/
        /// </summary>
        [JsonProperty(PropertyName = "template")]
        public Iok8skubernetespkgapiv1PodTemplateSpec Template { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Template == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Template");
            }
            if (Template != null)
            {
                Template.Validate();
            }
        }
    }
}
