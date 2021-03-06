# SharpX.Compiler.ShaderLab

An Experimental ~~Proof-of-Concept~~ C# to ShaderLab Transpiler for SharpX Compiler.

## **Un**supported C# language features

The C# language features listed below are limited due to the backend (HLSL).
C# (Version 9.0) features other than those listed below are roughly supported.

- Non-Primitive Types and its Methods / Properties / Fields / Constructors
- Nullable Types
- Conditional Access : `??`, `?.`, `!.`
- Throwing Exceptions
- `async` / `await` / `stackalloc` / `when` / `with` expression
- `as` / `is` / `typeof` / `sizeof` / `nameof` operator
- `checked` / `unsafe` / `fixed` / `lock` / `using` / `goto` / `try` - `catch` ( - `finally`) statement

## Examples

You can define `struct` to use `class`, `interface`, `record`, or `struct` declaration.
The properties of a struct can be declared using instance properties with getter or fields.

```csharp
public class AppData
{
    [Semantic("SV_POSITION")]
    public SlFloat4 Vertex { get; }
}

//
// struct AppData
// {
//     float4 Vertex : SV_POSITION;
// };
```

Shaders can be written as you would write a normal C# process.

```csharp
[Export("vert.{extension}")]
public class VertexShader
{
    [VertexShader]
    public Vertex2Fragment Vert(AppData i)
    {
        return new()
        {
            Vertex = UnityCg.UnityObjectToClipPos(v.Vertex),
            Normal = UnityCg.UnityObjectToWorldNormal(v.Normal),
            TexCoord = UnityCg.TransformTexture(v.TexCoord, Globals.MainTexture),
            WorldPos = Builtin.Mul<SlFloat3>(UnityCg.UnityObjectToWorld, v.Vertex),
            LocalPos = v.Vertex.XYZ
        };
    }
}
```

If you want to use a branch like `#ifdef PREPROCESSOR` in HLSL, you can write a similar statement in C# and pass compiler options to output the corresponding shader.

```csharp
[Export("frag.{extension}")]
public class FragmentShader
{
    [FragmentShader]
    [Function("fs")]
    [return: Semantic("SV_TARGET")]
    public SlFloat4 Fragment(Geometry2Fragment i)
    {
#if SHADER_SHADOWCASTER
        Compiler.Raw("SHADOW_CASTER_FRAGMENT(i)");
#else
        // other code
#endif
    }
}

// compile with SHADER_SHADOWCASTER preprocessor options
//
// float4 fs(g2f i) : SV_TARGET
// {
//     SHADOW_CASTER_FRAGMENT(i)
// }
```

## Special Syntaxes

Some of the features available in HLSL are not supported in C#.
SharpX.Compiler.ShaderLab provides a workaround.

### Statement Attributes

If you want to add an attribute to the statement itself, such as unroll, use `Compiler.AnnotatedStatement`.

```csharp
Compiler.AnnotatedStatement("unroll", () => {
    for (var j = 0; j < 3; j++)
    {
        ...
    }
});

//
// [unroll]
// for (int j = 0; j < 3;  j++)
// {
//     ...
// }
```

### Macro

If you want to extract macros used in Unity or other libraries, use `Compiler.Raw`.
This allows you to output HLSL source code in its original form.

```csharp
Compiler.Raw("SHADOW_CASTER_FRAGMENT(i)");

//
// SHADOW_CASTER_FRAGMENT(i)
```

If you want to expand a macro inside a struct, you can do the same by attaching a RawAttribute to the property starting with two underscores.

```csharp
public struct AppData
{
    [Raw("V2F_SHADOW_CASTER")]
    public int __ShadowCaster { get; }
}

//
// struct AppData
// {
//     V2F_SHADOW_CASTER
// }
```
