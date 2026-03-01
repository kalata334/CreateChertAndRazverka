@echo off
setlocal EnableDelayedExpansion

set REFS_DIR=%~dp0References

REM Создаём папку References если не существует
if not exist "%REFS_DIR%" mkdir "%REFS_DIR%"

set DLL1=SolidWorks.Interop.sldworks.dll
set DLL2=SolidWorks.Interop.swconst.dll
set DLL3=SolidWorks.Interop.swpublished.dll

REM Проверяем — если все DLL уже есть (размер > 10KB), пропускаем
set SKIP=1
for %%F in (%DLL1% %DLL2% %DLL3%) do (
    set FPATH=%REFS_DIR%\%%F
    if not exist "!FPATH!" set SKIP=0
    if exist "!FPATH!" (
        for %%S in ("!FPATH!") do (
            if %%~zS LSS 10240 set SKIP=0
        )
    )
)

if "%SKIP%"=="1" (
    echo [CopyInteropDlls] SolidWorks Interop DLL уже присутствуют в папке References — пропускаем.
    goto :eof
)

REM Список путей поиска
set PATHS[0]=D:\Solid\SolidWorks1\SOLIDWORKS\api\redist
set PATHS[1]=C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist
set PATHS[2]=C:\Program Files\SolidWorks Corp\SOLIDWORKS\api\redist

set FOUND_PATH=

for /L %%i in (0,1,2) do (
    if "!FOUND_PATH!"=="" (
        set CANDIDATE=!PATHS[%%i]!
        if exist "!CANDIDATE!\%DLL1%" (
            set FOUND_PATH=!CANDIDATE!
        )
    )
)

if "!FOUND_PATH!"=="" (
    echo [CopyInteropDlls] ПРЕДУПРЕЖДЕНИЕ: SolidWorks Interop DLL не найдены.
    echo [CopyInteropDlls] Проверенные пути:
    for /L %%i in (0,1,2) do echo   !PATHS[%%i]!
    echo [CopyInteropDlls] Сборка продолжается без Interop DLL (используется позднее связывание).
    goto :eof
)

echo [CopyInteropDlls] Найдены DLL в: !FOUND_PATH!
echo [CopyInteropDlls] Копирование в: %REFS_DIR%

for %%F in (%DLL1% %DLL2% %DLL3%) do (
    if exist "!FOUND_PATH!\%%F" (
        copy /Y "!FOUND_PATH!\%%F" "%REFS_DIR%\%%F" >nul
        echo [CopyInteropDlls] Скопирован: %%F
    ) else (
        echo [CopyInteropDlls] ПРЕДУПРЕЖДЕНИЕ: файл не найден: !FOUND_PATH!\%%F
    )
)

echo [CopyInteropDlls] Готово.
endlocal
