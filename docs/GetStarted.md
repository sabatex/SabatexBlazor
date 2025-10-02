# Sabatex Radzen Blazor

## 1. Install

The Sabatex Radzen Blazor components are distributed via the Sabatex.RadzenBlazor nuget package and include reference to Radzen Blazor package.

You can add the Radzen.Blazor nuget package to your Blazor application in one of the following ways:

- Via Visual Studio's Nuget Package Manager.
- Via command line dotnet add package Sabatex.RadzenBlazor
- By editing your application's .csproj file and adding a package reference ``` <PackageReference Include="Sabatex.RadzenBlazor" Version="*" /> ```

## 2. Import the namespace

Open the <b>_Imports.razor </b> file of your Blazor application and append the following:

```
@using Radzen
@using Radzen.Blazor
@using Sabatex.RadzenBlazor
```

## 3. Include a theme

Open <b> _Host.cshtml </b> and add this code within the ``` <head> ``` element:
```
<component type="typeof(RadzenTheme)" render-mode="WebAssemblyPrerendered" param-Theme="@("material")" />
```

## 4. Include Radzen.Blazor.js

Open the <b> _Host.cshtml </b> file of your application. Add this code after the last ```<script>```:

```
<script src="_content/Radzen.Blazor/Radzen.Blazor.js?v=@(typeof(Radzen.Colors).Assembly.GetName().Version)"></script>
```

