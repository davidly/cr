# dotnet build -c Release

dotnet publish cr.csproj --configuration Release -r osx.12-arm64 -f net6.0 --no-self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./ -nologo
