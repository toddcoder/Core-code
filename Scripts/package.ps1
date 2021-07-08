$target = "..\Core.lib\lib\net45"
Copy-Item "..\bin\Debug\Core.dll" $target
Copy-Item "..\Core.Internet\bin\Debug\Core.Internet.dll" $target
Copy-Item "..\Core.Data\bin\Debug\Core.Data.dll" $target
Copy-Item "..\Core.WinForms\bin\Debug\Core.WinForms.dll" $target
Push-Location "..\Core.lib"
Remove-Item "*.nupkg"
& 'C:\Program Files\dotnet\nuget.exe' pack .\Core.nuspec
Pop-Location