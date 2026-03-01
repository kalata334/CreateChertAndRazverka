namespace CreateChertAndRazverka
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Controls ─────────────────────────────────────────────────────
            this.grpDocument         = new System.Windows.Forms.GroupBox();
            this.btnRefresh          = new System.Windows.Forms.Button();
            this.lblStatus           = new System.Windows.Forms.Label();
            this.lblDocumentName     = new System.Windows.Forms.Label();
            this.lblDocumentType     = new System.Windows.Forms.Label();
            this.lblDocumentPath     = new System.Windows.Forms.Label();
            this.lblNameCaption      = new System.Windows.Forms.Label();
            this.lblTypeCaption      = new System.Windows.Forms.Label();
            this.lblPathCaption      = new System.Windows.Forms.Label();

            this.grpAuthor           = new System.Windows.Forms.GroupBox();
            this.lblAuthorCaption    = new System.Windows.Forms.Label();
            this.txtAuthor           = new System.Windows.Forms.TextBox();

            this.grpFolder           = new System.Windows.Forms.GroupBox();
            this.lblFolderCaption    = new System.Windows.Forms.Label();
            this.txtOutputFolder     = new System.Windows.Forms.TextBox();
            this.btnBrowseFolder     = new System.Windows.Forms.Button();

            this.grpFormat           = new System.Windows.Forms.GroupBox();
            this.rbA4                = new System.Windows.Forms.RadioButton();
            this.rbA3                = new System.Windows.Forms.RadioButton();
            this.rbA2                = new System.Windows.Forms.RadioButton();
            this.rbA1                = new System.Windows.Forms.RadioButton();
            this.rbA0                = new System.Windows.Forms.RadioButton();

            this.grpTemplates                 = new System.Windows.Forms.GroupBox();
            this.lblDrawingTemplateCap        = new System.Windows.Forms.Label();
            this.txtDrawingTemplate           = new System.Windows.Forms.TextBox();
            this.btnBrowseDrawingTemplate     = new System.Windows.Forms.Button();
            this.btnEditDrawingTemplate       = new System.Windows.Forms.Button();
            this.lblFlatPatternTemplateCap    = new System.Windows.Forms.Label();
            this.txtFlatPatternTemplate       = new System.Windows.Forms.TextBox();
            this.btnBrowseFlatPatternTemplate = new System.Windows.Forms.Button();
            this.btnEditFlatPatternTemplate   = new System.Windows.Forms.Button();
            this.lblAssemblyTemplateCap       = new System.Windows.Forms.Label();
            this.txtAssemblyTemplate          = new System.Windows.Forms.TextBox();
            this.btnBrowseAssemblyTemplate    = new System.Windows.Forms.Button();
            this.btnEditAssemblyTemplate      = new System.Windows.Forms.Button();
            this.btnSaveTemplates             = new System.Windows.Forms.Button();
            this.btnResetTemplates            = new System.Windows.Forms.Button();

            this.grpComponents       = new System.Windows.Forms.GroupBox();
            this.pnlComponentsTop    = new System.Windows.Forms.Panel();
            this.btnSelectAll        = new System.Windows.Forms.Button();
            this.btnDeselectAll      = new System.Windows.Forms.Button();
            this.comboFilter         = new System.Windows.Forms.ComboBox();
            this.txtSearch           = new System.Windows.Forms.TextBox();
            this.dataGridComponents  = new System.Windows.Forms.DataGridView();
            this.colSelected         = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colIcon             = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileName         = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSheetMetal       = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus           = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.grpOptions          = new System.Windows.Forms.GroupBox();
            this.chkCreateDrawings   = new System.Windows.Forms.CheckBox();
            this.chkCreateFlatPatterns = new System.Windows.Forms.CheckBox();
            this.chkExportPdf        = new System.Windows.Forms.CheckBox();
            this.chkAutoDimensions   = new System.Windows.Forms.CheckBox();

            this.pnlActions          = new System.Windows.Forms.Panel();
            this.btnCreateDrawings   = new System.Windows.Forms.Button();
            this.btnCreateFlatPatterns = new System.Windows.Forms.Button();
            this.btnCreateAll        = new System.Windows.Forms.Button();
            this.btnCancel           = new System.Windows.Forms.Button();

            this.progressBar         = new System.Windows.Forms.ProgressBar();
            this.lblProgress         = new System.Windows.Forms.Label();

            this.grpLog              = new System.Windows.Forms.GroupBox();
            this.pnlLogTop           = new System.Windows.Forms.Panel();
            this.lblLogCaption       = new System.Windows.Forms.Label();
            this.btnClearLog         = new System.Windows.Forms.Button();
            this.richTextBoxLog      = new System.Windows.Forms.RichTextBox();

            this.statusStrip         = new System.Windows.Forms.StatusStrip();
            this.tsslMain            = new System.Windows.Forms.ToolStripStatusLabel();

            // ================================================================
            // grpDocument
            // ================================================================
            this.grpDocument.SuspendLayout();
            this.grpDocument.Text     = "Текущий документ";
            this.grpDocument.Location = new System.Drawing.Point(8, 8);
            this.grpDocument.Size     = new System.Drawing.Size(784, 110);
            this.grpDocument.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                      | System.Windows.Forms.AnchorStyles.Left
                                      | System.Windows.Forms.AnchorStyles.Right;

            this.lblStatus.Text      = "● Нет подключения";
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location  = new System.Drawing.Point(8, 22);
            this.lblStatus.Size      = new System.Drawing.Size(250, 20);
            this.lblStatus.AutoSize  = false;

            this.lblNameCaption.Text     = "Документ:";
            this.lblNameCaption.Location = new System.Drawing.Point(8, 48);
            this.lblNameCaption.Size     = new System.Drawing.Size(80, 20);
            this.lblNameCaption.AutoSize = false;
            this.lblNameCaption.Font     = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.lblDocumentName.Text     = "(нет открытого документа)";
            this.lblDocumentName.Location = new System.Drawing.Point(95, 48);
            this.lblDocumentName.Size     = new System.Drawing.Size(450, 20);
            this.lblDocumentName.AutoSize = false;

            this.lblTypeCaption.Text     = "Тип:";
            this.lblTypeCaption.Location = new System.Drawing.Point(8, 70);
            this.lblTypeCaption.Size     = new System.Drawing.Size(80, 20);
            this.lblTypeCaption.AutoSize = false;
            this.lblTypeCaption.Font     = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.lblDocumentType.Text     = "🔴 Нет документа";
            this.lblDocumentType.Location = new System.Drawing.Point(95, 70);
            this.lblDocumentType.Size     = new System.Drawing.Size(300, 20);
            this.lblDocumentType.AutoSize = false;

            this.lblPathCaption.Text     = "Путь:";
            this.lblPathCaption.Location = new System.Drawing.Point(8, 88);
            this.lblPathCaption.Size     = new System.Drawing.Size(80, 20);
            this.lblPathCaption.AutoSize = false;
            this.lblPathCaption.Font     = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.lblDocumentPath.Text     = "";
            this.lblDocumentPath.Location = new System.Drawing.Point(95, 88);
            this.lblDocumentPath.Size     = new System.Drawing.Size(570, 18);
            this.lblDocumentPath.AutoSize = false;
            this.lblDocumentPath.Font     = new System.Drawing.Font("Segoe UI", 8);

            this.btnRefresh.Text     = "🔄 Обновить";
            this.btnRefresh.Location = new System.Drawing.Point(680, 18);
            this.btnRefresh.Size     = new System.Drawing.Size(95, 28);
            this.btnRefresh.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnRefresh.Click   += new System.EventHandler(this.btnRefresh_Click);

            this.grpDocument.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblStatus, this.lblNameCaption, this.lblDocumentName,
                this.lblTypeCaption, this.lblDocumentType,
                this.lblPathCaption, this.lblDocumentPath,
                this.btnRefresh
            });
            this.grpDocument.ResumeLayout(false);

            // ================================================================
            // grpAuthor
            // ================================================================
            this.grpAuthor.SuspendLayout();
            this.grpAuthor.Text     = "Автор";
            this.grpAuthor.Location = new System.Drawing.Point(8, 124);
            this.grpAuthor.Size     = new System.Drawing.Size(784, 52);
            this.grpAuthor.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;

            this.lblAuthorCaption.Text     = "Автор (ФИО):";
            this.lblAuthorCaption.Location = new System.Drawing.Point(8, 22);
            this.lblAuthorCaption.Size     = new System.Drawing.Size(90, 22);
            this.lblAuthorCaption.AutoSize = false;
            this.lblAuthorCaption.Font     = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.txtAuthor.Location = new System.Drawing.Point(105, 20);
            this.txtAuthor.Size     = new System.Drawing.Size(300, 23);

            this.grpAuthor.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblAuthorCaption, this.txtAuthor
            });
            this.grpAuthor.ResumeLayout(false);

            // ================================================================
            // grpFolder
            // ================================================================
            this.grpFolder.SuspendLayout();
            this.grpFolder.Text     = "Папка сохранения";
            this.grpFolder.Location = new System.Drawing.Point(8, 182);
            this.grpFolder.Size     = new System.Drawing.Size(784, 52);
            this.grpFolder.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;

            this.lblFolderCaption.Text     = "Папка:";
            this.lblFolderCaption.Location = new System.Drawing.Point(8, 22);
            this.lblFolderCaption.Size     = new System.Drawing.Size(50, 22);
            this.lblFolderCaption.AutoSize = false;
            this.lblFolderCaption.Font     = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.txtOutputFolder.Location = new System.Drawing.Point(62, 20);
            this.txtOutputFolder.Size     = new System.Drawing.Size(600, 23);
            this.txtOutputFolder.Anchor   = System.Windows.Forms.AnchorStyles.Left
                                          | System.Windows.Forms.AnchorStyles.Right
                                          | System.Windows.Forms.AnchorStyles.Top;

            this.btnBrowseFolder.Text     = "📁 Обзор...";
            this.btnBrowseFolder.Location = new System.Drawing.Point(668, 18);
            this.btnBrowseFolder.Size     = new System.Drawing.Size(105, 27);
            this.btnBrowseFolder.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnBrowseFolder.Click   += new System.EventHandler(this.btnBrowseFolder_Click);

            this.grpFolder.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblFolderCaption, this.txtOutputFolder, this.btnBrowseFolder
            });
            this.grpFolder.ResumeLayout(false);

            // ================================================================
            // grpFormat
            // ================================================================
            this.grpFormat.SuspendLayout();
            this.grpFormat.Text     = "Формат листа";
            this.grpFormat.Location = new System.Drawing.Point(8, 240);
            this.grpFormat.Size     = new System.Drawing.Size(784, 44);
            this.grpFormat.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;

            this.rbA4.Text     = "A4";
            this.rbA4.Checked  = true;
            this.rbA4.Location = new System.Drawing.Point(8, 18);
            this.rbA4.Size     = new System.Drawing.Size(55, 20);

            this.rbA3.Text     = "A3";
            this.rbA3.Location = new System.Drawing.Point(68, 18);
            this.rbA3.Size     = new System.Drawing.Size(55, 20);

            this.rbA2.Text     = "A2";
            this.rbA2.Location = new System.Drawing.Point(128, 18);
            this.rbA2.Size     = new System.Drawing.Size(55, 20);

            this.rbA1.Text     = "A1";
            this.rbA1.Location = new System.Drawing.Point(188, 18);
            this.rbA1.Size     = new System.Drawing.Size(55, 20);

            this.rbA0.Text     = "A0";
            this.rbA0.Location = new System.Drawing.Point(248, 18);
            this.rbA0.Size     = new System.Drawing.Size(55, 20);

            this.grpFormat.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.rbA4, this.rbA3, this.rbA2, this.rbA1, this.rbA0
            });
            this.grpFormat.ResumeLayout(false);

            // ================================================================
            // grpTemplates — Drawing template selection
            // ================================================================
            this.grpTemplates.SuspendLayout();
            this.grpTemplates.Text     = "Шаблоны чертежей";
            this.grpTemplates.Location = new System.Drawing.Point(8, 290);
            this.grpTemplates.Size     = new System.Drawing.Size(784, 140);
            this.grpTemplates.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                       | System.Windows.Forms.AnchorStyles.Left
                                       | System.Windows.Forms.AnchorStyles.Right;

            // Row 1 — Drawing template
            this.lblDrawingTemplateCap.Text      = "Шаблон деталей:";
            this.lblDrawingTemplateCap.Location  = new System.Drawing.Point(8, 24);
            this.lblDrawingTemplateCap.Size      = new System.Drawing.Size(155, 22);
            this.lblDrawingTemplateCap.AutoSize  = false;
            this.lblDrawingTemplateCap.Font      = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.txtDrawingTemplate.Location    = new System.Drawing.Point(168, 22);
            this.txtDrawingTemplate.Size        = new System.Drawing.Size(370, 23);
            this.txtDrawingTemplate.ReadOnly    = true;
            this.txtDrawingTemplate.Anchor      = System.Windows.Forms.AnchorStyles.Left
                                                | System.Windows.Forms.AnchorStyles.Right
                                                | System.Windows.Forms.AnchorStyles.Top;

            this.btnBrowseDrawingTemplate.Text     = "📁";
            this.btnBrowseDrawingTemplate.Location = new System.Drawing.Point(544, 21);
            this.btnBrowseDrawingTemplate.Size     = new System.Drawing.Size(32, 25);
            this.btnBrowseDrawingTemplate.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnBrowseDrawingTemplate.Click   += new System.EventHandler(this.btnBrowseDrawingTemplate_Click);

            this.btnEditDrawingTemplate.Text     = "✏️ Изменить";
            this.btnEditDrawingTemplate.Location = new System.Drawing.Point(582, 21);
            this.btnEditDrawingTemplate.Size     = new System.Drawing.Size(95, 25);
            this.btnEditDrawingTemplate.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            // "Изменить" intentionally opens the same file dialog as the browse button
            this.btnEditDrawingTemplate.Click   += new System.EventHandler(this.btnBrowseDrawingTemplate_Click);

            // Row 2 — Flat pattern template
            this.lblFlatPatternTemplateCap.Text      = "Шаблон развёрток:";
            this.lblFlatPatternTemplateCap.Location  = new System.Drawing.Point(8, 52);
            this.lblFlatPatternTemplateCap.Size      = new System.Drawing.Size(155, 22);
            this.lblFlatPatternTemplateCap.AutoSize  = false;
            this.lblFlatPatternTemplateCap.Font      = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.txtFlatPatternTemplate.Location    = new System.Drawing.Point(168, 50);
            this.txtFlatPatternTemplate.Size        = new System.Drawing.Size(370, 23);
            this.txtFlatPatternTemplate.ReadOnly    = true;
            this.txtFlatPatternTemplate.Anchor      = System.Windows.Forms.AnchorStyles.Left
                                                    | System.Windows.Forms.AnchorStyles.Right
                                                    | System.Windows.Forms.AnchorStyles.Top;

            this.btnBrowseFlatPatternTemplate.Text     = "📁";
            this.btnBrowseFlatPatternTemplate.Location = new System.Drawing.Point(544, 49);
            this.btnBrowseFlatPatternTemplate.Size     = new System.Drawing.Size(32, 25);
            this.btnBrowseFlatPatternTemplate.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnBrowseFlatPatternTemplate.Click   += new System.EventHandler(this.btnBrowseFlatPatternTemplate_Click);

            this.btnEditFlatPatternTemplate.Text     = "✏️ Изменить";
            this.btnEditFlatPatternTemplate.Location = new System.Drawing.Point(582, 49);
            this.btnEditFlatPatternTemplate.Size     = new System.Drawing.Size(95, 25);
            this.btnEditFlatPatternTemplate.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            // "Изменить" intentionally opens the same file dialog as the browse button
            this.btnEditFlatPatternTemplate.Click   += new System.EventHandler(this.btnBrowseFlatPatternTemplate_Click);

            // Row 3 — Assembly drawing template
            this.lblAssemblyTemplateCap.Text      = "Шаблон сборок:";
            this.lblAssemblyTemplateCap.Location  = new System.Drawing.Point(8, 80);
            this.lblAssemblyTemplateCap.Size      = new System.Drawing.Size(155, 22);
            this.lblAssemblyTemplateCap.AutoSize  = false;
            this.lblAssemblyTemplateCap.Font      = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.txtAssemblyTemplate.Location    = new System.Drawing.Point(168, 78);
            this.txtAssemblyTemplate.Size        = new System.Drawing.Size(370, 23);
            this.txtAssemblyTemplate.ReadOnly    = true;
            this.txtAssemblyTemplate.Anchor      = System.Windows.Forms.AnchorStyles.Left
                                                 | System.Windows.Forms.AnchorStyles.Right
                                                 | System.Windows.Forms.AnchorStyles.Top;

            this.btnBrowseAssemblyTemplate.Text     = "📁";
            this.btnBrowseAssemblyTemplate.Location = new System.Drawing.Point(544, 77);
            this.btnBrowseAssemblyTemplate.Size     = new System.Drawing.Size(32, 25);
            this.btnBrowseAssemblyTemplate.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnBrowseAssemblyTemplate.Click   += new System.EventHandler(this.btnBrowseAssemblyTemplate_Click);

            this.btnEditAssemblyTemplate.Text     = "✏️ Изменить";
            this.btnEditAssemblyTemplate.Location = new System.Drawing.Point(582, 77);
            this.btnEditAssemblyTemplate.Size     = new System.Drawing.Size(95, 25);
            this.btnEditAssemblyTemplate.Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            // "Изменить" intentionally opens the same file dialog as the browse button
            this.btnEditAssemblyTemplate.Click   += new System.EventHandler(this.btnBrowseAssemblyTemplate_Click);

            // Row 4 — Save / Reset buttons
            this.btnSaveTemplates.Text     = "💾 Сохранить шаблоны";
            this.btnSaveTemplates.Location = new System.Drawing.Point(8, 108);
            this.btnSaveTemplates.Size     = new System.Drawing.Size(175, 26);
            this.btnSaveTemplates.Click   += new System.EventHandler(this.btnSaveTemplates_Click);

            this.btnResetTemplates.Text     = "🔄 Сбросить по умолчанию";
            this.btnResetTemplates.Location = new System.Drawing.Point(190, 108);
            this.btnResetTemplates.Size     = new System.Drawing.Size(195, 26);
            this.btnResetTemplates.Click   += new System.EventHandler(this.btnResetTemplates_Click);

            this.grpTemplates.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblDrawingTemplateCap,     this.txtDrawingTemplate,
                this.btnBrowseDrawingTemplate,  this.btnEditDrawingTemplate,
                this.lblFlatPatternTemplateCap, this.txtFlatPatternTemplate,
                this.btnBrowseFlatPatternTemplate, this.btnEditFlatPatternTemplate,
                this.lblAssemblyTemplateCap,    this.txtAssemblyTemplate,
                this.btnBrowseAssemblyTemplate, this.btnEditAssemblyTemplate,
                this.btnSaveTemplates,          this.btnResetTemplates
            });
            this.grpTemplates.ResumeLayout(false);

            // ================================================================
            // grpComponents — list
            // ================================================================
            this.grpComponents.SuspendLayout();
            this.grpComponents.Text     = "Список компонентов";
            this.grpComponents.Location = new System.Drawing.Point(8, 436);
            this.grpComponents.Size     = new System.Drawing.Size(784, 230);
            this.grpComponents.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                        | System.Windows.Forms.AnchorStyles.Left
                                        | System.Windows.Forms.AnchorStyles.Right
                                        | System.Windows.Forms.AnchorStyles.Bottom;

            // Top toolbar
            this.pnlComponentsTop.Location = new System.Drawing.Point(8, 20);
            this.pnlComponentsTop.Size     = new System.Drawing.Size(768, 30);
            this.pnlComponentsTop.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                           | System.Windows.Forms.AnchorStyles.Left
                                           | System.Windows.Forms.AnchorStyles.Right;

            this.btnSelectAll.Text     = "☑ Выбрать все";
            this.btnSelectAll.Location = new System.Drawing.Point(0, 2);
            this.btnSelectAll.Size     = new System.Drawing.Size(120, 26);
            this.btnSelectAll.Click   += new System.EventHandler(this.btnSelectAll_Click);

            this.btnDeselectAll.Text     = "☐ Снять все";
            this.btnDeselectAll.Location = new System.Drawing.Point(126, 2);
            this.btnDeselectAll.Size     = new System.Drawing.Size(110, 26);
            this.btnDeselectAll.Click   += new System.EventHandler(this.btnDeselectAll_Click);

            this.comboFilter.Items.AddRange(new object[] {
                "Все", "Только детали", "Только листовые", "Только сборка"
            });
            this.comboFilter.SelectedIndex             = 0;
            this.comboFilter.DropDownStyle             = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilter.Location                  = new System.Drawing.Point(244, 3);
            this.comboFilter.Size                      = new System.Drawing.Size(160, 24);
            this.comboFilter.SelectedIndexChanged     += new System.EventHandler(this.comboFilter_SelectedIndexChanged);

            this.txtSearch.Location        = new System.Drawing.Point(412, 3);
            this.txtSearch.Size            = new System.Drawing.Size(200, 23);
            this.txtSearch.TextChanged    += new System.EventHandler(this.txtSearch_TextChanged);

            this.pnlComponentsTop.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnSelectAll, this.btnDeselectAll, this.comboFilter, this.txtSearch
            });

            // DataGrid
            this.dataGridComponents.Location            = new System.Drawing.Point(8, 56);
            this.dataGridComponents.Size                = new System.Drawing.Size(768, 166);
            this.dataGridComponents.Anchor              = System.Windows.Forms.AnchorStyles.Top
                                                        | System.Windows.Forms.AnchorStyles.Left
                                                        | System.Windows.Forms.AnchorStyles.Right
                                                        | System.Windows.Forms.AnchorStyles.Bottom;
            this.dataGridComponents.AllowUserToAddRows  = false;
            this.dataGridComponents.AllowUserToDeleteRows = false;
            this.dataGridComponents.RowHeadersVisible   = false;
            this.dataGridComponents.SelectionMode       = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridComponents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridComponents.CellValueChanged   += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridComponents_CellValueChanged);
            this.dataGridComponents.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridComponents_CurrentCellDirtyStateChanged);

            this.colSelected.Name       = "colSelected";
            this.colSelected.HeaderText = "";
            this.colSelected.Width      = 30;
            this.colSelected.FillWeight = 15;

            this.colIcon.Name       = "colIcon";
            this.colIcon.HeaderText = "";
            this.colIcon.Width      = 28;
            this.colIcon.FillWeight = 12;
            this.colIcon.ReadOnly   = true;

            this.colFileName.Name       = "colFileName";
            this.colFileName.HeaderText = "Имя файла";
            this.colFileName.ReadOnly   = true;
            this.colFileName.FillWeight = 120;

            this.colSheetMetal.Name       = "colSheetMetal";
            this.colSheetMetal.HeaderText = "Листовой металл";
            this.colSheetMetal.ReadOnly   = true;
            this.colSheetMetal.FillWeight = 50;

            this.colStatus.Name       = "colStatus";
            this.colStatus.HeaderText = "Статус";
            this.colStatus.ReadOnly   = true;
            this.colStatus.FillWeight = 40;

            this.dataGridComponents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colSelected, this.colIcon, this.colFileName,
                this.colSheetMetal, this.colStatus
            });

            this.grpComponents.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.pnlComponentsTop, this.dataGridComponents
            });
            this.grpComponents.ResumeLayout(false);

            // ================================================================
            // grpOptions
            // ================================================================
            this.grpOptions.SuspendLayout();
            this.grpOptions.Text     = "Параметры";
            this.grpOptions.Location = new System.Drawing.Point(8, 676);
            this.grpOptions.Size     = new System.Drawing.Size(784, 54);
            this.grpOptions.Anchor   = System.Windows.Forms.AnchorStyles.Bottom
                                     | System.Windows.Forms.AnchorStyles.Left
                                     | System.Windows.Forms.AnchorStyles.Right;

            this.chkCreateDrawings.Text     = "☑ Создавать чертежи деталей";
            this.chkCreateDrawings.Checked  = true;
            this.chkCreateDrawings.Location = new System.Drawing.Point(8, 22);
            this.chkCreateDrawings.Size     = new System.Drawing.Size(200, 22);

            this.chkCreateFlatPatterns.Text     = "☑ Создавать развёртки";
            this.chkCreateFlatPatterns.Checked  = true;
            this.chkCreateFlatPatterns.Location = new System.Drawing.Point(215, 22);
            this.chkCreateFlatPatterns.Size     = new System.Drawing.Size(185, 22);

            this.chkExportPdf.Text     = "☑ Сохранять в PDF";
            this.chkExportPdf.Checked  = true;
            this.chkExportPdf.Location = new System.Drawing.Point(408, 22);
            this.chkExportPdf.Size     = new System.Drawing.Size(155, 22);

            this.chkAutoDimensions.Text     = "☑ Проставлять размеры";
            this.chkAutoDimensions.Checked  = true;
            this.chkAutoDimensions.Location = new System.Drawing.Point(570, 22);
            this.chkAutoDimensions.Size     = new System.Drawing.Size(180, 22);

            this.grpOptions.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.chkCreateDrawings, this.chkCreateFlatPatterns,
                this.chkExportPdf, this.chkAutoDimensions
            });
            this.grpOptions.ResumeLayout(false);

            // ================================================================
            // pnlActions
            // ================================================================
            this.pnlActions.Location = new System.Drawing.Point(8, 736);
            this.pnlActions.Size     = new System.Drawing.Size(784, 38);
            this.pnlActions.Anchor   = System.Windows.Forms.AnchorStyles.Bottom
                                     | System.Windows.Forms.AnchorStyles.Left
                                     | System.Windows.Forms.AnchorStyles.Right;

            this.btnCreateDrawings.Text     = "✏️ Создать чертежи";
            this.btnCreateDrawings.Location = new System.Drawing.Point(0, 4);
            this.btnCreateDrawings.Size     = new System.Drawing.Size(160, 30);
            this.btnCreateDrawings.Enabled  = false;
            this.btnCreateDrawings.Click   += new System.EventHandler(this.btnCreateDrawings_Click);

            this.btnCreateFlatPatterns.Text     = "📐 Создать развёртки";
            this.btnCreateFlatPatterns.Location = new System.Drawing.Point(166, 4);
            this.btnCreateFlatPatterns.Size     = new System.Drawing.Size(165, 30);
            this.btnCreateFlatPatterns.Enabled  = false;
            this.btnCreateFlatPatterns.Click   += new System.EventHandler(this.btnCreateFlatPatterns_Click);

            this.btnCreateAll.Text     = "📋 Создать всё";
            this.btnCreateAll.Location = new System.Drawing.Point(338, 4);
            this.btnCreateAll.Size     = new System.Drawing.Size(140, 30);
            this.btnCreateAll.Enabled  = false;
            this.btnCreateAll.Font     = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            this.btnCreateAll.Click   += new System.EventHandler(this.btnCreateAll_Click);

            this.btnCancel.Text     = "⛔ Отмена";
            this.btnCancel.Location = new System.Drawing.Point(485, 4);
            this.btnCancel.Size     = new System.Drawing.Size(110, 30);
            this.btnCancel.Enabled  = false;
            this.btnCancel.Click   += new System.EventHandler(this.btnCancel_Click);

            this.pnlActions.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.btnCreateDrawings, this.btnCreateFlatPatterns,
                this.btnCreateAll, this.btnCancel
            });

            // ================================================================
            // Progress
            // ================================================================
            this.progressBar.Location = new System.Drawing.Point(8, 780);
            this.progressBar.Size     = new System.Drawing.Size(680, 20);
            this.progressBar.Anchor   = System.Windows.Forms.AnchorStyles.Bottom
                                      | System.Windows.Forms.AnchorStyles.Left
                                      | System.Windows.Forms.AnchorStyles.Right;
            this.progressBar.Minimum  = 0;
            this.progressBar.Maximum  = 100;

            this.lblProgress.Text     = "";
            this.lblProgress.Location = new System.Drawing.Point(698, 782);
            this.lblProgress.Size     = new System.Drawing.Size(100, 18);
            this.lblProgress.Anchor   = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;

            // ================================================================
            // grpLog
            // ================================================================
            this.grpLog.SuspendLayout();
            this.grpLog.Text     = "Лог — CreateChert And Razverka:";
            this.grpLog.Location = new System.Drawing.Point(8, 806);
            this.grpLog.Size     = new System.Drawing.Size(784, 150);
            this.grpLog.Anchor   = System.Windows.Forms.AnchorStyles.Bottom
                                 | System.Windows.Forms.AnchorStyles.Left
                                 | System.Windows.Forms.AnchorStyles.Right;

            this.pnlLogTop.Location = new System.Drawing.Point(8, 20);
            this.pnlLogTop.Size     = new System.Drawing.Size(768, 28);
            this.pnlLogTop.Anchor   = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;

            this.lblLogCaption.Text     = "Лог операций";
            this.lblLogCaption.Location = new System.Drawing.Point(0, 6);
            this.lblLogCaption.Size     = new System.Drawing.Size(120, 20);
            this.lblLogCaption.AutoSize = false;
            this.lblLogCaption.Font     = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            this.btnClearLog.Text     = "Очистить";
            this.btnClearLog.Location = new System.Drawing.Point(128, 2);
            this.btnClearLog.Size     = new System.Drawing.Size(80, 24);
            this.btnClearLog.Click   += new System.EventHandler(this.btnClearLog_Click);

            this.pnlLogTop.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblLogCaption, this.btnClearLog
            });

            this.richTextBoxLog.Location  = new System.Drawing.Point(8, 52);
            this.richTextBoxLog.Size      = new System.Drawing.Size(768, 92);
            this.richTextBoxLog.Anchor    = System.Windows.Forms.AnchorStyles.Top
                                          | System.Windows.Forms.AnchorStyles.Bottom
                                          | System.Windows.Forms.AnchorStyles.Left
                                          | System.Windows.Forms.AnchorStyles.Right;
            this.richTextBoxLog.ReadOnly  = true;
            this.richTextBoxLog.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.richTextBoxLog.ForeColor = System.Drawing.Color.LightGray;
            this.richTextBoxLog.Font      = new System.Drawing.Font("Consolas", 9);
            this.richTextBoxLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;

            this.grpLog.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.pnlLogTop, this.richTextBoxLog
            });
            this.grpLog.ResumeLayout(false);

            // ================================================================
            // Status strip
            // ================================================================
            this.tsslMain.Text = "CreateChert And Razverka  v1.0.0";
            this.statusStrip.Items.Add(this.tsslMain);
            this.statusStrip.SizingGrip = false;

            // ================================================================
            // MainForm
            // ================================================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(800, 986);
            this.MinimumSize         = new System.Drawing.Size(820, 1006);
            this.Text                = "CreateChert And Razverka";
            this.Font                = new System.Drawing.Font("Segoe UI", 9);
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load               += new System.EventHandler(this.MainForm_Load);
            this.FormClosing        += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.grpDocument,
                this.grpAuthor,
                this.grpFolder,
                this.grpFormat,
                this.grpTemplates,
                this.grpComponents,
                this.grpOptions,
                this.pnlActions,
                this.progressBar,
                this.lblProgress,
                this.grpLog,
                this.statusStrip
            });
        }

        #endregion

        // ── Control declarations ──────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpDocument;
        private System.Windows.Forms.Label    lblStatus;
        private System.Windows.Forms.Label    lblNameCaption;
        private System.Windows.Forms.Label    lblDocumentName;
        private System.Windows.Forms.Label    lblTypeCaption;
        private System.Windows.Forms.Label    lblDocumentType;
        private System.Windows.Forms.Label    lblPathCaption;
        private System.Windows.Forms.Label    lblDocumentPath;
        private System.Windows.Forms.Button   btnRefresh;

        private System.Windows.Forms.GroupBox grpAuthor;
        private System.Windows.Forms.Label    lblAuthorCaption;
        private System.Windows.Forms.TextBox  txtAuthor;

        private System.Windows.Forms.GroupBox grpFolder;
        private System.Windows.Forms.Label    lblFolderCaption;
        private System.Windows.Forms.TextBox  txtOutputFolder;
        private System.Windows.Forms.Button   btnBrowseFolder;

        private System.Windows.Forms.GroupBox   grpFormat;
        private System.Windows.Forms.RadioButton rbA4;
        private System.Windows.Forms.RadioButton rbA3;
        private System.Windows.Forms.RadioButton rbA2;
        private System.Windows.Forms.RadioButton rbA1;
        private System.Windows.Forms.RadioButton rbA0;

        private System.Windows.Forms.GroupBox grpTemplates;
        private System.Windows.Forms.Label    lblDrawingTemplateCap;
        private System.Windows.Forms.TextBox  txtDrawingTemplate;
        private System.Windows.Forms.Button   btnBrowseDrawingTemplate;
        private System.Windows.Forms.Button   btnEditDrawingTemplate;
        private System.Windows.Forms.Label    lblFlatPatternTemplateCap;
        private System.Windows.Forms.TextBox  txtFlatPatternTemplate;
        private System.Windows.Forms.Button   btnBrowseFlatPatternTemplate;
        private System.Windows.Forms.Button   btnEditFlatPatternTemplate;
        private System.Windows.Forms.Label    lblAssemblyTemplateCap;
        private System.Windows.Forms.TextBox  txtAssemblyTemplate;
        private System.Windows.Forms.Button   btnBrowseAssemblyTemplate;
        private System.Windows.Forms.Button   btnEditAssemblyTemplate;
        private System.Windows.Forms.Button   btnSaveTemplates;
        private System.Windows.Forms.Button   btnResetTemplates;

        private System.Windows.Forms.GroupBox       grpComponents;
        private System.Windows.Forms.Panel          pnlComponentsTop;
        private System.Windows.Forms.Button         btnSelectAll;
        private System.Windows.Forms.Button         btnDeselectAll;
        private System.Windows.Forms.ComboBox       comboFilter;
        private System.Windows.Forms.TextBox        txtSearch;
        private System.Windows.Forms.DataGridView   dataGridComponents;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn  colIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn  colFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn  colSheetMetal;
        private System.Windows.Forms.DataGridViewTextBoxColumn  colStatus;

        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkCreateDrawings;
        private System.Windows.Forms.CheckBox chkCreateFlatPatterns;
        private System.Windows.Forms.CheckBox chkExportPdf;
        private System.Windows.Forms.CheckBox chkAutoDimensions;

        private System.Windows.Forms.Panel  pnlActions;
        private System.Windows.Forms.Button btnCreateDrawings;
        private System.Windows.Forms.Button btnCreateFlatPatterns;
        private System.Windows.Forms.Button btnCreateAll;
        private System.Windows.Forms.Button btnCancel;

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label       lblProgress;

        private System.Windows.Forms.GroupBox    grpLog;
        private System.Windows.Forms.Panel       pnlLogTop;
        private System.Windows.Forms.Label       lblLogCaption;
        private System.Windows.Forms.Button      btnClearLog;
        private System.Windows.Forms.RichTextBox richTextBoxLog;

        private System.Windows.Forms.StatusStrip        statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslMain;
    }
}
