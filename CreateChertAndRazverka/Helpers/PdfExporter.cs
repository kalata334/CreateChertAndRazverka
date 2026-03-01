using System;
using System.IO;

namespace CreateChertAndRazverka.Helpers
{
    /// <summary>
    /// Exports SolidWorks drawing documents to PDF using the SolidWorks API.
    /// </summary>
    public static class PdfExporter
    {
        // swExportPDFData options — numeric constants from swconst
        private const int swExportPDFData_e = 0;

        /// <summary>
        /// Exports an already-open SolidWorks drawing document to PDF.
        /// </summary>
        /// <param name="swApp">The running SolidWorks application (dynamic COM object).</param>
        /// <param name="drawingDoc">The drawing IModelDoc2 to export (dynamic COM object).</param>
        /// <param name="outputPath">Full path for the output .pdf file.</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool ExportToPdf(dynamic swApp, dynamic drawingDoc, string outputPath)
        {
            if (drawingDoc == null)
                throw new ArgumentNullException("drawingDoc");

            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("outputPath cannot be empty.", "outputPath");

            try
            {
                FileHelper.EnsureDirectory(Path.GetDirectoryName(outputPath));

                // Use SolidWorks ExportToPDF2 (available in SW 2014+)
                // Signature: ExportToPDF2(FileName, SheetNames, ExportAllSheets)
                // Returns true on success
                int errors   = 0;
                int warnings = 0;

                // SaveAs with swSaveAsCurrentVersion and PDF format
                // swSaveAsType_e.swSaveAsPDF = 33 (SolidWorks constant)
                const int swSaveAsPDF      = 33;
                const int swSaveAsCurrentVersion = 0;
                const int swSaveWithReferencesZipToFolder = 0;

                bool result = drawingDoc.Extension.SaveAs(
                    outputPath,
                    swSaveAsCurrentVersion,
                    swSaveWithReferencesZipToFolder,
                    null,
                    ref errors,
                    ref warnings);

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Log("Ошибка экспорта PDF: " + ex.Message, LogLevel.Error);
                return false;
            }
        }
    }
}
