Echo Creates Debug Packages and pushes to Local Nuget Repo

dotnet pack -o ..\packages ..\src\TBD

for %%n in (..\packages\release\*.nupkg) do  dotnet nuget push -s d:\a_dev\LocalNugetPackages "%%n"
