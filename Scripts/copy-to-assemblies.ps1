Push-Location "C:\Users\tebennett\source\repos\toddcoder\Core"

$target = "Assemblies"

robocopy "bin\Debug" $target Core.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Internet\bin\Debug" $target Core.Internet.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Data\bin\Debug" $target Core.Data.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.WinForms\bin\Debug" $target Core.WinForms.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Markup\bin\Debug" $target Core.Markup.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Json\bin\Debug" $target Core.Json.dll Newtonsoft.Json.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Git\bin\Debug" $target Core.Git.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Services\bin\Debug" $target Core.Services.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Zip\bin\Debug" $target Core.Zip.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
robocopy "Core.Io\bin\Debug" $target Core.Io.dll /xf *.xml *.config *.pdb /NDL /NJH /NJS /nc /ns
Pop-Location