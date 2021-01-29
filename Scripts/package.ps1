$target = "..\Core.lib\lib\net45"
copy "..\bin\Debug\Core.dll" $target
copy "..\Core.Internet\bin\Debug\Core.Internet.dll" $target
copy "..\Core.Data\bin\Debug\Core.Data.dll" $target
copy "..\Core.ObjectGraphs\bin\Debug\Core.ObjectGraphs.dll" $target
pushd "..\Core.lib"
del "*.nupkg"
& 'C:\Program Files\dotnet\nuget.exe' pack .\Core.nuspec
popd