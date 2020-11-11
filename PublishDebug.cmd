rmdir /s /q Publish
msbuild.exe CpuShutdown.sln /t:Clean /p:Configuration=Debug /p:PublishSingleFile=False /p:SelfContained=False /p:PublishProtocol=FileSystem /p:PublishReadyToRun=False /p:PublishTrimmed=False /p:PublishDir=..\Publish
msbuild.exe CpuShutdown.sln /t:Publish /p:Configuration=Debug /p:PublishSingleFile=False /p:SelfContained=False /p:PublishProtocol=FileSystem /p:PublishReadyToRun=False /p:PublishTrimmed=False /p:PublishDir=..\Publish
