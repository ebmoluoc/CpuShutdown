[Setup]
AppId={{CDF8D637-2981-4038-A383-0441BB037CCC}
SetupMutex=Global\CDF8D637-2981-4038-A383-0441BB037CCC
AppCopyright=Copyright (c) 2021 Philippe Coulombe
AppPublisher=Philippe Coulombe
AppVersion=1.2.0.0
VersionInfoVersion=1.2.0.0
AppVerName=CPU Shutdown 1.2
AppName=CPU Shutdown
DefaultDirName={commonpf}\CPU Shutdown
UninstallDisplayIcon={app}\CpuShutdown.UI.Tray.exe
OutputBaseFilename=CpuShutdownSetup
OutputDir=.
LicenseFile=LICENSE
DisableProgramGroupPage=yes
DisableDirPage=yes
SolidCompression=yes
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
MinVersion=6.1.7601
WizardSizePercent=120,100

[Files]
Source: "LICENSE"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "Publish\*"; Excludes: "appsettings.json"; DestDir: {app}; Flags: restartreplace uninsrestartdelete ignoreversion
Source: "Publish\appsettings.json"; DestDir: {app}; Flags: restartreplace uninsrestartdelete onlyifdoesntexist

[Run]
Filename: {sys}\sc.exe; Parameters: "create CpuShutdownSvc displayname= ""CPU Shutdown"" start= auto depend= SENS binpath= ""{app}\CpuShutdown.Service.exe -g:C6BAB326-3F3B-4686-8DE8-AD8C198943D2"""; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "description CpuShutdownSvc ""Shuts down the system when a critical CPU temperature is reached."""; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "start CpuShutdownSvc"; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop CpuShutdownSvc"; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete CpuShutdownSvc"; Flags: runhidden

[Code]
procedure InitializeWizard();
begin
  WizardForm.LicenseMemo.Font.Name := 'Consolas';
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Exec('sc.exe', 'stop CpuShutdownSvc', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Exec('sc.exe', 'delete CpuShutdownSvc', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Result := '';
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
begin
  if CurUninstallStep = usUninstall then begin
    Exec('sc.exe', 'stop CpuShutdownSvc', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Exec('sc.exe', 'delete CpuShutdownSvc', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;
