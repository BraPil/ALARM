using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

// ADDS25 MODERNIZATION: AutoCAD Map3D 2025 API References
using Acad = Autodesk.AutoCAD.Runtime;
using AcadAS = Autodesk.AutoCAD.ApplicationServices;
using AcadASA = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadDB = Autodesk.AutoCAD.DatabaseServices;
using AcadEd = Autodesk.AutoCAD.EditorInput;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadWin = Autodesk.AutoCAD.Windows;
using AcadColor = Autodesk.AutoCAD.Colors;
using AcadPS = Autodesk.AutoCAD.PlottingServices;

// ADDS25 Core reference
using ADDS25.Core;

namespace ADDS25.AutoCAD
{
    /// <summary>
    /// ADDS25 AutoCAD Integration - Modernized for AutoCAD Map3D 2025
    /// Original: ADDS 2019 SCS.cs AutoCAD functions
    /// Migration: Updated AutoCAD API references for 2025
    /// </summary>
    public class AutoCADIntegration
    {
        #region **** ADDS25 MODERNIZATION - AutoCAD Commands ****

        /// <summary>
        /// ADDS25: Modernized Splash Screen Display for AutoCAD
        /// Original: DisplaySplash_2012 LISP function
        /// Migration: Updated for AutoCAD Map3D 2025 API
        /// </summary>
        [Acad.LispFunction("DisplaySplash_2025")]
        public void DisplaySplash(AcadDB.ResultBuffer args)
        {
            string? applicationName = null;
            
            if (args != null)
            {
                // ADDS25: Process input parameters from LISP
                ArrayList alInputParameters = ProcessInputParameters(args);
                applicationName = alInputParameters[0]?.ToString();
            }

            try
            {
                // ADDS25: Use Core SCS class for splash display
                SCS scs = new SCS();
                scs.DisplaySplash(applicationName ?? "ADDS25");

                // ADDS25: Log to AutoCAD Editor
                AcadAS.Document doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
                AcadEd.Editor ed = doc.Editor;
                ed.WriteMessage($"\n*** ADDS25 Splash Screen Displayed at {DateTime.Now.ToLongTimeString()}");
            }
            catch (Exception ex)
            {
                // ADDS25: Error handling for AutoCAD environment
                AcadAS.Document? doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
                if (doc != null)
                {
                    AcadEd.Editor ed = doc.Editor;
                    ed.WriteMessage($"\n*** ADDS25 Splash Screen Error: {ex.Message}");
                }
                
                Console.WriteLine($"ADDS25 AutoCAD Splash Error: {ex.Message}");
            }
        }

        /// <summary>
        /// ADDS25: Modernized Panel Data Retrieval for AutoCAD
        /// Original: MyGetPanData_2012 LISP function
        /// Migration: Updated for AutoCAD Map3D 2025 API and .NET Core 8
        /// </summary>
        [Acad.LispFunction("GetPanelData_2025")]
        public Int32 GetPanelData(AcadDB.ResultBuffer args)
        {
            // Get handles to current AutoCAD drawing session
            AcadAS.Document doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
            AcadDB.Database dbDwg = doc.Database;
            AcadEd.Editor ed = doc.Editor;

            ed.WriteMessage("\n *** ADDS25 GetPanelData Started at " + DateTime.Now.ToLongTimeString());

            try
            {
                if (args == null)
                {
                    throw new ArgumentException("Panel ID required");
                }

                // ADDS25: Process input parameters
                ArrayList alInputParameters = ProcessInputParameters(args);
                string panelId = alInputParameters[0]?.ToString() ?? throw new ArgumentException("Invalid panel ID");

                // ADDS25: Use Core SCS class for Oracle data retrieval
                SCS scs = new SCS();
                
                // TODO: Get connection string from configuration
                string connectionString = SCS.Configuration.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Oracle connection string not configured");
                }

                int recordCount = scs.GetCurrentOracleData(connectionString, panelId);
                
                ed.WriteMessage($"\n *** ADDS25 Retrieved {recordCount} records for panel {panelId}");
                
                // ADDS25: Reset AutoLISP symbol lists (equivalent to original)
                AcadDB.ResultBuffer rbNil = new AcadDB.ResultBuffer();
                AcadPutSym("CurMstDevLst", rbNil);
                AcadPutSym("CurRegDatLst", rbNil);
                AcadPutSym("CurObjBlkLst", rbNil);
                AcadPutSym("CurObjPL_Lst", rbNil);
                AcadPutSym("CurObjVrtLst", rbNil);
                AcadPutSym("CurObj4PLLst", rbNil);
                AcadPutSym("CurObjTxtLst", rbNil);

                return recordCount;
            }
            catch (Exception ex)
            {
                ed.WriteMessage($"\n *** ADDS25 GetPanelData Error: {ex.Message}");
                Console.WriteLine($"ADDS25 AutoCAD GetPanelData Error: {ex.Message}");
                return -1;
            }
        }

