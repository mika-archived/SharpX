namespace SharpX.Compiler.Udon.Models
{
    internal static class ErrorConstants
    {

        // Unsupported Features
        public const int NotSupportedPointerTypes = 1;

        public const int NotSupportedFunctionPointers = 2;

        public const int NotSupportedAsyncAwaitExpression = 3;

        public const int NotSupportedRefTypes = 4;

        public const int NotSupportedCheckedExpression = 5;

        public const int NotSupportedSizeofExpression = 6;

        public const int NotSupportedStackalloc = 7;

        public const int NotSupportedLinqFeatures = 8;

        public const int NotSupportedThrowingExceptions = 9;

        public const int NotSupportedGoto = 10;

        public const int NotSupportedUsingStatement = 11;

        public const int NotSupportedFixedStatement = 12;

        public const int NotSupportedCheckedStatement = 13;

        public const int NotSupportedUnsafeStatement = 14;

        public const int NotSupportedLockedStatement = 15;

        public const int NotSupportedHandlingExceptions = 16;
    }
}