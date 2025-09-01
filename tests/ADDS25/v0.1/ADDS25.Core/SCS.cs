using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

// ADDS25 MODERNIZATION: Updated Oracle reference for .NET Core 8
using OracleDb = Oracle.ManagedDataAccess.Client;   // MIGRATION: Oracle.DataAccess.Client → Oracle.ManagedDataAccess.Client

// ADDS25 MODERNIZATION: AutoCAD Map3D 2025 API references (to be added via AutoCAD project)
// using Acad = Autodesk.AutoCAD.Runtime;
// using AcadAS = Autodesk.AutoCAD.ApplicationServices;
// using AcadASA = Autodesk.AutoCAD.ApplicationServices.Application;
// using AcadDB = Autodesk.AutoCAD.DatabaseServices;
// using AcadEd = Autodesk.AutoCAD.EditorInput;
// using AcadGeo = Autodesk.AutoCAD.Geometry;
// using AcadWin = Autodesk.AutoCAD.Windows;
// using AcadColor = Autodesk.AutoCAD.Colors;
// using AcadPS = Autodesk.AutoCAD.PlottingServices;

namespace ADDS25.Core
{
    /// <summary>
    /// ADDS25 Core SCS Class - Modernized for .NET Core 8, AutoCAD Map3D 2025, Oracle 19c
    /// Original: ADDS 2019 SCS.cs (1,480 lines)
    /// Migration: Legacy Oracle.DataAccess → Oracle.ManagedDataAccess.Core
    /// </summary>
    public partial class SCS
    {
        static internal string? strDeveId = null;
        static internal string? g_strApplName = null;
        static internal bool g_flgAcadIsBusy = false;

        #region **** ADDS25 MODERNIZATION - Core Database Operations ****

        /// <summary>
        /// ADDS25: Modernized Oracle Data Retrieval
        /// Original: MyGetPanData_2012 with Oracle.DataAccess.Client
        /// Migration: Updated for Oracle.ManagedDataAccess.Core and .NET Core 8
        /// </summary>
        public Int32 GetCurrentOracleData(string connectionString, string panelId)
        {
            Console.WriteLine($"*** ADDS25 GetCurrentOracleData Started at {DateTime.Now.ToLongTimeString()}");

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(panelId))
            {
                throw new ArgumentException("Connection string and panel ID are required");
            }

            string strPanID = panelId;
            Int32 intCount = 0;
            
            if (!decimal.TryParse(strPanID, out decimal decPanID))
            {
                throw new ArgumentException($"Invalid panel ID format: {strPanID}");
            }

            // ADDS25 MODERNIZATION: SQL queries preserved from original ADDS 2019
            StringBuilder sbSQL007 = new StringBuilder();   // Info from AddsDB.ObjMstDev table
            sbSQL007.Append("SELECT omd.device_id, to_char(omd.edit_dtm,'MM-DD-YYYY HH24:MI:SS'), omd.status_flag, omd.adds_panel_id ");
            sbSQL007.Append("FROM addsdb.objmstdev omd ");
            sbSQL007.Append("WHERE omd.adds_panel_id = :panID");

            StringBuilder sbSQL006 = new StringBuilder();   // Info from AddsDB.ObjRgApp table
            sbSQL006.Append("SELECT orp.device_id, orp.xdtname, orp.xdtvalue ");
            sbSQL006.Append("FROM addsdb.objrgapp orp ");
            sbSQL006.Append("WHERE orp.device_id in (select omd.device_id from addsdb.objmstdev omd ");
            sbSQL006.Append("WHERE omd.adds_panel_id = :panID) ");

            StringBuilder sbSQL004 = new StringBuilder();   // Info from AddsDB.ObjBlk table
            sbSQL004.Append("SELECT obk.adds_blk_nam, obk.blkhandle, obk.adds_layer_nam,  ");
            sbSQL004.Append("       ROUND(obk.blkpntx, 4), ROUND(obk.blkpnty, 4), ROUND(obk.blkpntz, 4), ROUND(obk.blksclx, 4), ROUND(obk.blkscly, 4), ROUND(obk.blksclz, 4), ");
            sbSQL004.Append("       ROUND(NVL2(obk.blkrot, obk.blkrot, 0.0), 4) AS blkrot, NVL2(obk.blkspace, obk.blkspace, 0.0) AS blkspace, NVL2(obk.blkattflw, obk.blkattflw, 0) AS blkattflw, ");
            sbSQL004.Append("       obk.device_id, obk.adds_panel_id, obk.operpt_num, obk.feeder_num ");
            sbSQL004.Append("FROM addsdb.objblk obk ");
            sbSQL004.Append("WHERE obk.adds_panel_id = :panID");

