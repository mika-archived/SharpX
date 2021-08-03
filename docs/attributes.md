# Attributes

In ShaderSharp, you can control the shader output by using Attributes.

## ComponentAttribute

`ComponentAttribute` can be used to output the properties and fields defined in class, interface, record, and others as a `struct` structure.
This Attribute is also used in `ShaderSharp.Library.Primitives` and can be used to create C# wrappers for existing types defined in the shader side.

### Example

```csharp
// @ AppData.cs
[Component("appdata")]
public record AppData
{
    public SlFloat4 SomeProperty { get; init; }
}
```

transpiled to

```cpp
// @ source.cginc
struct appdata {
    float4 SomeProperty;
};
```

## ExportAttribute

`ExportAttribute` allows you to specify the name of the file to be write as a shader.
If omitted, all transpiled results will be write as `source.cginc`.

### Example

```csharp
// @ AppData.cs
[Export("core.cginc")]
public record AppData
{
    public SlFloat4 SomeProperty { get; init; }
}
```

transpiled to

```cpp
// @ core.cginc
struct AppData
{
    float4 SomeProperty;
};
```

## ExternalAttribute

`ExternalAttribute` indicates that the component or method of the set class is a primitive type or variable in the shader side, or is defined in a library supported by the shader.
It behaves in a similar way to the `DllImportAttribute` that can be placed in a normal .

### Example

```csharp
// @ SlFloat4.cs
[External]
[Component("float4")]
public class SlFloat4 {}

// @ AppData.cs
public record AppData
{
    public SlFloat4 SomeProperty { get; init; }
}
```

transpiled to

```cpp
// @ source.cginc
struct AppData {
    float4 SomeProperty;
};
```

## FunctionAttribute

`FunctionAttribute` tells the compiler to use the name specified in the attribute for the method, instead of using the method name as-is in the shader output.
This can be used to adapt the naming conventions to each language when creating function call definitions in C#, which are usually defined in the shader side.
When defining an externally defined function in C# use `FunctionAttribute`, it is recommended to defined the method with the `static extern` modifier.

### Example

```csharp
// @ VertexShader.cs
[Export("vert.cginc")]
public class VertexShader
{
    [Function("some_function")]
    private void SomeFunction() {}

    [Function("vs")]
    public Vertex2Fragment VertexMain()
    {
        SomeFunction();
        return new Vertex2Fragment();
    }
}
```

transpiled to

```csharp
// @ vert.cginc
void some_function() {}

Vertex2Fragment vs()
{
    some_function();
    return (Vertex2Fragment) 0;
}
```

## GlobalMemberAttribute

`GlobalMemberAttribute` indicates that the target static property and field is a global property that is populated from the shader application.
These are write as Material Property in the case of ShaderLab and as so-called `uniform` variables in the case of HLSL.

### Example

```csharp
// @ Globals.cs
[Export("core.cginc")]
public static class Globals
{
    [GlobalMember]
    [External]
    public static SlFloat4 _Time;

    [GlobalMember]
    public static SlFloat4 _Color;
}
```

transpiled to

```cpp
// @ core.cginc
float4 _Color;
```

## IncludeAttribute

`IncludeAttribute` notifies that the target file needs to be `#include` for method calls and variable accesses of the configured class.
This is used to call functions defined in `UnityCG.cginc` in the ShaderLab.
Note that in files where `ExportAttribute` is used, there is usually no need to explicitly indicate that the file needs to be included using `IncludeAttribute`.

### Example

```csharp
// @ UnityCg.cs
[Include("UnityCG.cginc")]
public static class UnityCg
{
    [External]
    [Function("some_function")]
    public static extern void SomeFunction();
}

// @ VertexShader.cs
[Export("vert.cginc")]
public class VertexShader
{
    [Function("vs")]
    public Vertex2Fragment VertexMain()
    {
        UnityCg.SomeFunction();
        return new Vertex2Fragment();
    }
}
```

transpiled to

```cpp
// vert.cginc

// If the parent file exists, it will be output to the corresponding location.
#include <UnityCg.cginc>

Vertex2Fragment vs()
{
    some_function();
    return (Vertex2Fragment) 0;
}
```

## InlineAttribute

`InlineAttribute` outputs the target method in its inline expanded state, rather than as a function call.
If the first argument is `false`, the method will be expanded by ShaderSharp, and if `true` (default), `inline` will be added to the function.

### Example

```csharp
// @ FragmentShader.cs
[Export("frag.cginc")]
public class FragmentShader
{
    [Function("get_alpha_color")]
    // [Inline]
    [Inline(false)]
    public SlFloat4 GetAlphaColor(SlFloat2 uv)
    {
        return Builtin.Tex2DLod(Globals._MainTex, uv) * Globals._Alpha;
    }

    [Function("fs")]

    public SlFloat4 FragMain(Vertex2Fragment i)
    {
        return GetAlphaColor(i.UV);
    }
}
```

transpiled to

```cpp
// @ frag.cginc

// When [Inline]
inline float4 get_alpha_color(float2 uv)
{
    return tex2dlod(_MainTex, uv) * _Alpha;
}


float4 fs(Vertex2Fragment i)
{
    // When [Inline]
    return get_alpha_color(i.uv);

    // When [Inline(false)]
    float2 get_alpha_color_uv_1 = i.UV;
    float4 get_alpha_color_result_1 = tex2dlod(_MainTex, get_alpha_color_uv_1) * _Alpha;
    return get_alpha_color_result_1;
}
```

## PropertyAttribute

`PropertyAttribute` tells the compiler to use an alias on output for structure-defined properties and fields.
You can think of this as a Property version of FunctionAttribute.

### Example

```csharp
// @ AppData.cs
[Component("appdata")]
[Export("core.cginc")]
public record AppData
{
    [Property("position")]
    public SlFloat2 Position { get; init; }
}
```

transpiled to

```cpp
// @ core.cginc
struct appdata {
    float2 position;
}
```

## SemanticAttribute

`SemanticAttribute` indicates that properties and fields should be given the semantic information specified when compiling to struct.

### Example

```csharp
// @ AppData.cs
[Component("appdata")]
[Export("core.cginc")]
public record AppData
{
    [Semantic("POSITION")]
    [Property("position")]
    public SlFloat2 Position { get; init; }
}
```

transpiled to

```cpp
// @ core.cginc
struct appdata {
    float2 position : POSITION;
}
```
