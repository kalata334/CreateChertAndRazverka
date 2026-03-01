using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CreateChertAndRazverka.Core;
using CreateChertAndRazverka.Helpers;
using CreateChertAndRazverka.Models;

namespace CreateChertAndRazverka
{
    public partial class MainForm : Form
    {
        // ── SW infrastructure ────────────────────────────────────────────────
        private readonly SolidWorksConnector _connector;
        private readonly DocumentMonitor     _monitor;
        private DocumentState                _currentState = DocumentState.Empty;

        // ── Creators ─────────────────────────────────────────────────────────
        private DrawingCreator         _drawingCreator;
        private FlatPatternCreator     _flatCreator;
        private AssemblyDrawingCreator _asmCreator;

        // ── Cancellation ─────────────────────────────────────────────────────
        private CancellationTokenSource _cts;

        // ── Component list (backing data) ─────────────────────────────────────
        private readonly List<ComponentInfo> _components = new List<ComponentInfo>();

        public MainForm()
        {
            InitializeComponent();

            _connector = new SolidWorksConnector();
            _monitor   = new DocumentMonitor(_connector);

            _monitor.DocumentChanged += OnDocumentChanged;
            _monitor.SwDisconnected  += OnSwDisconnected;
            _monitor.SwReconnected   += OnSwReconnected;

            LogHelper.Initialize(richTextBoxLog);

            // Load persisted settings (both legacy Properties.Settings and new SettingsManager)
            var appSettings = SettingsManager.Load();

            // Restore saved output folder (prefer new settings, fall back to legacy)
            string savedFolder = !string.IsNullOrEmpty(appSettings.OutputFolder)
                ? appSettings.OutputFolder
                : Properties.Settings.Default.OutputFolder;
            if (!string.IsNullOrEmpty(savedFolder))
                txtOutputFolder.Text = savedFolder;

            string savedAuthor = !string.IsNullOrEmpty(appSettings.AuthorName)
                ? appSettings.AuthorName
                : Properties.Settings.Default.Author;
            if (!string.IsNullOrEmpty(savedAuthor))
                txtAuthor.Text = savedAuthor;

            // Restore template paths
            txtDrawingTemplate.Text     = appSettings.DrawingTemplatePath;
            txtFlatPatternTemplate.Text = appSettings.FlatPatternTemplatePath;
            txtAssemblyTemplate.Text    = appSettings.AssemblyDrawingTemplatePath;

            UpdateDocumentPanel(DocumentState.Empty);
            UpdateButtonStates();
        }

        // ════════════════════════════════════════════════════════════════════
        // Form events
        // ════════════════════════════════════════════════════════════════════

        private void MainForm_Load(object sender, EventArgs e)
        {
            _connector.TryConnect();
            _monitor.Start();
            LogHelper.Log("CreateChert And Razverka запущен. Ожидание SolidWorks...", LogLevel.Info);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _monitor.Stop();
            _monitor.Dispose();
            _connector.Dispose();

            // Persist settings (both legacy and new)
            Properties.Settings.Default.OutputFolder = txtOutputFolder.Text;
            Properties.Settings.Default.Author       = txtAuthor.Text;
            Properties.Settings.Default.Save();

            SaveTemplateSettings();
        }

        // ════════════════════════════════════════════════════════════════════
        // Document monitor callbacks (marshalled to UI thread by Timer)
        // ════════════════════════════════════════════════════════════════════

