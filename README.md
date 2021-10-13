# MSBuild.CompactJsonResources

![header](https://raw.githubusercontent.com/dimonovdd/MSBuild.CompactJsonResources/main/header.svg)

[![NuGet Badge](https://img.shields.io/nuget/vpre/MSBuild.CompactJsonResources)](https://www.nuget.org/packages/MSBuild.CompactJsonResources/) [![license](https://img.shields.io/github/license/dimonovdd/MSBuild.CompactJsonResources)](https://github.com/dimonovdd/MSBuild.CompactJsonResources/blob/main/LICENSE) [![fuget.org](https://www.fuget.org/packages/MSBuild.CompactJsonResources/badge.svg)](https://www.fuget.org/packages/MSBuild.CompactJsonResources)

Often, Json files are added to a project as one line to reduce the size of the build. Such files are uncomfortable to read and track changes through version control systems. This package resolve this problem by removing whitespaces, trailing commas and comments from the builds without changing the beautiful Json source files by adding their single-line copies.

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
2) Set `EnableCompactJson` property to `true`. (When the value is `false` the files data are not changed)
3) Just use `JsonEmbeddedResource` as `EmbeddedResource`.

```xml
<PropertyGroup>
  <EnableCompactJson>true</EnableCompactJson>
</PropertyGroup>
<ItemGroup>
  <JsonEmbeddedResource Include="data.json" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="MSBuild.CompactJsonResources" Version="1.0.0" PrivateAssets="all"/>
</ItemGroup>
```
