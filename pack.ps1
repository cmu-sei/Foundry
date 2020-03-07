mkdir -silent nuget-local
Get-ChildItem -Path .\lib | Foreach-Object {
    dotnet build .\lib\$_
    dotnet pack -o nuget-local .\lib\$_
}