        private void OnDocumentChanged(object sender, DocumentState state)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnDocumentChanged(sender, state))); return; }

            _currentState = state;
            UpdateDocumentPanel(state);
            RefreshComponentList();
            UpdateButtonStates();

            string docName = string.IsNullOrEmpty(state.FileName) ? "" : " — " + state.FileName;
            Text = "CreateChert And Razverka" + docName;
        }

        private void OnSwDisconnected(object sender, EventArgs e)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnSwDisconnected(sender, e))); return; }

            _currentState = DocumentState.Empty;
            UpdateDocumentPanel(DocumentState.Empty);
            _components.Clear();
            RefreshGrid();
            UpdateButtonStates();
            lblStatus.Text      = "● Нет подключения";
            lblStatus.ForeColor = Color.Red;
            Text = "CreateChert And Razverka";
            LogHelper.Log("SolidWorks закрыт или недоступен.", LogLevel.Warning);
        }

        private void OnSwReconnected(object sender, EventArgs e)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnSwReconnected(sender, e))); return; }
            LogHelper.Log("SolidWorks подключён заново.", LogLevel.Success);
        }

        // ════════════════════════════════════════════════════════════════════
        // UI update helpers
        // ════════════════════════════════════════════════════════════════════

        private void UpdateDocumentPanel(DocumentState state)
        {
            if (state.Type == DocumentType.None)
            {
                lblStatus.Text      = "● Нет документа";
                lblStatus.ForeColor = Color.Red;
            }
            else
            {
                lblStatus.Text      = "● Подключено";
                lblStatus.ForeColor = Color.Green;
            }

            lblDocumentName.Text = string.IsNullOrEmpty(state.FileName)
                ? "(нет открытого документа)"
                : state.FileName;

            lblDocumentType.Text = state.TypeDisplay;
            lblDocumentPath.Text = string.IsNullOrEmpty(state.FilePath)
                ? ""
                : state.FilePath;
        }

        private void RefreshComponentList()
        {
            _components.Clear();

            if (_currentState.Type == DocumentType.None || _currentState.Type == DocumentType.Drawing)
            {
                RefreshGrid();
                return;
            }

            if (_currentState.Type == DocumentType.Part)
            {
                _components.Add(new ComponentInfo
                {
                    FilePath     = _currentState.FilePath,
                    FileName     = _currentState.FileName,
                    Type         = ComponentType.Part,
                    IsSheetMetal = _currentState.IsSheetMetal,
                    IsSelected   = true,
                    Status       = ComponentStatus.Pending,
                    StatusMessage = "Ожидание"
                });
            }
            else if (_currentState.Type == DocumentType.Assembly)
            {
                LoadAssemblyComponents();

                // Add [Assembly Drawing] row
                _components.Add(new ComponentInfo
                {
                    FilePath     = _currentState.FilePath,
                    FileName     = "[Сборочный чертёж]",
                    Type         = ComponentType.AssemblyDrawing,
                    IsSelected   = true,
                    Status       = ComponentStatus.Pending,
                    StatusMessage = "Ожидание"
                });
            }

            RefreshGrid();
        }

        private void LoadAssemblyComponents()
        {
            dynamic doc = _connector.GetActiveDocument();
            if (doc == null) return;

            try
            {
                object[] comps = doc.GetComponents(false) as object[];
                if (comps == null) return;

                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (dynamic comp in comps)
                {
                    if (comp == null) continue;
                    try
                    {
                        dynamic modelDoc = comp.GetModelDoc2();
                        if (modelDoc == null) continue;

                        string path = (string)modelDoc.GetPathName();
                        if (string.IsNullOrEmpty(path) || seen.Contains(path)) continue;
                        seen.Add(path);

                        // Assembly components are either parts (.sldprt) or sub-assemblies (.sldasm)
                        string compExt = System.IO.Path.GetExtension(path).ToLowerInvariant();
                        ComponentType ct = compExt == ".sldasm" ? ComponentType.Assembly : ComponentType.Part;
                        bool sm = ct == ComponentType.Part && SolidWorksConnector.IsSheetMetal(modelDoc);

                        _components.Add(new ComponentInfo
                        {
                            FilePath     = path,
                            FileName     = Path.GetFileName(path),
                            Type         = ct,
                            IsSheetMetal = sm,
                            IsSelected   = true,
                            Status       = ComponentStatus.Pending,
                            StatusMessage = "Ожидание"
                        });
                    }
                    catch { /* skip bad component */ }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("Предупреждение при загрузке компонентов: " + ex.Message, LogLevel.Warning);
            }
        }

        private void RefreshGrid()
        {
            dataGridComponents.SuspendLayout();
            dataGridComponents.Rows.Clear();

            string filter = comboFilter.SelectedItem?.ToString() ?? "Все";
            string search = txtSearch.Text.Trim().ToLowerInvariant();

            foreach (var c in _components)
            {
                if (!MatchesFilter(c, filter)) continue;
                if (!string.IsNullOrEmpty(search) &&
                    !c.FileName.ToLowerInvariant().Contains(search)) continue;

                int idx = dataGridComponents.Rows.Add();
                var row = dataGridComponents.Rows[idx];
                row.Cells["colSelected"].Value    = c.IsSelected;
                row.Cells["colIcon"].Value         = c.TypeIcon;
                row.Cells["colFileName"].Value     = c.FileName;
                row.Cells["colSheetMetal"].Value   = c.SheetMetalDisplay;
                row.Cells["colStatus"].Value       = c.StatusMessage;
                row.Tag = c;
            }

            dataGridComponents.ResumeLayout();
        }

        private bool MatchesFilter(ComponentInfo c, string filter)
        {
            switch (filter)
            {
                case "Только детали":    return c.Type == ComponentType.Part;
                case "Только листовые":  return c.Type == ComponentType.Part && c.IsSheetMetal;
                case "Только сборка":    return c.Type == ComponentType.Assembly
                                             || c.Type == ComponentType.AssemblyDrawing;
                default:                 return true;
            }
        }

        private void UpdateButtonStates()
        {
            bool connected  = _connector.IsConnected;
            bool hasPart    = _currentState.Type == DocumentType.Part;
            bool hasAssembly = _currentState.Type == DocumentType.Assembly;
            bool hasDoc     = hasPart || hasAssembly;
            bool busy       = _cts != null;

            btnCreateDrawings.Enabled    = connected && hasDoc && !busy;
            btnCreateFlatPatterns.Enabled = connected && !busy;
            btnCreateAll.Enabled         = connected && hasDoc && !busy;
            btnCancel.Enabled            = busy;

            // Flat pattern button only makes sense when there are sheet-metal parts
            if (!busy)
            {
                bool hasSheetMetal = _components.Exists(c =>
                    c.IsSelected && c.Type == ComponentType.Part && c.IsSheetMetal);
                btnCreateFlatPatterns.Enabled = connected && hasSheetMetal;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // Control event handlers
        // ════════════════════════════════════════════════════════════════════

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!_connector.IsConnected)
                _connector.TryConnect();

            dynamic doc = _connector.GetActiveDocument();
            DocumentType type = SolidWorksConnector.GetDocumentType(doc);
            string path = null;
            if (doc != null)
            {
                try { path = (string)doc.GetPathName(); }
                catch { }
            }

            var state = new DocumentState
            {
                Type         = type,
                FilePath     = path ?? "",
                FileName     = string.IsNullOrEmpty(path) ? "" : Path.GetFileName(path),
                IsSheetMetal = (type == DocumentType.Part) && SolidWorksConnector.IsSheetMetal(doc)
            };
            OnDocumentChanged(this, state);
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            string selected = FileHelper.SelectFolder(
                "Выберите папку для сохранения чертежей", txtOutputFolder.Text);
            if (selected != null)
                txtOutputFolder.Text = selected;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (var c in _components) c.IsSelected = true;
            RefreshGrid();
            UpdateButtonStates();
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            foreach (var c in _components) c.IsSelected = false;
            RefreshGrid();
            UpdateButtonStates();
        }

        private void comboFilter_SelectedIndexChanged(object sender, EventArgs e) => RefreshGrid();

        private void txtSearch_TextChanged(object sender, EventArgs e) => RefreshGrid();

        private void dataGridComponents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridComponents.Rows[e.RowIndex];
            if (row.Tag is ComponentInfo ci && e.ColumnIndex == dataGridComponents.Columns["colSelected"].Index)
            {
                ci.IsSelected = (bool)(row.Cells["colSelected"].Value ?? false);
                UpdateButtonStates();
            }
        }

        private void dataGridComponents_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridComponents.IsCurrentCellDirty)
                dataGridComponents.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void btnClearLog_Click(object sender, EventArgs e) => LogHelper.Clear();

        private void btnBrowseDrawingTemplate_Click(object sender, EventArgs e)
        {
            string path = BrowseTemplateFile(txtDrawingTemplate.Text);
            if (path != null) txtDrawingTemplate.Text = path;
        }

        private void btnBrowseFlatPatternTemplate_Click(object sender, EventArgs e)
        {
            string path = BrowseTemplateFile(txtFlatPatternTemplate.Text);
            if (path != null) txtFlatPatternTemplate.Text = path;
        }

        private void btnBrowseAssemblyTemplate_Click(object sender, EventArgs e)
        {
            string path = BrowseTemplateFile(txtAssemblyTemplate.Text);
            if (path != null) txtAssemblyTemplate.Text = path;
        }

        private void btnSaveTemplates_Click(object sender, EventArgs e)
        {
            SaveTemplateSettings();
            LogHelper.Log("Шаблоны сохранены.", LogLevel.Success);
        }

        private void btnResetTemplates_Click(object sender, EventArgs e)
        {
            string defaultDir = SettingsManager.DefaultTemplatesPath;
            string defaultDrawTemplate = "";
            if (Directory.Exists(defaultDir))
            {
                var drwdots = Directory.GetFiles(defaultDir, "*.drwdot");
                Array.Sort(drwdots);
                if (drwdots.Length > 0)
                    defaultDrawTemplate = drwdots[0];
            }
            txtDrawingTemplate.Text     = defaultDrawTemplate;
            txtFlatPatternTemplate.Text = defaultDrawTemplate;
            txtAssemblyTemplate.Text    = defaultDrawTemplate;
            SaveTemplateSettings();
            LogHelper.Log($"Шаблоны сброшены. Папка шаблонов: {defaultDir}", LogLevel.Info);
        }

        private string BrowseTemplateFile(string currentPath)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title  = "Выберите шаблон чертежа";
                dlg.Filter = "Шаблоны чертежей (*.drwdot)|*.drwdot"
                           + "|Форматы листов (*.slddrt)|*.slddrt"
                           + "|Все файлы (*.*)|*.*";
                if (!string.IsNullOrEmpty(currentPath) && File.Exists(currentPath))
                    dlg.InitialDirectory = Path.GetDirectoryName(currentPath);
                else if (Directory.Exists(SettingsManager.DefaultTemplatesPath))
                    dlg.InitialDirectory = SettingsManager.DefaultTemplatesPath;
                return dlg.ShowDialog(this) == DialogResult.OK ? dlg.FileName : null;
            }
        }

        private void SaveTemplateSettings()
        {
            var settings = SettingsManager.Load();
            settings.DrawingTemplatePath         = txtDrawingTemplate.Text.Trim();
            settings.FlatPatternTemplatePath     = txtFlatPatternTemplate.Text.Trim();
            settings.AssemblyDrawingTemplatePath = txtAssemblyTemplate.Text.Trim();
            settings.AuthorName                  = txtAuthor.Text.Trim();
            settings.OutputFolder                = txtOutputFolder.Text.Trim();
            SettingsManager.Save(settings);
        }

        // ════════════════════════════════════════════════════════════════════
        // Generation actions
        // ════════════════════════════════════════════════════════════════════

        private async void btnCreateDrawings_Click(object sender, EventArgs e)
        {
            await RunOperation(GenerateDrawingsAsync);
        }

        private async void btnCreateFlatPatterns_Click(object sender, EventArgs e)
        {
            await RunOperation(GenerateFlatPatternsAsync);
        }

        private async void btnCreateAll_Click(object sender, EventArgs e)
        {
            await RunOperation(GenerateAllAsync);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            LogHelper.Log("Операция отменена пользователем.", LogLevel.Warning);
        }

        private async Task RunOperation(Func<DrawingSettings, CancellationToken, Task> operation)
        {
            if (!ValidateInputs()) return;

            var settings = BuildSettings();
            FileHelper.EnsureDirectory(settings.OutputFolder);

            EnsureCreators();

            _cts = new CancellationTokenSource();
            UpdateButtonStates();
            progressBar.Value = 0;

            try
            {
                await operation(settings, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                LogHelper.Log("Операция отменена.", LogLevel.Warning);
            }
            catch (Exception ex)
            {
                LogHelper.Log("Неожиданная ошибка: " + ex.Message, LogLevel.Error);
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
                progressBar.Value = 0;
                lblProgress.Text  = "";
                UpdateButtonStates();
            }
        }

        private void EnsureCreators()
        {
            if (_drawingCreator == null) _drawingCreator = new DrawingCreator(_connector);
            if (_flatCreator    == null) _flatCreator    = new FlatPatternCreator(_connector);
            if (_asmCreator     == null) _asmCreator     = new AssemblyDrawingCreator(_connector);
        }

        private async Task GenerateDrawingsAsync(DrawingSettings settings, CancellationToken ct)
        {
            var selected = _components.FindAll(c =>
                c.IsSelected && c.Type == ComponentType.Part);

            await ProcessListAsync(selected, settings, ct, (c, s) =>
                _drawingCreator.CreateDrawing(c.FilePath, s));
        }

        private async Task GenerateFlatPatternsAsync(DrawingSettings settings, CancellationToken ct)
        {
            var selected = _components.FindAll(c =>
                c.IsSelected && c.Type == ComponentType.Part && c.IsSheetMetal);

            await ProcessListAsync(selected, settings, ct, (c, s) =>
                _flatCreator.CreateFlatPattern(c.FilePath, s));
        }

        private async Task GenerateAssemblyDrawingAsync(DrawingSettings settings, CancellationToken ct)
        {
            var selected = _components.FindAll(c =>
                c.IsSelected && c.Type == ComponentType.AssemblyDrawing);

            await ProcessListAsync(selected, settings, ct, (c, s) =>
                _asmCreator.CreateAssemblyDrawing(c.FilePath, s));
        }

        private async Task GenerateAllAsync(DrawingSettings settings, CancellationToken ct)
        {
            if (chkCreateDrawings.Checked)
                await GenerateDrawingsAsync(settings, ct);

            if (!ct.IsCancellationRequested && chkCreateFlatPatterns.Checked)
                await GenerateFlatPatternsAsync(settings, ct);

            if (!ct.IsCancellationRequested && _currentState.Type == DocumentType.Assembly)
                await GenerateAssemblyDrawingAsync(settings, ct);
        }

        private async Task ProcessListAsync(
            List<ComponentInfo>              list,
            DrawingSettings                  settings,
            CancellationToken                ct,
            Func<ComponentInfo, DrawingSettings, SingleResult> action)
        {
            if (list.Count == 0)
            {
                LogHelper.Log("Нет компонентов для обработки.", LogLevel.Warning);
                return;
            }

            var result = new GenerationResult();
            int total  = list.Count;

            for (int i = 0; i < total; i++)
            {
                ct.ThrowIfCancellationRequested();

                var comp = list[i];
                comp.Status        = ComponentStatus.Processing;
                comp.StatusMessage = "Обработка";
                UpdateRowStatus(comp);

                UpdateProgress(i + 1, total);

                SingleResult sr = await Task.Run(() => action(comp, settings), ct);
                result.Add(sr);

                comp.Status        = sr.Status == ResultStatus.Success ? ComponentStatus.Done
                                   : sr.Status == ResultStatus.Skipped ? ComponentStatus.Skipped
                                   : ComponentStatus.Error;
                comp.StatusMessage = sr.Status == ResultStatus.Success ? "Готово"
                                   : sr.Status == ResultStatus.Skipped ? "Пропущено"
                                   : "Ошибка";
                UpdateRowStatus(comp);
            }

            LogHelper.Log("Итог: " + result.Summary, LogLevel.Success);
        }

        private void UpdateProgress(int current, int total)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(current, total)));
                return;
            }
            int pct = total > 0 ? (int)((double)current / total * 100) : 0;
            progressBar.Value = Math.Min(pct, 100);
            lblProgress.Text  = $"Деталь {current} из {total}";
        }

        private void UpdateRowStatus(ComponentInfo comp)
        {
            if (InvokeRequired) { Invoke(new Action(() => UpdateRowStatus(comp))); return; }
            foreach (DataGridViewRow row in dataGridComponents.Rows)
            {
                if (row.Tag == comp)
                {
                    row.Cells["colStatus"].Value = comp.StatusMessage;
                    break;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // Validation / settings build
        // ════════════════════════════════════════════════════════════════════

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtOutputFolder.Text))
            {
                MessageBox.Show(
                    "Укажите папку для сохранения.",
                    "CreateChert And Razverka",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            if (!_connector.IsConnected)
            {
                MessageBox.Show(
                    "SolidWorks не подключён. Запустите SolidWorks и откройте документ.",
                    "CreateChert And Razverka",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private DrawingSettings BuildSettings()
        {
            return new DrawingSettings
            {
                OutputFolder              = txtOutputFolder.Text.Trim(),
                Author                    = txtAuthor.Text.Trim(),
                SheetFormat               = GetSelectedSheetFormat(),
                CreateDrawings            = chkCreateDrawings.Checked,
                CreateFlatPatterns        = chkCreateFlatPatterns.Checked,
                ExportToPdf               = chkExportPdf.Checked,
                AutoDimensions            = chkAutoDimensions.Checked,
                DrawingTemplatePath         = txtDrawingTemplate.Text.Trim(),
                FlatPatternTemplatePath     = txtFlatPatternTemplate.Text.Trim(),
                AssemblyDrawingTemplatePath = txtAssemblyTemplate.Text.Trim()
            };
        }

        private SheetFormat GetSelectedSheetFormat()
        {
            if (rbA3.Checked) return SheetFormat.A3;
            if (rbA2.Checked) return SheetFormat.A2;
            if (rbA1.Checked) return SheetFormat.A1;
            if (rbA0.Checked) return SheetFormat.A0;
            return SheetFormat.A4;
        }
    }
}
