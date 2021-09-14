namespace SharpX.Compiler.ShaderLab.Models
{
    internal static class ErrorConstants
    {
        // 0001 - 1000 : NotSupported
        public const int NotSupportedType = 1;

        public const int NotSupportedNestTypeDeclaration = 2;

        public const int NotSupportedUserDefinedCompilerGeneratedMethod = 3;

        public const int NotSupportedBracketedArgumentList = 4;

        public const int NotSupportedMultipleDeclarationInSingle = 5;

        public const int NotSupportedFieldInitializer = 6;

        public const int NotSupportedGlobalMemberDeclarationAsInstanceVariable = 7;

        public const int NotSupportedSemanticFieldDeclarationAsStaticVariable = 8;

        public const int NotSupportedPropertyInitializer = 9;

        public const int NotSupportedPropertyBodies = 10;

        public const int NotSupportedPropertyExpressionBodies = 11;

        public const int NotSupportedNonTypedParameters = 12;

        public const int NotSupportedUserDefinedConstructors = 13;

        public const int NotSupportedInitializerWithConstructor = 14;

        public const int NotSupportedNestedInitializer = 15;

        public const int NotSupportedArrayTypesYet = 16;

        public const int NotSupportedPointerTypes = 17;

        public const int NotSupportedFunctionPointers = 18;

        public const int NotSupportedNullableTypes = 19;

        public const int NotSupportedRefTypes = 20;

        public const int NotSupportedAsyncAwaitExpression = 21;

        public const int NotSupportedConditionalAccess = 22;

        public const int NotSupportedCheckedExpression = 23;

        public const int NotSupportedTypeofExpression = 24;

        public const int NotSupportedSizeofExpression = 25;

        public const int NotSupportedStackalloc = 26;

        public const int NotSupportedLinqFeatures = 27;

        public const int NotSupportedThrowingExceptions = 28;

        public const int NotSupportedGoto = 29;

        public const int NotSupportedUsingStatement = 30;

        public const int NotSupportedFixedStatement = 31;

        public const int NotSupportedCheckedStatement = 32;

        public const int NotSupportedUnsafeStatement = 33;

        public const int NotSupportedLockedStatement = 34;

        public const int NotSupportedHandlingExceptions = 35;

        public const int NotSupportedVariableMemberDeclarationsOutsideOfStruct = 36;

        public const int NotSupportedModifierFieldDeclarationAsStaticVariable = 8;

        // 1000 - 2000 : Invalid
        public const int InvalidComponentName = 1000;

        public const int InvalidParameterInAndOutAttribute = 1001;

        public const int InvalidSemanticsName = 1002;

        // 3000 - 4000 : Compiler Errors
        public const int FailedToCaptureSymbols = 3000;

        // 8000 -      : Warnings
        public const int SemanticIsNotSpecifiedInStruct = 8000;

    }
}