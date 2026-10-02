@echo off
rem Fantastik Dunya - acik dunya dilimi: derle, modelleri ice aktar, oyunu ac.
setlocal
set ROOT=%~dp0
set GODOT=%ROOT%_tools\Godot_v4.7.2-stable_mono_win64\Godot_v4.7.2-stable_mono_win64.exe
if not exist "%GODOT%" (
  echo Godot bulunamadi: %GODOT%
  pause
  exit /b 1
)
where dotnet >nul 2>nul
if errorlevel 1 goto nodotnet
dotnet --list-sdks | findstr /b "8." >nul
if errorlevel 1 goto nodotnet
cd /d "%ROOT%godot"
echo [1/3] C# derleniyor...
dotnet build -nologo -v q
if errorlevel 1 (
  echo Derleme basarisiz.
  pause
  exit /b 1
)
if not exist "%ROOT%godot\.godot\imported" (
  echo [2/3] Modeller ice aktariliyor (ilk acilista bir kez, ~1 dk)...
  "%GODOT%" --headless --path "%ROOT%godot" --import
)
echo [3/3] Oyun aciliyor...
start "" "%GODOT%" --path "%ROOT%godot"
exit /b 0

:nodotnet
echo .NET 8 SDK bulunamadi. Kurmak icin bir komut satirinda:
echo    winget install Microsoft.DotNet.SDK.8
echo sonra bu dosyayi yeniden calistir.
pause
exit /b 1
