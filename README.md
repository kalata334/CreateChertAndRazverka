# CreateChert And Razverka

Windows Forms приложение (.NET Framework 4.8, x64) для автоматического создания чертежей и развёрток в SolidWorks 2022.

## Требования

| Компонент       | Версия / Значение                     |
|-----------------|---------------------------------------|
| ОС              | Windows 10 / Windows 11               |
| .NET Framework  | 4.8                                   |
| SolidWorks      | 2022 (ProgID: `SldWorks.Application.30`) |
| Visual Studio   | 2022                                  |
| Разрядность     | x64 (обязательно)                     |

## Структура проекта

```
CreateChertAndRazverka.sln
└── CreateChertAndRazverka/
    ├── Program.cs
    ├── MainForm.cs / MainForm.Designer.cs / MainForm.resx
    ├── app.manifest
    ├── Properties/
    │   ├── AssemblyInfo.cs
    │   ├── Settings.settings / Settings.Designer.cs
    │   └── Resources.resx
    ├── Core/
    │   ├── SolidWorksConnector.cs   — подключение к SolidWorks через COM
    │   ├── DocumentMonitor.cs       — мониторинг активного документа (1500 мс)
    │   ├── DrawingCreator.cs        — создание чертежей деталей
    │   ├── FlatPatternCreator.cs    — создание развёрток листовых деталей
    │   ├── AssemblyDrawingCreator.cs — сборочный чертёж с BOM и выносками
    │   ├── DimensionManager.cs      — автоматическая простановка размеров
    │   └── TitleBlockManager.cs     — заполнение штампа
    ├── Models/
    │   ├── DrawingSettings.cs
    │   ├── ComponentInfo.cs
    │   ├── DocumentState.cs
    │   └── GenerationResult.cs
    ├── Helpers/
    │   ├── AdminHelper.cs           — проверка и запрос прав администратора
    │   ├── FileHelper.cs            — работа с файловой системой
    │   ├── LogHelper.cs             — цветной лог в RichTextBox
    │   └── PdfExporter.cs           — экспорт чертежей в PDF
    └── References/
        ├── SolidWorks.Interop.sldworks.dll     (скопировать из установки SW2022)
        ├── SolidWorks.Interop.swconst.dll      (скопировать из установки SW2022)
        └── SolidWorks.Interop.swpublished.dll  (скопировать из установки SW2022)
```

## Сборка

1. Скопируйте SolidWorks Interop DLL из папки установки SolidWorks 2022  
   (обычно `C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist\`) в папку `CreateChertAndRazverka\References\`.
2. Откройте `CreateChertAndRazverka.sln` в Visual Studio 2022.
3. Выберите конфигурацию **Release | x64**.
4. Соберите проект (Ctrl+Shift+B).

## Запуск

Приложение требует прав администратора (прописано в `app.manifest`).  
При первом запуске Windows UAC запросит подтверждение.

## Возможности

- 🔄 Автоматический мониторинг активного документа SolidWorks каждые 1500 мс  
- 📄 Создание чертежей деталей с автоматической простановкой размеров  
- 📐 Создание развёрток листовых деталей  
- 📋 Сборочный чертёж с таблицей BOM и выносными позициями  
- 📁 Сохранение в указанную папку  
- 📄 Дополнительный экспорт в PDF  
- ✍️ Вписывание имени автора в штамп каждого чертежа
