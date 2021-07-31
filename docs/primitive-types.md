# Primitive Types

ShaderSharp allows all types available in ShaderLab HLSL to be used on C# by using the types defined in `ShaderSharp.Library.Primitives`, in addition to some of the C# primitive types.
Also, primitive types in C# and in ShaderSharp can be implicitly cast.

```csharp
using ShaderSharp.Library.Primitives;
using static ShaderSharp.Library.Functions.Builtin;

SlFloat f = 1.0f; // implicit cast from float to SlFloat

Atan2(1.0, 1.0); // => SlFloat Atan2(SlFloat, SlFloat)
```

It is also possible to cast from `SlFloat4`, which corresponds to `float4`, to `float` as HLSL does.

```csharp
SlFloat4 f4 = new SlFloat4(1.0f);
SlFloat  f1 = f4;

Clamp(f4, f4, 0.5); // SlFloat4 Clamp(SlFloat4, SlFloat4, SlFloat4)
```

The HLSL primitive types supported by ShaderSharp are as follows:

| HLSL Primitive | ShaderSharp Primitive | C# Primitive |
| :------------: | :-------------------: | :----------: |
|     `bool`     |       `SlBool`        |    `bool`    |
|    `bool2`     |       `SlBool2`       |    _none_    |
|    `bool3`     |       `SlBool3`       |    _none_    |
|    `bool4`     |       `SlBool4`       |    _none_    |
|    `fixed`     |       `SlFixed`       |    _none_    |
|    `fixed2`    |      `SlFixed2`       |    _none_    |
|    `fixed3`    |      `SlFixed3`       |    _none_    |
|    `fixed4`    |      `SlFixed4`       |    _none_    |
|    `float`     |       `SlFloat`       |   `float`    |
|    `float2`    |      `SlFloat2`       |    _none_    |
|    `float3`    |      `SlFloat3`       |    _none_    |
|    `float4`    |      `SlFloat4`       |    _none_    |
|     `half`     |       `SlHalf`        |    _none_    |
|    `half2`     |       `SlHalf2`       |    _none_    |
|    `half3`     |       `SlHalf3`       |    _none_    |
|    `half4`     |       `SlHalf4`       |    _none_    |
|     `int`      |        `SlInt`        |    `int`     |
|     `int2`     |       `SlInt2`        |    _none_    |
|     `int3`     |       `SlInt3`        |    _none_    |
|     `int4`     |       `SlInt4`        |    _none_    |
|     `uint`     |       `SlUint`        |    `uint`    |
|    `uint2`     |       `SlUint2`       |    _none_    |
|    `uint3`     |       `SlUint3`       |    _none_    |
|    `uint4`     |       `SlUint4`       |    _none_    |
|  `sampler2D`   |     `SlSampler2D`     |    _none_    |
|  `sampler3D`   |     `SlSampler3D`     |    _none_    |
| `samplerCUBE`  |    `SlSamplerCube`    |    _none_    |
|   `bool1x1`    |      `SlBool1x1`      |    _none_    |
|   `fixed1x1`   |     `SlFixed1x1`      |    _none_    |
|   `float1x1`   |     `SlFloat1x1`      |    _none_    |
|   `half1x1`    |      `SlHalf1x1`      |    _none_    |
|    `int1x1`    |      `SlInt1x1`       |    _none_    |
|   `uint1x1`    |      `SlUint1x1`      |    _none_    |
|  `Texture2D`   |     `SlTexture2D`     |    _none_    |
| `SamplerState` |   `SlSamplerState`    |    _none_    |

Note that `fixed` and `half` are converted to `float` in DirectX.
They also support implicit casting from `float` in C#, but if you enter a value out of range, you will not be notified of a compile error.