            StringBuilder sbSQL005 = new StringBuilder();   // Info from AddsDB.ObjAtt table
            sbSQL005.Append("SELECT oat.atttag, oat.attvalue, oat.atthandle, oat.adds_layer_nam, ");
            sbSQL005.Append("       ROUND(oat.attnpntx, 4), ROUND(oat.attnpnty, 4), ROUND(oat.attnpntz, 4), ");
            sbSQL005.Append("       ROUND(NVL2(oat.attjpntx, oat.attjpntx, 0.0), 4) AS attjpntx,  ROUND(NVL2(oat.attjpnty, oat.attjpnty,  0.0), 4) AS attjpnty, ROUND(NVL2(oat.attjpntz,oat.attjpntz, 0.0), 4) AS attjpntz, ");
            sbSQL005.Append("       NVL2(oat.attthick, oat.attthick, 0.0) AS attthick, NVL2(oat.atthgt, oat.atthgt, 0.0) AS atthgt, ");
            sbSQL005.Append("       ROUND(NVL2(oat.attrot, oat.attrot, 0.0), 6) as attrot, oat.adds_style, NVL2(oat.attflag, oat.attflag, 0) as attflag, ");
            sbSQL005.Append("       NVL2(oat.atttxtflag, oat.atttxtflag, 0.0) as atttxtflag, NVL2(oat.atthorzflag, oat.atthorzflag, 0.0) as atthorzflag,  ");
            sbSQL005.Append("       NVL2(oat.attvertflag, oat.attvertflag, 0.0) as attvertflag, oat.device_id ");
            sbSQL005.Append("FROM addsdb.objatt oat ");
            sbSQL005.Append("WHERE (oat.device_id in  ");
            sbSQL005.Append("           (select omd.device_id from addsdb.objmstdev omd where omd.adds_panel_id = :panID) )");

            DataSet ds = new DataSet();

            try
            {
                // ADDS25 MODERNIZATION: Updated Oracle connection for .NET Core 8
                using (OracleDb.OracleConnection oracleConn = new OracleDb.OracleConnection(connectionString))
                {
                    oracleConn.Open();

                    // Setup Oracle Command - MODERNIZED for Oracle.ManagedDataAccess.Core
                    OracleDb.OracleCommand oracleCommand = new OracleDb.OracleCommand();
                    oracleCommand.Connection = oracleConn;
                    oracleCommand.CommandType = CommandType.Text;
                    oracleCommand.CommandText = sbSQL007.ToString();

                    // Add SQL parameters - MODERNIZED parameter handling
                    oracleCommand.Parameters.Add("panID", OracleDb.OracleDbType.Decimal).Value = decPanID;

                    // Get ObjMstDev - Oracle data from DB & store in DataSet
                    OracleDb.OracleDataAdapter da = new OracleDb.OracleDataAdapter(oracleCommand);
                    da.TableMappings.Add("Table", "ObjMstDev");
                    da.Fill(ds);
                    intCount = ds.Tables["ObjMstDev"]?.Rows.Count ?? 0;

                    Console.WriteLine($"ADDS25: Retrieved {intCount} records from ObjMstDev for panel {panelId}");

                    // Get ObjRgApp - Oracle data from DB & store in DataSet
                    oracleCommand.CommandText = sbSQL006.ToString();
                    OracleDb.OracleDataAdapter daObjRgApp = new OracleDb.OracleDataAdapter(oracleCommand);
                    daObjRgApp.TableMappings.Add("Table", "ObjRgApp");
                    daObjRgApp.Fill(ds);

                    // Get ObjBlk - Oracle data from DB & store in DataSet
                    oracleCommand.CommandText = sbSQL004.ToString();
                    OracleDb.OracleDataAdapter daObjBlk = new OracleDb.OracleDataAdapter(oracleCommand);
                    daObjBlk.TableMappings.Add("Table", "ObjBlk");
                    daObjBlk.Fill(ds);

                    // Get ObjAtt - Oracle data from DB & store in DataSet
                    oracleCommand.CommandText = sbSQL005.ToString();
                    OracleDb.OracleDataAdapter daObjAtt = new OracleDb.OracleDataAdapter(oracleCommand);
                    daObjAtt.TableMappings.Add("Table", "ObjAtt");
                    daObjAtt.Fill(ds);

                    Console.WriteLine($"ADDS25: Complete data retrieval successful - {ds.Tables.Count} tables loaded");
                }
            }
            catch (OracleDb.OracleException oex)
            {
                Console.WriteLine($"ADDS25 Oracle Error: {oex.Message}");
                Console.WriteLine($"Oracle Error Code: {oex.Number}");
                throw new Exception($"ADDS25 Oracle Database Error: {oex.Message}", oex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ADDS25 General Error: {ex.Message}");
                throw new Exception($"ADDS25 Data Retrieval Error: {ex.Message}", ex);
            }

            return intCount;
        }

