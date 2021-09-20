$target = "..\Core.lib\lib\net45"
Copy-Item "..\bin\Debug\Core.dll" $target
Copy-Item "..\Core.Internet\bin\Debug\Core.Internet.dll" $target
Copy-Item "..\Core.Data\bin\Debug\Core.Data.dll" $target
Copy-Item "..\Core.WinForms\bin\Debug\Core.WinForms.dll" $target
Copy-Item "..\Core.Markup\bin\Debug\Core.Markup.dll" $target
Copy-Item "..\Core.Json\bin\Debug\Core.Json.dll" $target
Copy-Item "..\Core.Git\bin\Debug\Core.Git.dll" $target
Copy-Item "..\Core.Services\bin\Debug\Core.Services.dll" $target
Push-Location "..\Core.lib"
Remove-Item "*.nupkg"
& 'C:\Users\tebennett\Utilities\nuget.exe' pack .\Core.nuspec
Pop-Location