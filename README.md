# GGOOF

GGUF file format manipulator.

Acronym of the library name stands for GGUF Generally Object Oriented File (Manipulator).

- No dependencies
- High performance
- NativeAOT support
- Docker support
- WebAssembly support

## Examples

```csharp
Using GGOOF;

var model = Model.Version3.FromScratch();
```

## Interop

This library is meant to be used within dotnet environment, but since it can be compiled to native library, any language able to dynamically load it - may use it. _Wholesome experience not guaranteed._