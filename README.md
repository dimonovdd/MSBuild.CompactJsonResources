# MSBuild.CompactJsonResources

![header](https://raw.githubusercontent.com/dimonovdd/MSBuild.CompactJsonResources/main/header.svg)

[![NuGet Badge](https://img.shields.io/nuget/vpre/MSBuild.CompactJsonResources)](https://www.nuget.org/packages/MSBuild.CompactJsonResources/) [![license](https://img.shields.io/github/license/dimonovdd/MSBuild.CompactJsonResources)](https://github.com/dimonovdd/MSBuild.CompactJsonResources/blob/main/LICENSE) [![fuget.org](https://www.fuget.org/packages/MSBuild.CompactJsonResources/badge.svg)](https://www.fuget.org/packages/MSBuild.CompactJsonResources)

Often, Json files are added to the project as one line for reduce a size of a build. It is not very convenient to read and track changes through version control systems. This package removes whitespaces, trailing commas and comments from builds without changing the source files.

```json
{
  "null": null, //this is comment
  "number": 123,
  "object": {
    "a": "b",
    "c": "d",
  },
  "string": "Hello World",
}
```

```json
{"null":null,"number":123,"object":{"a":"b","c":"d"},"string":"Hello World"}
```

## Getting started

1) Add reference for this package.
2) Set `EnableCompactJson` property to `true.` (When the value is `false` the files data are not changed)
3) Just use `JsonEmbeddedResource` as `EmbeddedResource`.

```xml
<PropertyGroup>
  <EnableCompactJson>true</EnableCompactJson>
</PropertyGroup>
<ItemGroup>
  <JsonEmbeddedResource Include="data.json" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="MSBuild.CompactJsonResources" Version="1.0.0-preview1" PrivateAssets="all"/>
</ItemGroup>
```
