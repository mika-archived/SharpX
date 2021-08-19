# SharpX.Compiler.CodeCleanup

An Experimental ~~Proof-of-Concept~~ C# Code Eliminator by Roslyn Semantic Model and Workspace API.  
This extension detects unused methods, properties, etc. in your code and removes them before compilation.
This will lighten the processing of the language backend that will be output and clean up the code.

## Supported Eliminations

- [x] Methods
- [x] Properties
- [x] Fields
- [ ] Local Functions

## Examples

```csharp
public class SomeClass
{
    [GlobalMember]
    public SlFloat3 _Color { get; }

    [GlobalMember]
    public SlFloat3 _SecondColor { get; }

    public void UnusedMethod() {}

    [Mark]
    public SlFloat3 FragmentShader(...)
    {
        return _Color;
    }
}
```

rewrites to

```csharp
public class SomeClass
{
    [GlobalMember]
    public SlFloat3 _Color { get; }

    [Mark]
    public SlFloat3 FragmentShader(...)
    {
        return _Color;
    }
}
```

## Configuration

If you do not want to delete a method or property with a specific name, you can add a setting to prevent it from being deleted.

```json
// @ txc.config.json
{
  "CodeCleanup": {
    "AllowList": {
      // If the method, property, fields has specified attribute(s), do not eliminate it.
      "Attributes": ["FullyQualifiedClassName"],
      // If the method specified the name, do not eliminate it.
      "Methods": ["ContainerSymbol#MethodName"],
      // If the property ...
      "Properties": [],
      // If the field ...
      "Fields": []
    }
  }
}
```

## Attention

Basically, this plugin does not take into account the specifics of each language backend.
Therefore, you need to specify the entry points in Attributes and Methods.
For example, in the case of ShaderLab, the settings are as follows:

```json
{
  "CodeCleanup": {
    "AllowList": {
      "Attributes": [
        "SharpX.Library.ShaderLab.Attributes.VertexShaderAttribute",
        "SharpX.Library.ShaderLab.Attributes.GeometryShaderAttribute",
        "SharpX.Library.ShaderLab.Attributes.FragmentShaderAttribute",
        "SharpX.Library.ShaderLab.Attributes.GlobalMemberAttribute"
      ],
      "Methods": [],
      "Properties": [],
      "Fields": [],
      "Variables": []
    }
  }
}
```
