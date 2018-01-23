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
    /// A StatefulSetSpec is the specification of a StatefulSet.
    /// </summary>
    public partial class Iok8skubernetespkgapisappsv1beta1StatefulSetSpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisappsv1beta1StatefulSetSpec class.
        /// </summary>
        public Iok8skubernetespkgapisappsv1beta1StatefulSetSpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisappsv1beta1StatefulSetSpec class.
        /// </summary>
        /// <param name="serviceName">serviceName is the name of the service
        /// that governs this StatefulSet. This service must exist before the
        /// StatefulSet, and is responsible for the network identity of the
        /// set. Pods get DNS/hostnames that follow the pattern:
        /// pod-specific-string.serviceName.default.svc.cluster.local where
        /// "pod-specific-string" is managed by the StatefulSet
        /// controller.</param>
        /// <param name="template">template is the object that describes the
        /// pod that will be created if insufficient replicas are detected.
        /// Each pod stamped out by the StatefulSet will fulfill this Template,
        /// but have a unique identity from the rest of the
        /// StatefulSet.</param>
        /// <param name="podManagementPolicy">podManagementPolicy controls how
        /// pods are created during initial scale up, when replacing pods on
        /// nodes, or when scaling down. The default policy is `OrderedReady`,
        /// where pods are created in increasing order (pod-0, then pod-1, etc)
        /// and the controller will wait until each pod is ready before
        /// continuing. When scaling down, the pods are removed in the opposite
        /// order. The alternative policy is `Parallel` which will create pods
        /// in parallel to match the desired scale without waiting, and on
        /// scale down will delete all pods at once.</param>
        /// <param name="replicas">replicas is the desired number of replicas
        /// of the given Template. These are replicas in the sense that they
        /// are instantiations of the same Template, but individual replicas
        /// also have a consistent identity. If unspecified, defaults to
        /// 1.</param>
        /// <param name="revisionHistoryLimit">revisionHistoryLimit is the
        /// maximum number of revisions that will be maintained in the
        /// StatefulSet's revision history. The revision history consists of
        /// all revisions not represented by a currently applied
        /// StatefulSetSpec version. The default value is 10.</param>
        /// <param name="selector">selector is a label query over pods that
        /// should match the replica count. If empty, defaulted to labels on
        /// the pod template. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors</param>
        /// <param name="updateStrategy">updateStrategy indicates the
        /// StatefulSetUpdateStrategy that will be employed to update Pods in
        /// the StatefulSet when a revision is made to Template.</param>
        /// <param name="volumeClaimTemplates">volumeClaimTemplates is a list
        /// of claims that pods are allowed to reference. The StatefulSet
        /// controller is responsible for mapping network identities to claims
        /// in a way that maintains the identity of a pod. Every claim in this
        /// list must have at least one matching (by name) volumeMount in one
        /// container in the template. A claim in this list takes precedence
        /// over any volumes in the template, with the same name.</param>
        public Iok8skubernetespkgapisappsv1beta1StatefulSetSpec(string serviceName, Iok8skubernetespkgapiv1PodTemplateSpec template, string podManagementPolicy = default(string), int? replicas = default(int?), int? revisionHistoryLimit = default(int?), Iok8sapimachinerypkgapismetav1LabelSelector selector = default(Iok8sapimachinerypkgapismetav1LabelSelector), Iok8skubernetespkgapisappsv1beta1StatefulSetUpdateStrategy updateStrategy = default(Iok8skubernetespkgapisappsv1beta1StatefulSetUpdateStrategy), IList<Iok8skubernetespkgapiv1PersistentVolumeClaim> volumeClaimTemplates = default(IList<Iok8skubernetespkgapiv1PersistentVolumeClaim>))
        {
            PodManagementPolicy = podManagementPolicy;
            Replicas = replicas;
            RevisionHistoryLimit = revisionHistoryLimit;
            Selector = selector;
            ServiceName = serviceName;
            Template = template;
            UpdateStrategy = updateStrategy;
            VolumeClaimTemplates = volumeClaimTemplates;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets podManagementPolicy controls how pods are created
        /// during initial scale up, when replacing pods on nodes, or when
        /// scaling down. The default policy is `OrderedReady`, where pods are
        /// created in increasing order (pod-0, then pod-1, etc) and the
        /// controller will wait until each pod is ready before continuing.
        /// When scaling down, the pods are removed in the opposite order. The
        /// alternative policy is `Parallel` which will create pods in parallel
        /// to match the desired scale without waiting, and on scale down will
        /// delete all pods at once.
        /// </summary>
        [JsonProperty(PropertyName = "podManagementPolicy")]
        public string PodManagementPolicy { get; set; }

        /// <summary>
        /// Gets or sets replicas is the desired number of replicas of the
        /// given Template. These are replicas in the sense that they are
        /// instantiations of the same Template, but individual replicas also
        /// have a consistent identity. If unspecified, defaults to 1.
        /// </summary>
        [JsonProperty(PropertyName = "replicas")]
        public int? Replicas { get; set; }

        /// <summary>
        /// Gets or sets revisionHistoryLimit is the maximum number of
        /// revisions that will be maintained in the StatefulSet's revision
        /// history. The revision history consists of all revisions not
        /// represented by a currently applied StatefulSetSpec version. The
        /// default value is 10.
        /// </summary>
        [JsonProperty(PropertyName = "revisionHistoryLimit")]
        public int? RevisionHistoryLimit { get; set; }

        /// <summary>
        /// Gets or sets selector is a label query over pods that should match
        /// the replica count. If empty, defaulted to labels on the pod
        /// template. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors
        /// </summary>
        [JsonProperty(PropertyName = "selector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector Selector { get; set; }

        /// <summary>
        /// Gets or sets serviceName is the name of the service that governs
        /// this StatefulSet. This service must exist before the StatefulSet,
        /// and is responsible for the network identity of the set. Pods get
        /// DNS/hostnames that follow the pattern:
        /// pod-specific-string.serviceName.default.svc.cluster.local where
        /// "pod-specific-string" is managed by the StatefulSet controller.
        /// </summary>
        [JsonProperty(PropertyName = "serviceName")]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets template is the object that describes the pod that
        /// will be created if insufficient replicas are detected. Each pod
        /// stamped out by the StatefulSet will fulfill this Template, but have
        /// a unique identity from the rest of the StatefulSet.
        /// </summary>
        [JsonProperty(PropertyName = "template")]
        public Iok8skubernetespkgapiv1PodTemplateSpec Template { get; set; }

        /// <summary>
        /// Gets or sets updateStrategy indicates the StatefulSetUpdateStrategy
        /// that will be employed to update Pods in the StatefulSet when a
        /// revision is made to Template.
        /// </summary>
        [JsonProperty(PropertyName = "updateStrategy")]
        public Iok8skubernetespkgapisappsv1beta1StatefulSetUpdateStrategy UpdateStrategy { get; set; }

        /// <summary>
        /// Gets or sets volumeClaimTemplates is a list of claims that pods are
        /// allowed to reference. The StatefulSet controller is responsible for
        /// mapping network identities to claims in a way that maintains the
        /// identity of a pod. Every claim in this list must have at least one
        /// matching (by name) volumeMount in one container in the template. A
        /// claim in this list takes precedence over any volumes in the
        /// template, with the same name.
        /// </summary>
        [JsonProperty(PropertyName = "volumeClaimTemplates")]
        public IList<Iok8skubernetespkgapiv1PersistentVolumeClaim> VolumeClaimTemplates { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ServiceName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ServiceName");
            }
            if (Template == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Template");
            }
            if (Template != null)
            {
                Template.Validate();
            }
            if (VolumeClaimTemplates != null)
            {
                foreach (var element in VolumeClaimTemplates)
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