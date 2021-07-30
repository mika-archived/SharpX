# ShaderSharp Function Generator

The ShaderSharp Function Generator is a Roslyn Source Generator provided as ShaderSharp.CodeGen.
These automatically generate C# HLSL function wrappers from DSL definitions.

## Syntax

The Function Generator has the following syntax.

- Comment
- Directive
- Declaration

### Comment

Lines starts with `//` will be recognized as comments.

### Directive

Lines starting with # are recognized as directives and give instructions to the code generator.
The following directives are valid:

- `#namespace NAMESPACE` ... Specifies the namespace to which the class belongs.
- `#class CLASS_NAME` ... Specifies the class name to be generated.
- `#converter CONVERTER` ... Specifies the naming convention for the generated methods.
- `#include FILEPATH` ... Specifies the path to use when including external files.

### Declaration

The following is the definition of the function. It is specified using the following syntax.

```typescript
function FUNCTION_NAME
{
    signatures
    {
        (PARAMETER1, PARAMETER2, ...) => RETURNS
        // one or more signatures
    }
}
```

The number of overloads corresponding to the signature will be generated with the name FUNCTION_NAME.
For PARAMETER and RETURNS, specify the following format:

```c
NAME is COMPONENT implements PRIMITIVE
// or
NAME is COMPONENT implements PRIMITIVE has N elements
```

NAME is the parameter name, COMPONENT is one of scalar, vector, or matrix, and PRIMITIVE is a primitive type such as float, bool, or int.
If you want to accept only those with a specific number of elements as parameters, specify `has N elements`.

The following is an example.

```plain
x is scalar implements float
x is vector implements bool
x is vector implements float has 3 elements
x is matrix implements float has TxT elements
x is matrix implements float has 3x3 elements
```

In addition, the following additional types can be specified in the return value.

```
void
__input__
```

void indicates that there is no return value, and \_\_input\_\_ indicates the same type as the first argument.
