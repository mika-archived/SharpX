# ShaderSharp

Experimental C# to ShaderLab Compiler.

## Motivation

I usually write my own shaders to make my avatar look more cute in VRChat.
However, editor support in shaders is poor compared to other languages, and coding is not comfortable.
That's why I created a system based on C# with strong editor support and converting it to ShaderLab (HLSL).
This allows you to use the strongly type system, IntelliSense, compiler features, and other C# features.

## Requirements

### Runtime Requirements

- .NET 6 Preview 2
- .NET Standard 2.0 (CodeGen - Analyzer)

This means that ShaderSharp will not work at runtime on Unity at this time.
However, since ShaderSharp itself is provided as a library and executable, it can be run in the Unity editor by writing a simple wrapper code.

### Developer Requirements

- Visual Studio 2019 with .NET 6

## Sample Shader

Shaders can be written using regular C# syntax, with some limitations.
The complete code can be found in the [ShaderSharp.Example](Source/ShaderSharp.Example/) project.

```csharp
// TODO
```

The above C# code will be transpiled into ShaderLab HLSL code as follows:

```cpp
// <auto-generated />
// TODO
```

## Similar Projects

- [AshleighAdams/ShaderSharp](https://github.com/AshleighAdams/ShaderSharp) - C# to Shader Transpiler for GLSL
- [mellinoe/ShaderGen](https://github.com/mellinoe/ShaderGen) - C# to Shader Transpiler for HLSL, GLSL, and Metal

## License

MIT by [@6jz](https://twitter.com/6jz)