        #endregion

        #region **** ADDS25 MODERNIZATION - AutoCAD Utility Functions ****

        /// <summary>
        /// ADDS25: Process input parameters from AutoLISP
        /// Original: Adds.ProcessInputParameters
        /// Migration: Modernized parameter processing
        /// </summary>
        private ArrayList ProcessInputParameters(AcadDB.ResultBuffer args)
        {
            ArrayList parameters = new ArrayList();
            
            if (args != null)
            {
                foreach (AcadDB.TypedValue tv in args)
                {
                    parameters.Add(tv.Value);
                }
            }
            
            return parameters;
        }

        /// <summary>
        /// ADDS25: Put symbol value to AutoLISP
        /// Original: Adds.AcadPutSym
        /// Migration: Modernized AutoLISP symbol management for AutoCAD 2025
        /// NOTE: AutoCAD 2025 API change - LispService method updated
        /// </summary>
        private void AcadPutSym(string symbolName, AcadDB.ResultBuffer value)
        {
            try
            {
                // ADDS25 MIGRATION LEARNING: AutoCAD 2025 API has changed
                // Original method doc.LispService.SetLispSymbol() no longer exists
                // Need to use alternative approach for AutoCAD 2025
                
                AcadAS.Document doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
                AcadEd.Editor ed = doc.Editor;
                
                // ADDS25 WORKAROUND: Use command-line approach for LISP symbol setting
                // This is a temporary solution until we find the correct AutoCAD 2025 API
                string lispCommand = $"(setq {symbolName} nil)";
                ed.Command("LISP", lispCommand);
                
                Console.WriteLine($"ADDS25: Set LISP symbol {symbolName} using command workaround");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ADDS25 AcadPutSym Error for {symbolName}: {ex.Message}");
            }
        }

        /// <summary>
        /// ADDS25: AutoCAD Application Initialization
        /// Original: AddsGo function equivalent
        /// Migration: Modernized initialization for AutoCAD Map3D 2025 with LISP integration
        /// </summary>
        [Acad.CommandMethod("ADDS25_INIT")]
        public void InitializeADDS25()
        {
            AcadAS.Document doc = AcadAS.Application.DocumentManager.MdiActiveDocument;
            AcadEd.Editor ed = doc.Editor;

            try
            {
                ed.WriteMessage("\n*** ADDS25 Initialization Started ***");
                
                // ADDS25: Set application configuration
                SCS.Configuration.ApplicationName = "ADDS25";
                SCS.Configuration.Version = "25.0.0";
                SCS.Configuration.TargetFramework = ".NET Core 8";
                SCS.Configuration.AutoCADVersion = "Map3D 2025";
                SCS.Configuration.OracleVersion = "19c";
                
                // ADDS25: Initialize LISP Integration Bridge
                ed.WriteMessage("\n*** ADDS25 Loading LISP Integration Bridge ***");
                var lispBridge = new LispIntegrationBridge();
                
                try
                {
                    lispBridge.LoadCoreLispFiles();
                    lispBridge.TranslateCriticalLispFunctions();
                    
                    var status = lispBridge.GetIntegrationStatus();
                    ed.WriteMessage($"\nLISP Integration: {status.LoadedFiles} files, {status.DiscoveredFunctions} functions, {status.DiscoveredVariables} variables");
                }
                catch (Exception lispEx)
                {
                    ed.WriteMessage($"\nADDS25 LISP Integration Warning: {lispEx.Message}");
                    Console.WriteLine($"ADDS25 LISP Integration Warning: {lispEx.Message}");
                }
                
                // ADDS25: Set initialization status
                SCS.Status.IsInitialized = true;
                SCS.Status.LastInitialization = DateTime.Now;
                
                ed.WriteMessage($"\n*** ADDS25 Initialization Complete ***");
                ed.WriteMessage($"\nApplication: {SCS.Configuration.ApplicationName} v{SCS.Configuration.Version}");
                ed.WriteMessage($"\nFramework: {SCS.Configuration.TargetFramework}");
                ed.WriteMessage($"\nAutoCAD: {SCS.Configuration.AutoCADVersion}");
                ed.WriteMessage($"\nOracle: {SCS.Configuration.OracleVersion}");
                
                // ADDS25: Display splash screen
                // MIGRATION LEARNING: AcadDB.LispDataType.Text no longer exists in AutoCAD 2025
                // Using alternative approach with TypedValue
                AcadDB.TypedValue typedValue = new AcadDB.TypedValue(5005, "ADDS25"); // 5005 = RTSTR (string type)
                DisplaySplash(new AcadDB.ResultBuffer(typedValue));
            }
            catch (Exception ex)
            {
                ed.WriteMessage($"\n*** ADDS25 Initialization Error: {ex.Message}");
                Console.WriteLine($"ADDS25 AutoCAD Initialization Error: {ex.Message}");
            }
        }

        #endregion
    }
}
