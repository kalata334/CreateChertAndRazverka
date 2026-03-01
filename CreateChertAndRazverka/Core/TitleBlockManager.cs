using System;
using CreateChertAndRazverka.Helpers;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Fills the SolidWorks drawing title block (штамп) with author, document name, and date.
    /// Uses the CustomPropertyManager to write to the sheet-format properties
    /// that are linked to the title block notes.
    /// </summary>
    public class TitleBlockManager
    {
        private readonly SolidWorksConnector _connector;

        public TitleBlockManager(SolidWorksConnector connector)
        {
            _connector = connector ?? throw new ArgumentNullException("connector");
        }

        /// <summary>
        /// Fills standard title-block fields in the drawing document.
        /// </summary>
        /// <param name="drawingDoc">The open drawing IModelDoc2.</param>
        /// <param name="author">Author (ФИО) to insert.</param>
        /// <param name="documentName">Document name / designation.</param>
        public void FillTitleBlock(dynamic drawingDoc, string author, string documentName)
        {
            if (drawingDoc == null) return;

            try
            {
                // Custom properties at document level
                dynamic custProps = drawingDoc.Extension.CustomPropertyManager[""];

                SetProperty(custProps, "Разработал",       author);
                SetProperty(custProps, "Проверил",         "");
                SetProperty(custProps, "Обозначение",      documentName);
                SetProperty(custProps, "Наименование",     documentName);
                SetProperty(custProps, "Дата",             DateTime.Today.ToString("dd.MM.yyyy"));
                SetProperty(custProps, "Масса",            "");
                SetProperty(custProps, "Масштаб",          "1:1");
                SetProperty(custProps, "Лист",             "1");
                SetProperty(custProps, "Листов",           "1");

                // Also try the Sheet Format (SW 2022 stores title-block vars here)
                dynamic sheet = drawingDoc.GetCurrentSheet();
                if (sheet != null)
                {
                    try
                    {
                        dynamic sheetProps = sheet.CustomPropertyManager;
                        if (sheetProps != null)
                        {
                            SetProperty(sheetProps, "Разработал",   author);
                            SetProperty(sheetProps, "Обозначение",  documentName);
                            SetProperty(sheetProps, "Наименование", documentName);
                            SetProperty(sheetProps, "Дата",         DateTime.Today.ToString("dd.MM.yyyy"));
                        }
                    }
                    catch { /* sheet properties may not exist in all templates */ }
                }

                drawingDoc.GraphicsRedraw2();

                // Also write author/name directly into sheet-format note text boxes
                try
                {
                    drawingDoc.EditSheetFormat();
                    object[] notes = null;
                    try { notes = (object[])drawingDoc.GetNotes(); } catch { }
                    if (notes != null)
                    {
                        foreach (object noteObj in notes)
                        {
                            if (noteObj == null) continue;
                            try
                            {
                                dynamic note = noteObj;
                                string text = null;
                                try { text = (string)note.GetText(); } catch { }
                                if (string.IsNullOrEmpty(text)) continue;
                                string upper = text.ToUpperInvariant();
                                if (upper.Contains("РАЗРАБ") || upper.Contains("DRAWN"))
                                    note.SetText(author);
                                else if (upper.Contains("НАИМЕНОВ") || upper.Contains("TITLE"))
                                    note.SetText(documentName);
                                // Both name ("Наименование") and designation ("Обозначение") receive
                                // the same documentName value so that both stamp cells are populated.
                                else if (upper.Contains("ОБОЗНАЧ") || upper.Contains("DESIGNATION"))
                                    note.SetText(documentName);
                            }
                            catch { }
                        }
                    }
                    drawingDoc.EditSheet();
                }
                catch { /* older template may not support sheet-format note editing */ }

                LogHelper.Log("Штамп заполнен.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                LogHelper.Log("Предупреждение при заполнении штампа: " + ex.Message, LogLevel.Warning);
            }
        }

        private static void SetProperty(dynamic custProps, string name, string value)
        {
            if (custProps == null) return;
            try
            {
                // swCustomInfoType_e.swCustomInfoText = 30
                // Add2: (FieldName, FieldType, FieldValue)
                // Returns swCustomInfoAddResult_e; 0=already exists, we use Set instead
                int result = custProps.Add3(name, 30, value, 1);
                if (result != 0)
                    custProps.Set2(name, value);
            }
            catch
            {
                try { custProps.Set2(name, value); }
                catch { /* property may be read-only in this template */ }
            }
        }
    }
}