        /// <summary>
        /// ADDS25: Modernized Oracle Data Insertion
        /// Original: PutNewOraData with Oracle.DataAccess.Client
        /// Migration: Updated for Oracle.ManagedDataAccess.Core and .NET Core 8
        /// </summary>
        public bool PutNewOracleData(string connectionString, string panelId, DataSet dataToInsert)
        {
            Console.WriteLine($"*** ADDS25 PutNewOracleData Started at {DateTime.Now.ToLongTimeString()}");

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(panelId) || dataToInsert == null)
            {
                throw new ArgumentException("Connection string, panel ID, and data are required");
            }

            try
            {
                // ADDS25 MODERNIZATION: Updated Oracle connection for .NET Core 8
                using (OracleDb.OracleConnection oracleConn = new OracleDb.OracleConnection(connectionString))
                {
                    oracleConn.Open();

                    // ADDS25: Begin transaction for data integrity
                    using (OracleDb.OracleTransaction transaction = oracleConn.BeginTransaction())
                    {
                        try
                        {
                            // TODO: Implement complete data insertion logic from original ADDS 2019
                            // This is a placeholder for the full implementation
                            Console.WriteLine($"ADDS25: Data insertion logic to be implemented for panel {panelId}");
                            
                            transaction.Commit();
                            Console.WriteLine("ADDS25: Data insertion transaction committed successfully");
                            return true;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            Console.WriteLine("ADDS25: Data insertion transaction rolled back due to error");
                            throw;
                        }
                    }
                }
            }
            catch (OracleDb.OracleException oex)
            {
                Console.WriteLine($"ADDS25 Oracle Error: {oex.Message}");
                throw new Exception($"ADDS25 Oracle Database Error: {oex.Message}", oex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ADDS25 General Error: {ex.Message}");
                throw new Exception($"ADDS25 Data Insertion Error: {ex.Message}", ex);
            }
        }

        #endregion

        #region **** ADDS25 MODERNIZATION - Splash Screen System ****

        /// <summary>
        /// ADDS25: Modernized Splash Screen Display
        /// Original: DisplaySplash_2012 with Windows Forms
        /// Migration: Maintained Windows Forms compatibility for .NET Core 8
        /// Note: AutoCAD integration will be handled in ADDS25.AutoCAD project
        /// </summary>
        public void DisplaySplash(string applicationName)
        {
            g_strApplName = applicationName ?? "ADDS25";

            try
            {
                // ADDS25: Create modernized splash screen form
                // TODO: Implement frmAcadSplash equivalent for ADDS25
                Console.WriteLine($"ADDS25: Displaying splash screen for {g_strApplName}");
                
                // Placeholder for splash screen implementation
                MessageBox.Show($"ADDS25 Splash Screen\n\nApplication: {g_strApplName}\nVersion: .NET Core 8\nAutoCAD: Map3D 2025\nOracle: 19c", 
                               "ADDS25 - Modernized", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ADDS25 Splash Screen Error: {ex.Message}");
                MessageBox.Show($"ADDS25 Splash Screen Error: {ex.Message}", "ADDS25 Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region **** ADDS25 MODERNIZATION - Configuration and Initialization ****

        /// <summary>
        /// ADDS25: Application Configuration Properties
        /// Migration: Modernized configuration management
        /// </summary>
        public static class Configuration
        {
            public static string ConnectionString { get; set; } = string.Empty;
            public static string ApplicationName { get; set; } = "ADDS25";
            public static string Version { get; set; } = "25.0.0";
            public static string TargetFramework { get; set; } = ".NET Core 8";
            public static string AutoCADVersion { get; set; } = "Map3D 2025";
            public static string OracleVersion { get; set; } = "19c";
        }

        /// <summary>
        /// ADDS25: Initialization Status
        /// Migration: Modernized status tracking
        /// </summary>
        public static class Status
        {
            public static bool IsInitialized { get; set; } = false;
            public static bool IsOracleConnected { get; set; } = false;
            public static bool IsAutoCADConnected { get; set; } = false;
            public static DateTime LastInitialization { get; set; } = DateTime.MinValue;
        }

        #endregion
    }
}
