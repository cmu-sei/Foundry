# Stack.Http

This library provides common http components

## Usage

`dotnet add package Stack.Http -s https://nuget.cwd.local/v3/index.json`

You should also add a `NuGet.Config` file to your relying project so that everybody restores from the same repo.

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.cwd.local" value="https://nuget.cwd.local/v3/index.json" />
  </packageSources>
</configuration>
```
