# CreateChert And Razverka

**Версия 1.0.0** — Windows Forms приложение (.NET Framework 4.8, x64) для автоматического создания чертежей и развёрток в SolidWorks 2022.

## Возможности

- 🔄 Автоматический мониторинг активного документа SolidWorks каждые 1500 мс
- 📄 Создание чертежей деталей с автоматической простановкой размеров (виды: спереди, сверху, справа, изометрия)
- 📐 Создание развёрток листовых деталей (автоопределение листового металла)
- 📋 Сборочный чертёж с таблицей BOM и выносными позициями
- ✍️ Заполнение штампа ГОСТ 2.104-2006 с именем автора
- 📁 Сохранение в указанную папку (.slddrw)
- 📄 Экспорт в PDF
- ⛔ Отмена генерации в любой момент

## Требования

| Компонент      | Версия / Значение                        |
|----------------|------------------------------------------|
| ОС             | Windows 10 / Windows 11                  |
| .NET Framework | 4.8                                      |
| SolidWorks     | 2022 (ProgID: `SldWorks.Application.30`) |
| Visual Studio  | 2022                                     |
| Разрядность    | x64 (обязательно)                        |

## Структура проекта

```
CreateChertAndRazverka.sln
├── CreateChertAndRazverka/
│   ├── Program.cs
│   ├── MainForm.cs / MainForm.Designer.cs / MainForm.resx
│   ├── app.manifest
│   ├── Properties/
│   │   ├── AssemblyInfo.cs
│   │   ├── Settings.settings / Settings.Designer.cs
│   │   └── Resources.resx
│   ├── Core/
│   │   ├── SolidWorksConnector.cs    — подключение к SolidWorks через COM (dynamic)
│   │   ├── DocumentMonitor.cs        — мониторинг активного документа каждые 1500 мс
│   │   ├── DrawingCreator.cs         — создание чертежей деталей
│   │   ├── FlatPatternCreator.cs     — создание развёрток листовых деталей
│   │   ├── AssemblyDrawingCreator.cs — сборочный чертёж с BOM и выносками
│   │   ├── DimensionManager.cs       — автоматическая простановка размеров
│   │   └── TitleBlockManager.cs      — заполнение штампа ГОСТ 2.104-2006
│   ├── Models/
│   │   ├── DrawingSettings.cs
│   │   ├── ComponentInfo.cs
│   │   ├── DocumentState.cs
│   │   └── GenerationResult.cs
│   ├── Helpers/
│   │   ├── AdminHelper.cs            — проверка и запрос прав администратора
│   │   ├── FileHelper.cs             — работа с файловой системой
│   │   ├── LogHelper.cs              — цветной лог в RichTextBox
│   │   └── PdfExporter.cs            — экспорт чертежей в PDF
│   └── References/                   — папка для реальных Interop DLL (не в репозитории)
├── Interop/
│   └── SolidWorksInteropStubs.cs     — stub-интерфейсы SolidWorks API (документация)
└── Installer/
    └── setup.iss                     — скрипт установщика Inno Setup
```

## Автоматический поиск SolidWorks Interop DLL

Программа **автоматически** ищет SolidWorks Interop DLL при запуске. Ручное копирование DLL не требуется.

### Порядок поиска при запуске

При старте программа регистрирует `AssemblyResolve`-обработчик, который ищет DLL в следующем порядке:

1. Пути из файла `InteropPaths.txt` (рядом с exe) — пользовательские пути
2. `D:\Solid\SolidWorks1\SOLIDWORKS\api\redist\` — путь пользователя
3. `C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist\` — стандартный путь
4. `C:\Program Files\SolidWorks Corp\SOLIDWORKS\api\redist\` — альтернативный путь
5. Реестр Windows: `HKLM\SOFTWARE\SolidWorks\SOLIDWORKS 2022\Setup\SolidWorks Folder`
6. Папка рядом с exe файлом программы

### Указать путь вручную (InteropPaths.txt)

Если SolidWorks установлен в нестандартное место, откройте файл `InteropPaths.txt`
(находится рядом с exe или в папке проекта) и добавьте свой путь:

```
# Пути для поиска SolidWorks Interop DLL (по одному пути на строку)
E:\MySolidWorks\SOLIDWORKS\api\redist
```

### Pre-Build Event (автокопирование при сборке)

При сборке проекта автоматически запускается скрипт `CopyInteropDlls.bat`, который
копирует DLL из найденного пути в папку `References\`. Это позволяет использовать
раннее связывание (early-binding) без ручного копирования файлов.

## Сборка без SolidWorks Interop DLL (по умолчанию)

Приложение использует **позднее связывание (late-binding / dynamic)** для вызова COM API SolidWorks. Это означает, что **SolidWorks Interop DLL не требуются** для компиляции проекта.

1. Откройте `CreateChertAndRazverka.sln` в Visual Studio 2022.
2. Выберите конфигурацию **Release | x64**.
3. Соберите проект (`Ctrl+Shift+B`).

## Замена заглушек на реальные Interop DLL (опционально)

Для раннего связывания (early-binding) и поддержки IntelliSense по типам SolidWorks:

1. Скопируйте следующие DLL из папки установки SolidWorks 2022 в папку `CreateChertAndRazverka\References\`:

   ```
   D:\Solid\SolidWorks1\SOLIDWORKS\api\redist\SolidWorks.Interop.sldworks.dll
   D:\Solid\SolidWorks1\SOLIDWORKS\api\redist\SolidWorks.Interop.swconst.dll
   D:\Solid\SolidWorks1\SOLIDWORKS\api\redist\SolidWorks.Interop.swpublished.dll
   ```

   Стандартный путь установки SolidWorks:
   ```
   C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist\
   ```

2. В файле `CreateChertAndRazverka.csproj` раскомментируйте блок `<Reference>`:
   ```xml
   <Reference Include="SolidWorks.Interop.sldworks">
     <HintPath>References\SolidWorks.Interop.sldworks.dll</HintPath>
     <EmbedInteropTypes>False</EmbedInteropTypes>
   </Reference>
   <Reference Include="SolidWorks.Interop.swconst">
     <HintPath>References\SolidWorks.Interop.swconst.dll</HintPath>
     <EmbedInteropTypes>False</EmbedInteropTypes>
   </Reference>
   <Reference Include="SolidWorks.Interop.swpublished">
     <HintPath>References\SolidWorks.Interop.swpublished.dll</HintPath>
     <EmbedInteropTypes>False</EmbedInteropTypes>
   </Reference>
   ```

3. Пересоберите проект.

> Файл `Interop/SolidWorksInteropStubs.cs` содержит минимальные stub-определения всех
> использованных интерфейсов, классов и перечислений SolidWorks API для справки.

## Запуск

Приложение требует прав администратора (прописано в `app.manifest`).  
При первом запуске Windows UAC запросит подтверждение. Взаимодействие с SolidWorks 2022
через COM API требует повышенных привилегий.

## Установщик

Папка `Installer/` содержит скрипт `setup.iss` для [Inno Setup](https://jrsoftware.org/isinfo.php) 6.x.

Перед сборкой установщика:
1. Выполните сборку проекта в конфигурации **Release | x64**.
2. Откройте `Installer/setup.iss` в Inno Setup Compiler.
3. Нажмите **Build → Compile**.

Установщик проверяет наличие .NET Framework 4.8 и предупреждает об отсутствии SolidWorks 2022.

## Лицензия

Copyright © 2024. Все права защищены.
