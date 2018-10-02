namespace Lykke.AlgoStore.Services.Validation
{
    internal class ValidationErrors
    {
        public const string ERROR_BASEALGO_NOT_INHERITED = "AS0001";
        public const string ERROR_BASEALGO_MULTIPLE_INHERITANCE = "AS0002";
        public const string ERROR_ALGO_NOT_SEALED = "AS0003";
        public const string ERROR_TYPE_NAMED_BASEALGO = "AS0004";
        public const string ERROR_EVENT_NOT_IMPLEMENTED = "AS0005";
        public const string ERROR_INDICATOR_NAME_NOT_LITERAL = "AS0006";
        public const string ERROR_INDICATOR_DUPLICATE_NAME = "AS0007";
        public const string ERROR_DEFAULT_VALUE_NULL = "AS0008";
        public const string ERROR_NAMESPACE_NOT_CORRECT = "AS0009";
        public const string ERROR_NAMESPACE_NOT_FOUND = "AS0010";
        public const string ERROR_BLACKLISTED_TYPE_USED = "AS0011";
        public const string ERROR_BLACKLISTED_NAMESPACE_USED = "AS0012";
        public const string ERROR_PARAMETER_LOCAL_REFERENCE_USED = "AS0013";
    }
}
