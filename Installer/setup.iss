; CreateChert And Razverka — Inno Setup Script
; Requires Inno Setup 6.x  (https://jrsoftware.org/isinfo.php)

#define AppName      "CreateChert And Razverka"
#define AppVersion   "1.0.0"
#define AppPublisher "kalata334"
#define AppExeName   "CreateChertAndRazverka.exe"
#define AppId        "{{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}"

[Setup]
AppId={#AppId}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL=https://github.com/kalata334/CreateChertAndRazverka
AppSupportURL=https://github.com/kalata334/CreateChertAndRazverka/issues
AppUpdatesURL=https://github.com/kalata334/CreateChertAndRazverka
DefaultDirName={autopf64}\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
OutputDir=Output
OutputBaseFilename=CreateChertAndRazverka_Setup_v{#AppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; Minimum Windows version: Windows 7 SP1 (6.1.7601)
MinVersion=6.1.7601

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Main executable and application files
; Build Release|x64 in Visual Studio before running this installer script.
Source: "..\CreateChertAndRazverka\bin\x64\Release\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\CreateChertAndRazverka\bin\x64\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\CreateChertAndRazverka\bin\x64\Release\*.config"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
// ─── Check .NET Framework 4.8 ────────────────────────────────────────────────
function IsDotNet48Installed(): Boolean;
var
  Release: Cardinal;
begin
  Result := False;
  if RegQueryDWordValue(HKLM,
      'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full',
      'Release', Release) then
    // 528040 = .NET 4.8 on Windows 10 May 2019 Update or later
    Result := (Release >= 528040);
end;

// ─── Check SolidWorks 2022 ────────────────────────────────────────────────────
function IsSolidWorks2022Installed(): Boolean;
var
  SWPath: String;
begin
  Result := RegQueryStringValue(HKLM,
      'SOFTWARE\SolidWorks\SOLIDWORKS 2022\Setup',
      'SolidWorks Folder', SWPath);
  if not Result then
    Result := RegQueryStringValue(HKCU,
        'SOFTWARE\SolidWorks\SOLIDWORKS 2022\Setup',
        'SolidWorks Folder', SWPath);
end;

procedure InitializeWizard();
begin
  // No-op — checks are performed in NextButtonClick
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;

  if CurPageID = wpWelcome then
  begin
    // Check .NET 4.8
    if not IsDotNet48Installed() then
    begin
      if MsgBox(
          'Microsoft .NET Framework 4.8 не обнаружен.' + #13#10 +
          'Приложение CreateChert And Razverka требует .NET Framework 4.8.' + #13#10 + #13#10 +
          'Загрузить .NET Framework 4.8 можно по адресу:' + #13#10 +
          'https://dotnet.microsoft.com/download/dotnet-framework/net48' + #13#10 + #13#10 +
          'Продолжить установку без .NET Framework 4.8?',
          mbError, MB_YESNO) = IDNO then
        Result := False;
    end;

    // Check SolidWorks 2022
    if Result and not IsSolidWorks2022Installed() then
    begin
      MsgBox(
          'SolidWorks 2022 не обнаружен на этом компьютере.' + #13#10 +
          'CreateChert And Razverka требует SolidWorks 2022.' + #13#10 + #13#10 +
          'Установка будет продолжена, однако приложение не будет' + #13#10 +
          'функционировать без запущенного SolidWorks 2022.',
          mbInformation, MB_OK);
    end;
  end;
end;
