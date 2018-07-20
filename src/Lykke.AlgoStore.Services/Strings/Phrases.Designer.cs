﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lykke.AlgoStore.Services.Strings {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Phrases {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Phrases() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Lykke.AlgoStore.Services.Strings.Phrases", typeof(Phrases).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo content is empty.
        /// </summary>
        internal static string AlgoContentEmpty {
            get {
                return ResourceManager.GetString("AlgoContentEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot save algo data. ClientId: {0}, AlgoId: {1}.
        /// </summary>
        internal static string AlgoDataSaveFailed {
            get {
                return ResourceManager.GetString("AlgoDataSaveFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot save algo data. Algo code validation failed.{0}ClientId: {1}, AlgoId: {2}{0}Details:{0}{3}.
        /// </summary>
        internal static string AlgoDataSaveFailedOnCodeValidation {
            get {
                return ResourceManager.GetString("AlgoDataSaveFailedOnCodeValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AlgoId is empty.
        /// </summary>
        internal static string AlgoIdEmpty {
            get {
                return ResourceManager.GetString("AlgoIdEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot {0} algo because it has {1}algo instances..
        /// </summary>
        internal static string AlgoInstancesExist {
            get {
                return ResourceManager.GetString("AlgoInstancesExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo {0} (client {1}) is public and not editable.
        /// </summary>
        internal static string AlgoIsPublic {
            get {
                return ResourceManager.GetString("AlgoIsPublic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Public algorithms can not be modified.
        /// </summary>
        internal static string AlgoIsPublicDisplayMessage {
            get {
                return ResourceManager.GetString("AlgoIsPublicDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The algo must not be public..
        /// </summary>
        internal static string AlgoMustNotBePublic {
            get {
                return ResourceManager.GetString("AlgoMustNotBePublic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Asset {0} is not valid for asset pair {1}..
        /// </summary>
        internal static string AssetInvalidForAssetPair {
            get {
                return ResourceManager.GetString("AssetInvalidForAssetPair", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not retrieve asset(s) for the given Asset Pair.
        /// </summary>
        internal static string AssetNotFound {
            get {
                return ResourceManager.GetString("AssetNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The asset pair {0} is temporarily disabled..
        /// </summary>
        internal static string AssetPairDisabledDisplayMessage {
            get {
                return ResourceManager.GetString("AssetPairDisabledDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not retrieve asset pair with id {0}.
        /// </summary>
        internal static string AssetPairNotFound {
            get {
                return ResourceManager.GetString("AssetPairNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Asset {0} and {1} are missing from wallet {2}.
        /// </summary>
        internal static string AssetsMissingFromWallet {
            get {
                return ResourceManager.GetString("AssetsMissingFromWallet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ClientId is empty.
        /// </summary>
        internal static string ClientIdEmpty {
            get {
                return ResourceManager.GetString("ClientIdEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo creation process failed due to code validation errors. Details: {0}.
        /// </summary>
        internal static string CreateAlgoFailedOnCodeValidationDisplayMessage {
            get {
                return ResourceManager.GetString("CreateAlgoFailedOnCodeValidationDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo creation process failed while saving algo data.
        /// </summary>
        internal static string CreateAlgoFailedOnDataSaveDisplayMessage {
            get {
                return ResourceManager.GetString("CreateAlgoFailedOnDataSaveDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo creation process failed due to data validation errors.
        /// </summary>
        internal static string CreateAlgoFailedOnValidationDisplayMessage {
            get {
                return ResourceManager.GetString("CreateAlgoFailedOnValidationDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} Start Date cannot be later than or equal to the End Date.
        /// </summary>
        internal static string DatesValidationMessage {
            get {
                return ResourceManager.GetString("DatesValidationMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem while deleting the kuebrnetes deployment.  Algo Instance Stopping client error: {0}.
        /// </summary>
        internal static string DeleteKubernetesDeploymentError {
            get {
                return ResourceManager.GetString("DeleteKubernetesDeploymentError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Demo or BackTest instances are not allowed to run without fake trading..
        /// </summary>
        internal static string DemoOrBacktestCantRunLive {
            get {
                return ResourceManager.GetString("DemoOrBacktestCantRunLive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo edit process failed due to code validation errors. Details: {0}.
        /// </summary>
        internal static string EditAlgoFailedOnCodeValidationDisplayMessage {
            get {
                return ResourceManager.GetString("EditAlgoFailedOnCodeValidationDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo edit process failed while saving algo data.
        /// </summary>
        internal static string EditAlgoFailedOnDataSaveDisplayMessage {
            get {
                return ResourceManager.GetString("EditAlgoFailedOnDataSaveDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo edit process failed due to data validation errors.
        /// </summary>
        internal static string EditAlgoFailedOnValidationDisplayMessage {
            get {
                return ResourceManager.GetString("EditAlgoFailedOnValidationDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Classes inheriting BaseAlgo must be sealed.
        /// </summary>
        internal static string ERROR_ALGO_NOT_SEALED {
            get {
                return ResourceManager.GetString("ERROR_ALGO_NOT_SEALED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More than one class inheriting BaseAlgo is not allowed.
        /// </summary>
        internal static string ERROR_BASEALGO_MULTIPLE_INHERITANCE {
            get {
                return ResourceManager.GetString("ERROR_BASEALGO_MULTIPLE_INHERITANCE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A class inheriting BaseAlgo was not found.
        /// </summary>
        internal static string ERROR_BASEALGO_NOT_INHERITED {
            get {
                return ResourceManager.GetString("ERROR_BASEALGO_NOT_INHERITED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Setting null as the default value is not allowed.
        /// </summary>
        internal static string ERROR_DEFAULT_VALUE_NULL {
            get {
                return ResourceManager.GetString("ERROR_DEFAULT_VALUE_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo must override {0} and/or {1}.
        /// </summary>
        internal static string ERROR_EVENT_NOT_IMPLEMENTED {
            get {
                return ResourceManager.GetString("ERROR_EVENT_NOT_IMPLEMENTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided namespace is not allowed.
        /// </summary>
        internal static string ERROR_NAMESPACE_NOT_CORRECT {
            get {
                return ResourceManager.GetString("ERROR_NAMESPACE_NOT_CORRECT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A required namespace is not found.
        /// </summary>
        internal static string ERROR_NAMESPACE_NOT_FOUND {
            get {
                return ResourceManager.GetString("ERROR_NAMESPACE_NOT_FOUND", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate indicator names are not allowed.
        /// </summary>
        internal static string ERROR_INDICATOR_DUPLICATE_NAME {
            get {
                return ResourceManager.GetString("ERROR_INDICATOR_DUPLICATE_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Indicator name must be a string literal.
        /// </summary>
        internal static string ERROR_INDICATOR_NAME_NOT_LITERAL {
            get {
                return ResourceManager.GetString("ERROR_INDICATOR_NAME_NOT_LITERAL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A type named BaseAlgo is not allowed.
        /// </summary>
        internal static string ERROR_TYPE_NAMED_BASEALGO {
            get {
                return ResourceManager.GetString("ERROR_TYPE_NAMED_BASEALGO", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem getting pod information. Algo Instance Stopping client error: {0}.
        /// </summary>
        internal static string ErrorGettingPod {
            get {
                return ResourceManager.GetString("ErrorGettingPod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Instance {0} must be stopped before it can be deleted..
        /// </summary>
        internal static string InstanceMustBeStopped {
            get {
                return ResourceManager.GetString("InstanceMustBeStopped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Instance Not Found.
        /// </summary>
        internal static string InstanceNotFound {
            get {
                return ResourceManager.GetString("InstanceNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have reached the limit of running/deploying instances. You can&apos;t run more instances..
        /// </summary>
        internal static string LimitOfRunningInsatcnesReached {
            get {
                return ResourceManager.GetString("LimitOfRunningInsatcnesReached", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Live algo instances are not allowed to use fake trading..
        /// </summary>
        internal static string LiveAlgoCantFakeTrade {
            get {
                return ResourceManager.GetString("LiveAlgoCantFakeTrade", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; field is missing from AlgoMetaData.
        /// </summary>
        internal static string MetadataFieldMissing {
            get {
                return ResourceManager.GetString("MetadataFieldMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find algo data. ClientId: {0}, AlgoId: {1}.
        /// </summary>
        internal static string NoAlgoData {
            get {
                return ResourceManager.GetString("NoAlgoData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find algo data.
        /// </summary>
        internal static string NoAlgoDataDisplayMessage {
            get {
                return ResourceManager.GetString("NoAlgoDataDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user currently has {0} running/deploying instances. The user is not allowed to create and deploy more instances. ClientId: {1}  and AlgoId: {2}.
        /// </summary>
        internal static string NotAvailableCreationOfInstances {
            get {
                return ResourceManager.GetString("NotAvailableCreationOfInstances", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algo Not Found.
        /// </summary>
        internal static string NotFoundAlgo {
            get {
                return ResourceManager.GetString("NotFoundAlgo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} cannot be deleted..
        /// </summary>
        internal static string ParamCantBeDeleted {
            get {
                return ResourceManager.GetString("ParamCantBeDeleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid {0}..
        /// </summary>
        internal static string ParamInvalid {
            get {
                return ResourceManager.GetString("ParamInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} was not found..
        /// </summary>
        internal static string ParamNotFoundDisplayMessage {
            get {
                return ResourceManager.GetString("ParamNotFoundDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The permissions of this role cannot be modified..
        /// </summary>
        internal static string PermissionsCantBeModified {
            get {
                return ResourceManager.GetString("PermissionsCantBeModified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Public algo Not Found.
        /// </summary>
        internal static string PublicAlgoNotFound {
            get {
                return ResourceManager.GetString("PublicAlgoNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Role {0} already exists..
        /// </summary>
        internal static string RoleAlreadyExists {
            get {
                return ResourceManager.GetString("RoleAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This role can&apos;t be modified..
        /// </summary>
        internal static string RoleCantBeModified {
            get {
                return ResourceManager.GetString("RoleCantBeModified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are running algo instance(s).
        /// </summary>
        internal static string RunningAlgoInstanceExists {
            get {
                return ResourceManager.GetString("RunningAlgoInstanceExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Algorithms with running instances can not be modified.
        /// </summary>
        internal static string RunningAlgoInstanceExistsDisplayMessage {
            get {
                return ResourceManager.GetString("RunningAlgoInstanceExistsDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is empty..
        /// </summary>
        internal static string StringParameterMissing {
            get {
                return ResourceManager.GetString("StringParameterMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please fill in all required fields..
        /// </summary>
        internal static string StringParameterMissingDisplayMessage {
            get {
                return ResourceManager.GetString("StringParameterMissingDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The amount should be higher than minimal order size {0} {1}.
        /// </summary>
        internal static string TradeVolumeBelowMinimum {
            get {
                return ResourceManager.GetString("TradeVolumeBelowMinimum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You do not have permission to access this algo..
        /// </summary>
        internal static string UserCantSeeAlgo {
            get {
                return ResourceManager.GetString("UserCantSeeAlgo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot publish/unpublish algo because you are not the author of the algo..
        /// </summary>
        internal static string UserNotAuthorOfAlgo {
            get {
                return ResourceManager.GetString("UserNotAuthorOfAlgo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Current user does not belong to &apos;User&apos; role..
        /// </summary>
        internal static string UserNotInUserRole {
            get {
                return ResourceManager.GetString("UserNotInUserRole", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The wallet is already used by another your instance.
        /// </summary>
        internal static string WalletAlreadyUsed {
            get {
                return ResourceManager.GetString("WalletAlreadyUsed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This wallet has no assets..
        /// </summary>
        internal static string WalletHasNoAssetsDisplayMessage {
            get {
                return ResourceManager.GetString("WalletHasNoAssetsDisplayMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wallet is already used by a running instance - Wallet Id {0}. You should use another wallet. Algo id {1}, Client Id {2}.
        /// </summary>
        internal static string WalletIsAlreadyUsed {
            get {
                return ResourceManager.GetString("WalletIsAlreadyUsed", resourceCulture);
            }
        }
    }
}
