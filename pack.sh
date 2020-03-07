#!/bin/bash

mkdir -p nuget-local

for i in $(ls lib/); do
    dotnet build ./lib/$i
    dotnet pack -o nuget-local ./lib/$i
done
