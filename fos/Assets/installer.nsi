; Main constants - define following constants as you want them displayed in your installation wizard
!define PRODUCT_NAME "phos"
!define PRODUCT_VERSION "1.0.0.0-alpha"
!define PRODUCT_PUBLISHER "megaworld"
!define PRODUCT_WEB_SITE "https://megaworld.space"

; Following constants you should never change
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!include "MUI2.nsh"
!define MUI_ABORTWARNING
!define MUI_ICON "Icon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Wizard pages
!insertmacro MUI_PAGE_WELCOME
; Note: you should create License.txt in the same folder as this file, or remove following line.
;!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\fos.exe"
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Russian"
; Replace the constants bellow to hit suite your project
Name "${PRODUCT_NAME}"
OutFile "phos_setup${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES64\phos"
ShowInstDetails show
ShowUnInstDetails show

Section /o "Desktop shortcut"
  CreateShortCut "$DESKTOP\phos.lnk" "$INSTDIR\fos.exe"
SectionEnd

Section /o "Start menu shortcut"
  CreateShortCut "$SMPROGRAMS\phos.lnk" "$INSTDIR\fos.exe"
SectionEnd

Section "phos" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File "..\bin\Release\net6.0\publish\fos.exe"
  File "..\bin\Release\net6.0\publish\fos.dll"
  File "..\bin\Release\net6.0\publish\DebounceThrottle.dll"
  File "..\bin\Release\net6.0\publish\GongSolutions.WPF.DragDrop.dll"
  File "..\bin\Release\net6.0\publish\Hardcodet.NotifyIcon.Wpf.dll"
  File "..\bin\Release\net6.0\publish\Microsoft.Toolkit.Uwp.Notifications.dll"
  File "..\bin\Release\net6.0\publish\Microsoft.Windows.SDK.NET.dll"
  File "..\bin\Release\net6.0\publish\ModernWpf.Controls.dll"
  File "..\bin\Release\net6.0\publish\ModernWpf.dll"
  File "..\bin\Release\net6.0\publish\NHotkey.dll"
  File "..\bin\Release\net6.0\publish\NHotkey.Wpf.dll"
  File "..\bin\Release\net6.0\publish\System.Management.dll"
  File "..\bin\Release\net6.0\publish\WindowsDisplayAPI.dll"
  File "..\bin\Release\net6.0\publish\WinRT.Runtime.dll"
  File "..\bin\Release\net6.0\publish\fos.deps.json"
  File "..\bin\Release\net6.0\publish\fos.runtimeconfig.json"
  SetOutPath "$INSTDIR\ru"
  File "..\bin\Release\net6.0\publish\ru\fos.resources.dll"
  SetOutPath "$INSTDIR\ru-RU"
  File "..\bin\Release\net6.0\publish\ru-RU\ModernWpf.resources.dll"
  File "..\bin\Release\net6.0\publish\ru-RU\ModernWpf.Controls.resources.dll"
  SetOutPath "$INSTDIR"

; It is pretty clear what following line does: just rename the file name to your project startup executable.
  ;CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\fos.exe" ""
SectionEnd

Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
  SectionSetFlags ${SEC01} 17
FunctionEnd

Section -AdditionalIcons
  CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\fos.exe" ""
SectionEnd

Section -Post
  ;Following lines will make uninstaller work - do not change anything, unless you really want to.
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  
  ; COOL STUFF: Following line will add a registry setting that will add the INSTDIR into the list of folders from where
  ; the assemblies are listed in the Add Reference in C# or Visual Studio.
  ; This is super-cool if your installation package contains assemblies that someone will use to build more applications - 
  ; and it doesn't hurt even if it is placed there, it will only make the VS a bit slower to find all assemblies when adding references.
  ;WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "SOFTWARE\Microsoft\.NETFramework\v2.0.50727\AssemblyFoldersEx\ZWare\ZawsCC" "" "$INSTDIR"
SectionEnd

; Replace the following strings to suite your needs
Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "Application was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove phos and all of its components?" IDYES +2
  Abort
FunctionEnd

; Remove any file that you have added above - removing uninstallation and folders last.
; Note: if there is any file changed or added to these folders, they will not be removed. Also, parent folder (which in my example 
; is company name ZWare) will not be removed if there is any other application installed in it.
Section Uninstall
  Delete "$INSTDIR\fos.exe"
  Delete "$INSTDIR\fos.dll"
  Delete "$INSTDIR\DebounceThrottle.dll"
  Delete "$INSTDIR\GongSolutions.WPF.DragDrop.dll"
  Delete "$INSTDIR\Hardcodet.NotifyIcon.Wpf.dll"
  Delete "$INSTDIR\Microsoft.Toolkit.Uwp.Notifications.dll"
  Delete "$INSTDIR\Microsoft.Windows.SDK.NET.dll"
  Delete "$INSTDIR\ModernWpf.Controls.dll"
  Delete "$INSTDIR\ModernWpf.dll"
  Delete "$INSTDIR\NHotkey.dll"
  Delete "$INSTDIR\NHotkey.Wpf.dll"
  Delete "$INSTDIR\System.Management.dll"
  Delete "$INSTDIR\WindowsDisplayAPI.dll"
  Delete "$INSTDIR\WinRT.Runtime.dll"
  Delete "$INSTDIR\fos.deps.json"
  Delete "$INSTDIR\fos.runtimeconfig.json"

  Delete "$INSTDIR\ru\fos.resources.dll"
  Delete "$INSTDIR\ru-RU\ModernWpf.resources.dll"
  Delete "$INSTDIR\ru-RU\ModernWpf.Controls.resources.dll"

  RMDir "$INSTDIR\ru"
  RMDir "$INSTDIR\ru-RU"
  Delete "$INSTDIR\uninst.exe"

  RMDir "$INSTDIR"
  RMDir "$INSTDIR\.."

  Delete "$DESKTOP\phos.lnk"
  Delete "$SMPROGRAMS\phos.lnk"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"

  SetAutoClose true
SectionEnd