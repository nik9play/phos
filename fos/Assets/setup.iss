#define phosAppName "phos"
#define phosAppVersion "0.0.2"
#define phosAppPublisher "megaworld"
#define phosAppURL "https://megaworld.space"
#define phosAppExeName "phos.exe"

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
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\{#phosAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\DebounceThrottle.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\GongSolutions.WPF.DragDrop.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\Hardcodet.NotifyIcon.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\MdXaml.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\Microsoft.Toolkit.Mvvm.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\Microsoft.Toolkit.Uwp.Notifications.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\Microsoft.Windows.SDK.NET.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ModernWpf.Controls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ModernWpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\NHotkey.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\NHotkey.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\phos.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\System.Management.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\WindowsDisplayAPI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\WinRT.Runtime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\NJsonSchema.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\Namotion.Reflection.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\phos.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\phos.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\Assets\welcome_video.mp4"; DestDir: "{app}\Assets"; Flags: ignoreversion
;Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ru\*"; DestDir: "{app}\ru"; Flags: ignoreversion recursesubdirs createallsubdirs
;Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ru-RU\*"; DestDir: "{app}\ru-RU"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ru\phos.resources.dll"; DestDir: "{app}\ru"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ru-RU\ModernWpf.Controls.resources.dll"; DestDir: "{app}\ru-RU"; Flags: ignoreversion
Source: "C:\Users\nik9\source\repos\fos\fos\bin\Release\net6.0\publish\ru-RU\ModernWpf.resources.dll"; DestDir: "{app}\ru-RU"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#phosAppName}"; Filename: "{app}\{#phosAppExeName}"
Name: "{autodesktop}\{#phosAppName}"; Filename: "{app}\{#phosAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#phosAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(phosAppName, '&', '&&')}}"; Flags: nowait postinstall
