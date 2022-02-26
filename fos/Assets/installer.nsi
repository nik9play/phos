ManifestDPIAware true

!define PRODUCT_NAME "phos"
!define PRODUCT_VERSION "0.0.1-alpha"
!define PRODUCT_PUBLISHER "megaworld"
!define PRODUCT_WEB_SITE "https://megaworld.space"

!include "FileFunc.nsh"

!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_AUTOSTART_KEY "Software\Microsoft\Windows\CurrentVersion\Run\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_UNINST_AUTOSTART_ROOT_KEY "HKCU"

!include "MUI2.nsh"
!define MUI_ABORTWARNING
!define MUI_ICON "Icon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\nsis3-metro.bmp"
!insertmacro MUI_PAGE_WELCOME
;!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\phos.exe"
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Russian"

Name "${PRODUCT_NAME}"
OutFile "phos_setup${PRODUCT_VERSION}.exe"
InstallDir "$LocalAppData\Programs\phos"
ShowInstDetails show
ShowUnInstDetails show

Section /o "$(CreateDeskShort)"
  CreateShortCut "$DESKTOP\phos.lnk" "$INSTDIR\phos.exe"
SectionEnd

Section "$(CreateStartShort)"
  CreateShortCut "$SMPROGRAMS\phos.lnk" "$INSTDIR\phos.exe"
SectionEnd

Section "phos" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File "..\bin\Release\net6.0\publish\phos.exe"
  File "..\bin\Release\net6.0\publish\phos.dll"
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
  File "..\bin\Release\net6.0\publish\Microsoft.Toolkit.Mvvm.dll"
  File "..\bin\Release\net6.0\publish\phos.deps.json"
  File "..\bin\Release\net6.0\publish\phos.runtimeconfig.json"
  SetOutPath "$INSTDIR\ru"
  File "..\bin\Release\net6.0\publish\ru\phos.resources.dll"
  SetOutPath "$INSTDIR\ru-RU"
  File "..\bin\Release\net6.0\publish\ru-RU\ModernWpf.resources.dll"
  File "..\bin\Release\net6.0\publish\ru-RU\ModernWpf.Controls.resources.dll"
  SetOutPath "$INSTDIR"
SectionEnd

Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
  SectionSetFlags ${SEC01} 17
FunctionEnd

;Localization
LangString CreateDeskShort ${LANG_ENGLISH} "Create desktop shortcut"
LangString CreateDeskShort ${LANG_RUSSIAN} "Создать ярлык на рабочем столе"
LangString CreateStartShort ${LANG_ENGLISH} "Create Start menu shortcut"
LangString CreateStartShort ${LANG_RUSSIAN} "Создать ярлык в меню Пуск"
LangString UninstQuestion ${LANG_ENGLISH} "Are you sure you want to completely remove phos?"
LangString UninstQuestion ${LANG_RUSSIAN} "Вы уверены что хотите удалить phos?"
LangString UninstDone ${LANG_ENGLISH} "Application was successfully removed from your computer."
LangString UninstDone ${LANG_RUSSIAN} "Приложение было успешно удалено."

Section -Post
  ;Following lines will make uninstaller work - do not change anything, unless you really want to.
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\phos.exe,0"
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoRepair" "1"
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoModify" "1"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"

  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "EstimatedSize" "$0"
SectionEnd

;Function un.onUninstSuccess
;  HideWindow
;  MessageBox MB_ICONINFORMATION|MB_OK "$(UninstDone)"
;FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "$(UninstQuestion)" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\phos.exe"
  Delete "$INSTDIR\phos.dll"
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
  Delete "$INSTDIR\Microsoft.Toolkit.Mvvm.dll"
  Delete "$INSTDIR\phos.deps.json"
  Delete "$INSTDIR\phos.runtimeconfig.json"

  Delete "$INSTDIR\ru\phos.resources.dll"
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
  DeleteRegKey ${PRODUCT_UNINST_AUTOSTART_ROOT_KEY} "${PRODUCT_AUTOSTART_KEY}"

  SetAutoClose true
SectionEnd