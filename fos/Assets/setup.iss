#define phosAppName "phos"
#define phosAppVersion "0.0.5"
#define phosAppPublisher "megaworld"
#define phosAppURL "https://megaworld.space"
#define phosAppExeName "phos.exe"
#define publishFolder "..\bin\Release\net6.0-windows10.0.18362.0\win-x64\publish"

[Setup]
AppId={{1D469005-80D5-4B6F-B614-35387F782D64}
AppName={#phosAppName}
AppVersion={#phosAppVersion}
AppMutex="phos.megaworld"
;AppVerName={#phosAppName} {#phosAppVersion}
AppPublisher={#phosAppPublisher}
AppPublisherURL={#phosAppURL}
AppSupportURL={#phosAppURL}
AppUpdatesURL={#phosAppURL}
UninstallDisplayName={#phosAppName}
UninstallDisplayIcon="{app}\{#phosAppExeName},0"
SetupIconFile="Icon2.ico"
RestartApplications=false
CloseApplications=yes
ArchitecturesAllowed=x64
DefaultDirName={autopf}\{#phosAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputDir=.\                                                                                                        
OutputBaseFilename=phos_setup{#phosAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#publishFolder}\{#phosAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\DebounceThrottle.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\GongSolutions.WPF.DragDrop.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\H.Hooks.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\H.NotifyIcon.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\H.NotifyIcon.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\Microsoft.Toolkit.Mvvm.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\Microsoft.Toolkit.Uwp.Notifications.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\Microsoft.Windows.SDK.NET.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\ModernWpf.Controls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\ModernWpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\NHotkey.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\NHotkey.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\phos.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\System.Management.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\WindowsDisplayAPI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\WinRT.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\NJsonSchema.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\Namotion.Reflection.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\Octokit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\phos.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\phos.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#publishFolder}\Assets\welcome_video.mp4"; DestDir: "{app}\Assets"; Flags: ignoreversion
;Source: "{#publishFolder}\ru\*"; DestDir: "{app}\ru"; Flags: ignoreversion recursesubdirs createallsubdirs
;Source: "{#publishFolder}\ru-RU\*"; DestDir: "{app}\ru-RU"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#publishFolder}\ru\phos.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
Source: "{#publishFolder}\ru-RU\ModernWpf.Controls.resources.dll"; DestDir: "{app}\ru-RU"; Flags: ignoreversion
Source: "{#publishFolder}\ru-RU\ModernWpf.resources.dll"; DestDir: "{app}\ru-RU"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#phosAppName}"; Filename: "{app}\{#phosAppExeName}"
Name: "{autodesktop}\{#phosAppName}"; Filename: "{app}\{#phosAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#phosAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(phosAppName, '&', '&&')}}"; Flags: nowait postinstall
