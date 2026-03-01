using System;
using CreateChertAndRazverka.Helpers;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Adds reference dimensions to views in a SolidWorks drawing document.
    /// </summary>
    public class DimensionManager
    {
        private readonly SolidWorksConnector _connector;

        public DimensionManager(SolidWorksConnector connector)
        {
            _connector = connector ?? throw new ArgumentNullException("connector");
        }

        /// <summary>
        /// Inserts model-driven (reference) dimensions in all views of the given drawing.
        /// Uses swDimensionType_e and the drawing API to place dimensions automatically.
        /// </summary>
        public void AddDimensions(dynamic drawingDoc)
        {
            if (drawingDoc == null) return;

            try
            {
                // Iterate over all drawing views on the active sheet
                object[] views = drawingDoc.GetCurrentSheet()?.GetViews() as object[];
                if (views == null || views.Length == 0) return;

                foreach (dynamic view in views)
                {
                    if (view == null) continue;
                    try { AddDimensionsToView(drawingDoc, view); }
                    catch (Exception ex)
                    {
                        LogHelper.Log("Предупреждение при простановке размеров: " + ex.Message,
                            LogLevel.Warning);
                    }
                }

                drawingDoc.GraphicsRedraw2();
            }
            catch (Exception ex)
            {
                LogHelper.Log("Ошибка при автоматической простановке размеров: " + ex.Message,
                    LogLevel.Warning);
            }
        }

        private void AddDimensionsToView(dynamic drawingDoc, dynamic view)
        {
            // Select all edges in this view
            view.SelectAllAnnotations();

            // Use SolidWorks model-item insertion
            // InsertModelAnnotations3:
            //   src        = swImportModelItemsSource_e.swImportModelItemsFromEntireModel (8)
            //   type       = swInsertAnnotation_e flags (dimensions = 2, notes = 4, ref geom = 256)
            //   config     = false (not per configuration)
            //   eliminate  = true (eliminate duplicate dim values)
            //   combineView = false
            drawingDoc.Extension.InsertModelAnnotations3(
                8,      // swImportModelItemsFromEntireModel
                2,      // dimensions only
                false,
                true,
                false);
        }
    }
}
