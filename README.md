# REPOSITORY ARCHIVED: Next-Gen -> [natsuneko-laboratory/SharpX](https://github.com/natsuneko-laboratory/SharpX)

# SharpX

Extensible Experimental C# to _X_ Transpiler.  
SharpX compiler understands the code and compiles it according to the C# 9 language specification.
However, how much language specification is supported depends on the compilation target (for example, ShaderLab does not support LINQ and async-await).

## Motivation

C# has a flexible and strongly type system, high quality editor support, compiler platform system, and its ecosystem.
This makes programming in C# a fairly pleasant experience.
Therefore, I thought that if I could convert from C# to other relatively low-level languages, such as HLSL, GLSL, or Assemblies, my productivity in those areas would be greatly improved.
Originally, it was a tool for writing ShaderLab in C#, but I thought it could be applied to other languages with some flexibility in the plugin system.
SharpX was born as a result.

## VS other similar tools

There are several implementations of the C# to _X_ transpiler that I can see.
Some of the advantages over them include:

- Roslyn based - You can use the latest C# language specifications
- Extendable - This allows you to compile to any language

However, there are some disadvantages as well:

- Unoptimized - Not specializing in a particular language, mysterious syntax may be required
- Portability - Portability to different languages is low because each specific language requires different requirements

## Available Backend Languages

- ShaderLab (HLSL)

## Requirements

This tool works independently of any particular platform.

### Runtime (User) Requirements

- .NET 6 Preview 7 or greater

This means that SharpX will not work at natively on Unity at this time.
However, since SharpX itself is provided as a library and executable, it can be run in the UnityEditor by writing simple wrapper editor extension code.

### Developer Requirements

- .NET 6 Preview 7 or greater
- Visual Studio 2022 Preview 3 or greater

## Documentation

Full documentation is available at [docs](./docs) directory in this repository.

## Questions

### Should I use SharpX?

No.
SharpX is an experimental tool and is not recommended to be used in a production environment if possible.
Also, some people have told to me in the past, _quality is poor_.

### What softwares are using SharpX?

At the very least, I've been using SharpX and SharpX ShaderLab plugin to create shaders for my own VRChat avatars.

## Similar Projects

- [ASDAlexander77/cs2cpp](https://github.com/ASDAlexander77/cs2cpp) - C# to CPP Transpiler
- [AshleighAdams/ShaderSharp](https://github.com/AshleighAdams/ShaderSharp) - C# to Shader Transpiler for GLSL
- [mellinoe/ShaderGen](https://github.com/mellinoe/ShaderGen) - C# to Shader Transpiler for HLSL, GLSL, and Metal
- [reignstudios/CS2X](https://github.com/reignstudios/CS2X) - C# to _X_ Transpiler

## License

MIT by [@6jz](https://twitter.com/6jz)
