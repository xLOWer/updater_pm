using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    public partial class Form1 : Form
    {
        Task task;
        static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        CancellationToken token = cancelTokenSource.Token;
        delegate void Message(string str, ListView lb, Color col = new Color());
        Message msg = InvokeMessage;



        public Form1()
        {
            InitializeComponent();

            try
            {
                OracleDbService.Configure();
                ConfigureView();
            }
            catch (Exception ex)
            {
                err( ex );
            }

        }



        static private void InvokeMessage(string str, ListView lb, Color col = new Color())
        {
            lb.Invoke( new Action( () =>
            {
                lb.Items.Add( str );
                int c = lb.Items.Count;
                var element = lb.Items[c - 1];
                element.GetBounds( ItemBoundsPortion.ItemOnly );
                element.ToolTipText = str;
                element.BackColor = col;
            } ) );
        }

        private void err(Exception ex)
        {
            msg( $"Message: {ex.Message}", logList, Color.Red );
            msg( $"StackTrace: {ex.StackTrace}", logList, Color.Red );
            msg( $"Source: {ex.Source}", logList, Color.Red );
            msg( $"TargetSite: { ex.TargetSite}", logList, Color.Red );
        }

        private void topToolStrip(string str, string name)
        {
            if (!string.IsNullOrEmpty( str ))
                toolStrip1.Invoke( new Action( () =>
                {
                    toolStrip1.Items.Add( name + " " + str );
                    toolStrip1.Items.Add( new ToolStripSeparator() );
                } ) );
        }

        private void bottomStatusStrip(string str, string name)
        {
            if (!string.IsNullOrEmpty( str ))
                statusStrip1.Invoke( new Action( () =>
                {
                    statusStrip1.Items.Add( name + " " + str );
                    statusStrip1.Items.Add( new ToolStripSeparator() );
                } ) );
        }

        bool DownloadUpdate()
        {
            var action = "download";
            try
            {
                List<string> upd = new List<string>()
                {
                    "COUNTRIES",
                    "CUSTOMERS",
                    "CONTRACTORS",
                    "GROUPS",
                    "GOODS",
                    "GROUP_ITEMS",
                    "ITEMS",
                    "PRICES_CACHE",
                    "BAR_CODES"
                };

                msg( $"[{DateFormat( DateTime.Now )}] НАЧАТО СКАЧИВАНИЕ ОБНОВЛЕНИЯ", logList );
                string signature = UpdateStart( action );
                progressBar.Invoke( new Action( () => { progressBar.Maximum = upd.Count; } ) );

                for (int i = 0; i <= upd.Count - 1; ++i)
                {
                    Download( signature, upd[i], action );
                }

                UpdateEnd( signature, action );
                msg( $"[{DateFormat( DateTime.Now )}] СКАЧИВАНИЕ ОБНОВЛЕНИЯ ЗАВЕРШЕНО", logList );
            }
            catch (Exception ex)
            {
                err( ex );
                SetControlsEnabling( true );
                return false;
            }

            return true;
        }

        bool ProcessUpdate()
        {
            var action = "process";
            try
            {
                List<string> upd = new List<string>()
                {
                    "COUNTRIES",
                    "CUSTOMERS",
                    "CONTRACTORS",
                    "GROUPS",
                    "GOODS",
                    "GROUP_ITEMS",
                    "ITEMS",
                    "PRICES_CACHE",
                    "BAR_CODES"
                };

                msg( $"[{DateFormat( DateTime.Now )}] НАЧАТА ОБРАБОТКА ОБНОВЛЕНИЯ", logList );
                string signature = UpdateStart( action );
                progressBar.Invoke( new Action( () => { progressBar.Maximum = upd.Count; } ) );

                for (int i = 0; i <= upd.Count - 1; ++i)
                {
                    Process( signature, upd[i], action );
                }

                UpdateEnd( signature, action );
                msg( $"[{DateFormat( DateTime.Now )}] ОБРАБОТКА ОБНОВЛЕНИЯ ЗАВЕРШЕНА", logList );
            }
            catch (Exception ex)
            {
                err( ex );
                SetControlsEnabling( true );
                return false;
            }
            return true;
        }



        string UpdateStart(string comment)
        {
            progressBar.Invoke( new Action( () => { progressBar.Value = 0; } ) );
            SetControlsEnabling( false );
            var start_time = OracleDateFormat( DateTime.Now );
            OracleDbService.ConnOpen();

            string SERVER_HOST = OracleDbService.SelectSingleValue( $"SELECT SYS_CONTEXT('USERENV','SERVER_HOST') FROM dual" );
            OracleDbService.ConnOpen();
            string signature = OracleDbService.SelectSingleValue( $"SELECT '{SERVER_HOST}_'||to_char({start_time},'ddmmyyyy') FROM dual" );
            int signature_count
                = int.Parse( OracleDbService.SelectSingleValue(
                    $@"SELECT count(*) FROM abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" where SIGNATURE='{signature}' and ""COMMENT""='{comment}'" ) );
            if (signature_count == 0)
            {
                OracleDbService.ConnOpen();
                OracleDbService.Insert( $@"
insert into abt.update_log2@""{AppConfig.OracleRemoteLinkName}""
( SIGNATURE,UPDATE_START,""IP_ADDRESS"",""NETWORK_PROTOCOL"",""OS_USER"",""PROXY_USER"",""SESSION_USER"",""host"",""COMMENT"")
values
('{signature}'
, {start_time}
, '{OracleDbService.SelectSingleValue( "SELECT SYS_CONTEXT('USERENV','IP_ADDRESS') FROM dual" )}'
, '{OracleDbService.SelectSingleValue( "SELECT SYS_CONTEXT('USERENV','NETWORK_PROTOCOL') FROM dual" )}'
, '{OracleDbService.SelectSingleValue( "SELECT SYS_CONTEXT('USERENV','OS_USER') FROM dual" )}'
, '{OracleDbService.SelectSingleValue( "SELECT SYS_CONTEXT('USERENV','PROXY_USER') FROM dual" )}'
, '{OracleDbService.SelectSingleValue( "SELECT SYS_CONTEXT('USERENV','SESSION_USER') FROM dual" )}'
, '{SERVER_HOST}'
, '{comment}')" );

            }

            OracleDbService.ConnOpen();
            OracleDbService.Insert( $@"
UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" SET
  BAR_CODES = null
, CONTRACTORS = null
, COUNTRIES = null
, CUSTOMERS = null
, GOODS = null
, GROUPS = null
, GROUP_ITEMS = null
, ITEMS = null
, PRICES_CACHE = null
, UPDATE_START = {start_time}
, UPDATE_END = null
  WHERE SIGNATURE = '{signature}' and ""COMMENT""='{comment}'" );

            return signature;
        }

        void UpdateEnd(string signature, string comment)
        {
            progressBar.Invoke( new Action( () => { progressBar.Value = 0; } ) );
            OracleDbService.Insert(
                $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" SET UPDATE_END = SYSDATE WHERE SIGNATURE = '{signature}' and ""COMMENT""='{comment}'" );
            OracleDbService.conn.Close();
            progressBar.Invoke( new Action( () => { progressBar.Value = progressBar.Maximum; } ) );
            SetControlsEnabling( true );
        }



        void Download(string signature, string obj, string comment)
        {
            msg( $"[{DateFormat( DateTime.Now )}] {obj} скачивается...", logList );
            var start_time = DateTime.Now;
            List<OracleCommand> sqls = new List<OracleCommand>();
            sqls.Add( new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" SET {obj}=0 WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            } );
            sqls.Add( new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = $"ABT.PKG_UPDATE.{obj}"
            } );
            OracleDbService.ExecuteCommand( sqls );
            sqls.Clear();
            var time = Math.Round( (DateTime.Now - start_time).TotalDays * 24 * 60 * 60 ).ToString();
            sqls.Add( new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" " +
                              $@"SET {obj}={time} WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            } );
            OracleDbService.ExecuteCommand( sqls );
            msg( $"[{DateFormat( DateTime.Now )}]{obj} скачалось за {time} сек", logList );
            progressBar.Invoke( new Action( () => { progressBar.Value += 1; } ) );
        }

        void Process(string signature, string obj, string comment)
        {
            msg( $"[{DateFormat( DateTime.Now )}] {obj} обрабатывается...", logList );
            var start_time = DateTime.Now;
            List<OracleCommand> sqls = new List<OracleCommand>();
            sqls.Add( new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" SET {obj}=0 WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            } );
            sqls.Add( new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = $"ABT.ABT_REPLICATION.PROCESS_" + (obj == "PRICES_CACHE" ? "PRICES" : obj)
            } );
            OracleDbService.ExecuteCommand( sqls );

            sqls.Clear();
            var time = Math.Round( (DateTime.Now - start_time).TotalDays * 24 * 60 * 60 ).ToString();
            sqls.Add( new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" " +
                              $@"SET {obj}={time} WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            } );
            OracleDbService.ExecuteCommand( sqls );
            msg( $"[{DateFormat( DateTime.Now )}]{obj} обработано на {signature} за {time} сек", logList );
            progressBar.Invoke( new Action( () => { progressBar.Value += 1; } ) );
        }

        void SetControlsEnabling(bool enabled)
        {
            fullUpdateButton.Invoke( new Action( () => { fullUpdateButton.Enabled = enabled; } ) );
        }

        string OracleDateFormat(DateTime Date)
            => $"TO_DATE('{Date.Day}/{Date.Month}/{Date.Year} {Date.Hour}:{Date.Minute}:{Date.Second}','DD/MM/YYYY hh24:mi:ss')";

        string DateFormat(DateTime Date)
            => $"{Date.Day}/{Date.Month}/{Date.Year} {Date.Hour}:{Date.Minute}:{Date.Second}:{Date.Millisecond}";

        private void fullUpdateButton_Click(object sender, EventArgs e)
        {
            msg( "ожидайте проверки всех параметров...", logList );
            task = Task.Factory.StartNew( () =>
            {
                OracleDbService.ConnOpen();

                if (!IsLinkExist)
                {
                    msg( "линк\t...\tFAIL\t\t попытка пересоздать линк...", logList, Color.Orange );
                    CreateLink();
                    Task.Factory.StartNew( () =>
                    {
                        ConfigureView();
                    } );
                    msg( "линк\t...\tОК", logList, Color.LightGreen );

                }
                else
                { msg( "линк\t...\tОК", logList, Color.LightGreen ); }

                if (!IsLinkHasCorrectUser)
                {
                    msg( "юзер\t...\tFAIL\t\t попытка пересоздать юзера...", logList, Color.Orange );
                    RemoveLink();
                    CreateLink();
                    Task.Factory.StartNew( () =>
                    {
                        ConfigureView();
                    } );
                    msg( "юзер\t...\tОК", logList, Color.LightGreen );
                }
                else
                { msg( "юзер\t...\tОК", logList, Color.LightGreen ); }

                if (!IsPkgExist || !IsPkgValid || !IsPkgBodyValid)
                {
                    msg( "пакет\t...\tINVALID\t\t попытка пересоздать пакет...", logList, Color.Orange );
                    RemovePkg();
                    CreatePkg();
                    CreatePkgBody();
                    CompilePkg();
                    CompilePkgBody();
                    Task.Factory.StartNew( () =>
                    {
                        ConfigureView();
                    } );
                    msg( "пакет\t...\tVALID", logList, Color.LightGreen );
                }
                else
                { msg( "пакет\t...\tVALID", logList, Color.LightGreen ); }

                OracleDbService.conn.Open();
                if (DownloadUpdate())
                {
                    OracleDbService.conn.Open();
                    if (ProcessUpdate())
                    {
                        ConfigureView();
                        MessageBox.Show( null,
                            "Можете продолжить работу",
                            "Обновление завершено!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information );
                    }
                }
                OracleDbService.conn.Close();
            },
            token );
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send( nameOrAddress );
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        private void RefreshStrip()
        {
            OracleDbService.ConnOpen();
            topToolStrip(
                OracleDbService.SelectSingleValue( @"SELECT
( SELECT OBJECT_NAME FROM all_objects WHERE UPPER( OBJECT_NAME ) like UPPER( '%pkg_update%' ) AND OBJECT_TYPE LIKE 'PACKAGE' ) ||
'(' || (SELECT STATUS FROM all_objects WHERE UPPER( OBJECT_NAME ) like UPPER( '%pkg_update%' ) AND OBJECT_TYPE LIKE 'PACKAGE' ) ||
',' || (SELECT STATUS FROM all_objects WHERE UPPER( OBJECT_NAME ) like UPPER( '%pkg_update%' ) AND OBJECT_TYPE LIKE 'PACKAGE BODY' ) || ')'
FROM dual" ),
                "" );

            topToolStrip(
                OracleDbService.SelectSingleValue(
                    $"SELECT USERNAME||'@'||DB_LINK from ALL_DB_LINKS WHERE db_link LIKE '%{AppConfig.OracleRemoteLinkName}%'" ),
                "линк:" );

            bottomStatusStrip( OracleDbService.SelectSingleValue(
                $@"SELECT * FROM dual@""{AppConfig.OracleRemoteLinkName}""" ),
                "" );

            bottomStatusStrip(
                OracleDbService.SelectSingleValue(
                    "SELECT * FROM v$version WHERE BANNER LIKE '%Database%'" ),
                "" );

            topToolStrip(
                OracleDbService.SelectSingleValue(
                    $@"SELECT MAX(UPDATE_END) FROM abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" WHERE SIGNATURE LIKE SYS_CONTEXT('USERENV','SERVER_HOST')|| '%'" ),
                "посл. обнов.:" );

            OracleDbService.conn.Close();
        }

        private void ConfigureView()
        {
            Task.Factory.StartNew( () =>
            {
                statusStrip1.Invoke( new Action( () => { statusStrip1.Items.Clear(); } ) );
                toolStrip1.Invoke( new Action( () => { toolStrip1.Items.Clear(); } ) );

                RefreshStrip();
            } );
        }

        private void RemovePkg()
        {
            OracleDbService.ExecuteCommand( new OracleCommand() { CommandText = "DROP PACKAGE ABT.PKG_UPDATE" } );
        }

        private void CreatePkg()
        {
            OracleDbService.ExecuteCommand( new OracleCommand()
            {
                CommandText =
"CREATE OR REPLACE PACKAGE ABT.pkg_update IS \n" +
"\tPROCEDURE BAR_CODES; \n" +
"\tPROCEDURE CONTRACTORS; \n" +
"\tPROCEDURE COUNTRIES; \n" +
"\tPROCEDURE CUSTOMERS; \n" +
"\tPROCEDURE GOODS; \n" +
"\tPROCEDURE GROUPS; \n" +
"\tPROCEDURE GROUP_ITEMS; \n" +
"\tPROCEDURE ITEMS; \n" +
"\tPROCEDURE PRICES_CACHE; \n" +
"END pkg_update; \n"
            } );
        }
        private void CreatePkgBody()
        {
            OracleDbService.ExecuteCommand( new OracleCommand()
            {
                CommandText = "CREATE OR REPLACE PACKAGE BODY pkg_update IS \n" +
"PROCEDURE BAR_CODES IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_BAR_CODES;\n" +
$"	INSERT INTO ABT.IMPORT_BAR_CODES SELECT * FROM ABT.UPDATE_BAR_CODES@\"{AppConfig.OracleRemoteLinkName}\"; COMMIT;\n" +
"END BAR_CODES;\n" +
"PROCEDURE CONTRACTORS IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_CONTRACTORS;\n" +
$"	INSERT INTO ABT.IMPORT_CONTRACTORS SELECT * FROM ABT.UPDATE_CONTRACTORS@\"{AppConfig.OracleRemoteLinkName}\" WHERE ID_DISTRICT IN (SELECT ID FROM REF_DISTRICTS RD); COMMIT;\n" +
"END CONTRACTORS;\n" +
"PROCEDURE COUNTRIES IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_COUNTRIES;\n" +
$"	INSERT INTO ABT.IMPORT_COUNTRIES SELECT * FROM ABT.UPDATE_COUNTRIES@\"{AppConfig.OracleRemoteLinkName}\"; COMMIT;\n" +
"END COUNTRIES;\n" +
"PROCEDURE CUSTOMERS IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_CUSTOMERS;\n" +
$"	INSERT INTO ABT.IMPORT_CUSTOMERS SELECT * FROM ABT.UPDATE_CUSTOMERS@\"{AppConfig.OracleRemoteLinkName}\"; COMMIT;\n" +
"END CUSTOMERS;\n" +
"PROCEDURE GOODS IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_GOODS;\n" +
$"	INSERT INTO ABT.IMPORT_GOODS SELECT * FROM ABT.UPDATE_GOODS@\"{AppConfig.OracleRemoteLinkName}\"; COMMIT;\n" +
"END GOODS;\n" +
"PROCEDURE GROUPS IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_GROUPS;\n" +
$"	INSERT INTO ABT.IMPORT_GROUPS SELECT * FROM ABT.UPDATE_GROUPS@\"{AppConfig.OracleRemoteLinkName}\"; COMMIT;\n" +
"END GROUPS;\n" +
"PROCEDURE GROUP_ITEMS IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_GROUP_ITEMS;\n" +
$"	INSERT INTO ABT.IMPORT_GROUP_ITEMS SELECT * FROM ABT.UPDATE_GROUP_ITEMS@\"{AppConfig.OracleRemoteLinkName}\"; COMMIT;\n" +
"END GROUP_ITEMS;\n" +
"PROCEDURE ITEMS IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_ITEMS;\n" +
$"	INSERT INTO ABT.IMPORT_ITEMS SELECT * FROM ABT.UPDATE_ITEMS@\"{AppConfig.OracleRemoteLinkName}\"; COMMIT;\n" +
"END ITEMS;\n" +
"PROCEDURE PRICES_CACHE IS BEGIN\n" +
"	DELETE FROM ABT.IMPORT_PRICES_CACHE;\n" +
$"	INSERT INTO ABT.IMPORT_PRICES_CACHE SELECT * FROM ABT.UPDATE_PRICES_CACHE@\"{AppConfig.OracleRemoteLinkName}\" WHERE ID_PRICE_TYPE IN (SELECT ID FROM REF_PRICE_TYPES); COMMIT;" +
"END PRICES_CACHE;\n" +
"END pkg_update;"
            } );
        }
        private void CompilePkg()
        {
            OracleDbService.ExecuteCommand( new OracleCommand()
            {
                CommandText = "ALTER PACKAGE ABT.PKG_UPDATE COMPILE SPECIFICATION"
            } );
        }
        private void CompilePkgBody()
        {
            OracleDbService.ExecuteCommand( new OracleCommand()
            {
                CommandText = "ALTER PACKAGE ABT.PKG_UPDATE COMPILE BODY"
            } );
        }
        private void RemoveLink()
        {
            OracleDbService.ExecuteCommand( new OracleCommand()
            {
                CommandText = $@"DROP PUBLIC DATABASE LINK  ""{AppConfig.OracleRemoteLinkName}"""
            } );

        }
        private void CreateLink()
        {
            OracleDbService.ExecuteCommand( new OracleCommand()
            {
                CommandText = $@"CREATE PUBLIC DATABASE LINK ""{AppConfig.OracleRemoteLinkName}""
CONNECT TO {AppConfig.OracleRemoteUser} IDENTIFIED BY ""{AppConfig.OracleRemotePassword}"" USING '{AppConfig.OracleRemoteLinkTNS}'"
            } );
        }
        private bool IsPkgExist
            => !String.IsNullOrEmpty( OracleDbService.SelectSingleValue(
                "SELECT OBJECT_NAME FROM all_objects WHERE UPPER(OBJECT_NAME) like UPPER('%pkg_update%') AND OBJECT_TYPE LIKE 'PACKAGE'"
                ) );
        private bool IsPkgValid
            => OracleDbService.SelectSingleValue(
                "SELECT STATUS FROM all_objects WHERE UPPER(OBJECT_NAME) like UPPER('%pkg_update%') AND OBJECT_TYPE LIKE 'PACKAGE'"
                ) == "VALID";
        private bool IsPkgBodyValid
            => OracleDbService.SelectSingleValue(
                "SELECT STATUS FROM all_objects WHERE UPPER(OBJECT_NAME) like UPPER('%pkg_update%') AND OBJECT_TYPE LIKE 'PACKAGE BODY'"
                ) == "VALID";
        private bool IsLinkExist
            => OracleDbService.SelectSingleValue(
                $"SELECT DB_LINK FROM ALL_DB_LINKS WHERE DB_LINK LIKE '%{AppConfig.OracleRemoteLinkName}%'"
                ) == AppConfig.OracleRemoteLinkName;
        private bool IsLinkHasCorrectUser
            => OracleDbService.SelectSingleValue(
                $"SELECT lower(USER) FROM ALL_DB_LINKS WHERE DB_LINK LIKE '%{AppConfig.OracleRemoteLinkName}%'"
                ) == AppConfig.OracleRemoteUser;
        private bool IsLinkAvailable
            => OracleDbService.SelectSingleValue(
                $"SELECT 'OK' from dual@\"{AppConfig.OracleRemoteLinkName}\""
                ) == "OK";

        private void download_new_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start( "update.bat" );
        }
    }
}

