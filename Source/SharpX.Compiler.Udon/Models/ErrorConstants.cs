namespace SharpX.Compiler.Udon.Models
{
    internal static class ErrorConstants
    {
        // Unsupported Features by Compiler
        public const int NotSupportedLinqFeatures = 1;

        public const int NotSupportedGoto = 2;

        public const int NotSupportedInParameter = 3;

        public const int NotSupportedOutParameter = 4;

        public const int NotSupportedRefParameter = 5;

        public const int NotSupportedNamedParameter = 6;


        // Udon Related Features
        public const int NotSupportedPointerTypes = 1001;

        public const int NotSupportedFunctionPointers = 1002;

        public const int NotSupportedAsyncAwaitExpression = 1003;

        public const int NotSupportedRefTypes = 1004;

        public const int NotSupportedCheckedExpression = 1005;

        public const int NotSupportedSizeofExpression = 1006;

        public const int NotSupportedStackalloc = 1007;

        public const int NotSupportedThrowingExceptions = 1009;

        public const int NotSupportedUsingStatement = 1011;

        public const int NotSupportedFixedStatement = 1012;

        public const int NotSupportedCheckedStatement = 1013;

        public const int NotSupportedUnsafeStatement = 1014;

        public const int NotSupportedLockedStatement = 1015;

        public const int NotSupportedHandlingExceptions = 1016;

        public const int NotSupportedUdonType = 1017;

        public const int NotSupportedAwakeEvent = 1018;

        public const int NotSupportedUserDefinedConstructors = 1019;

        public const int NotSupportedUdonMethod = 1020;

        // Unhandled Errors
        public const int FailedToCaptureSymbols = 3000;
    }
}