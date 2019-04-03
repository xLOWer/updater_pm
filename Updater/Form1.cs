using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    public partial class Form1 : Form
    {
        public bool IsRemoteServerPinging = false;
        public bool IsPackageExists = false;
        Task task;

        static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        CancellationToken token = cancelTokenSource.Token;

        delegate void Message(string str, ListBox lb);
        static private void InvokeMessage(string str, ListBox lb)
        {
            lb.Invoke(new Action(() => { lb.Items.Add(str); }));
        }
        Message msg = InvokeMessage;

        private void err(Exception ex)
        {
            msg($"Message: {ex.Message}", logList);
            msg($"StackTrace: {ex.StackTrace}", logList);
            msg($"Source: {ex.Source}", logList);
            msg($"TargetSite: { ex.TargetSite}", logList);
        }


        public Form1()
        {
            InitializeComponent();
            
            try
            {
                OracleDbService.Configure();
            }
            catch (Exception ex) { err(ex); }
        }
        
        bool DownloadUpdate()
        {
            var action = "download";
            try
            {
                List<string> upd = new List<string>() { "COUNTRIES", "CUSTOMERS", "CONTRACTORS", "GROUPS", "GOODS", "GROUP_ITEMS", "ITEMS", "PRICES_CACHE", "BAR_CODES" };
                
                logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}] НАЧАТО СКАЧИВАНИЕ ОБНОВЛЕНИЯ"); }));
                string signature = UpdateStart(action);
                progressBar.Invoke(new Action(() => { progressBar.Maximum = upd.Count; }));

                for (int i = 0; i <= upd.Count - 1; ++i)
                {
                    Download(signature, upd[i], action);
                }

                UpdateEnd(signature, action);
                logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}] СКАЧИВАНИЕ ОБНОВЛЕНИЯ ЗАВЕРШЕНО"); }));
            }
            catch (Exception ex)
            {
                err(ex);
                SetControlsEnabling(true);
                return false;
            }

            return true;
        }

        bool ProcessUpdate()
        {
            var action = "process";
            try
            {
                List<string> upd = new List<string>() { "COUNTRIES", "CUSTOMERS", "CONTRACTORS", "GROUPS", "GOODS", "GROUP_ITEMS", "ITEMS", "PRICES_CACHE", "BAR_CODES" };

                logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}] НАЧАТА ОБРАБОТКА ОБНОВЛЕНИЯ"); }));
                string signature = UpdateStart(action);
                progressBar.Invoke(new Action(() => { progressBar.Maximum = upd.Count; }));

                for (int i = 0; i <= upd.Count - 1; ++i)
                {
                    Process(signature, upd[i], action);
                }

                UpdateEnd(signature, action);
                logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}] ОБРАБОТКА ОБНОВЛЕНИЯ ЗАВЕРШЕНА"); }));
            }
            catch (Exception ex)
            {
                err(ex);
                SetControlsEnabling(true);
                return false;
            }
            return true;
        }


        string UpdateStart(string comment)
        {
            progressBar.Invoke(new Action(() => { progressBar.Value = 0; }));
            SetControlsEnabling(false);
            var start_time = OracleDateFormat(DateTime.Now);
            OracleDbService.ConnOpen();

            string SERVER_HOST = OracleDbService.SelectSingleValue($"SELECT SYS_CONTEXT('USERENV','SERVER_HOST') FROM dual");
            string signature = OracleDbService.SelectSingleValue($"SELECT '{SERVER_HOST}_'||to_char({start_time},'ddmmyyyy') FROM dual");
            int signature_count
                = int.Parse(OracleDbService.SelectSingleValue($@"SELECT count(*) FROM abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" where SIGNATURE='{signature}' and ""COMMENT""='{comment}'"));
            if (signature_count == 0)
            {
                OracleDbService.Insert($@"
insert into abt.update_log2@""{AppConfig.OracleRemoteLinkName}""
( SIGNATURE,UPDATE_START,""IP_ADDRESS"",""NETWORK_PROTOCOL"",""OS_USER"",""PROXY_USER"",""SESSION_USER"",""host"",""COMMENT"")
values
('{signature}'
, {start_time}
, '{OracleDbService.SelectSingleValue("SELECT SYS_CONTEXT('USERENV','IP_ADDRESS') FROM dual")}'
, '{OracleDbService.SelectSingleValue("SELECT SYS_CONTEXT('USERENV','NETWORK_PROTOCOL') FROM dual")}'
, '{OracleDbService.SelectSingleValue("SELECT SYS_CONTEXT('USERENV','OS_USER') FROM dual")}'
, '{OracleDbService.SelectSingleValue("SELECT SYS_CONTEXT('USERENV','PROXY_USER') FROM dual")}'
, '{OracleDbService.SelectSingleValue("SELECT SYS_CONTEXT('USERENV','SESSION_USER') FROM dual")}'
, '{SERVER_HOST}'
, '{comment}')");

            }

            OracleDbService.Insert($@"
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
  WHERE SIGNATURE = '{signature}' and ""COMMENT""='{comment}'");

            return signature;
        }

        void UpdateEnd(string signature, string comment)
        {
            progressBar.Invoke(new Action(() => { progressBar.Value = 0; }));
            OracleDbService.Insert($@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" SET UPDATE_END = SYSDATE WHERE SIGNATURE = '{signature}' and ""COMMENT""='{comment}'");
            OracleDbService.conn.Close();
            progressBar.Invoke(new Action(() => { progressBar.Value = progressBar.Maximum; }));
            SetControlsEnabling(true);
        }


        void Download(string signature, string obj, string comment)
        {
            logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}] {obj} скачивается..."); }));
            var start_time = DateTime.Now;
            List<OracleCommand> sqls = new List<OracleCommand>();
            sqls.Add(new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" SET {obj}=0 WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            });
            sqls.Add(new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = $"ABT.PKG_UPDATE.{obj}"
            });
            OracleDbService.ExecuteCommand(sqls);
            sqls.Clear();
            var time = Math.Round((DateTime.Now - start_time).TotalDays * 24 * 60 * 60).ToString();
            sqls.Add(new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" " +
                              $@"SET {obj}={time} WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            });
            OracleDbService.ExecuteCommand(sqls);
            logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}]{obj} скачалось за {time} сек"); }));
            progressBar.Invoke(new Action(() => { progressBar.Value += 1; }));
        }

        void Process(string signature, string obj, string comment)
        {
            logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}] {obj} обрабатывается..."); }));
            var start_time = DateTime.Now;
            List<OracleCommand> sqls = new List<OracleCommand>();
            sqls.Add(new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" SET {obj}=0 WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            });
            sqls.Add(new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = $"ABT.ABT_REPLICATION.PROCESS_" + (obj == "PRICES_CACHE" ? "PRICES" : obj)
            });
            OracleDbService.ExecuteCommand(sqls);

            sqls.Clear();
            var time = Math.Round((DateTime.Now - start_time).TotalDays * 24 * 60 * 60).ToString();
            sqls.Add(new OracleCommand()
            {
                CommandText = $@"UPDATE abt.update_log2@""{AppConfig.OracleRemoteLinkName}"" " +
                              $@"SET {obj}={time} WHERE SIGNATURE='{signature}' and ""COMMENT""='{comment}'",
            });
            OracleDbService.ExecuteCommand(sqls);
            logList.Invoke(new Action(() => { logList.Items.Add($"[{DateFormat(DateTime.Now)}]{obj} обработано на {signature} за {time} сек"); }));
            progressBar.Invoke(new Action(() => { progressBar.Value += 1; }));
        }
        
        void SetControlsEnabling(bool enabled)
        {
            fullUpdateButton.Invoke(new Action(() => { fullUpdateButton.Enabled = enabled; }));
        }
        
        string OracleDateFormat(DateTime Date) => $"TO_DATE('{Date.Day}/{Date.Month}/{Date.Year} {Date.Hour}:{Date.Minute}:{Date.Second}','DD/MM/YYYY hh24:mi:ss')";

        string DateFormat(DateTime Date) => $"{Date.Day}/{Date.Month}/{Date.Year} {Date.Hour}:{Date.Minute}:{Date.Second}:{Date.Millisecond}";
        
        private void fullUpdateButton_Click(object sender, EventArgs e)
        {
            /*if (!PingHost(AppConfig.OracleRemoteHost))
            {
                MessageBox.Show("Сервер обновлений не доступен");
                return;
            }*/
            task = Task.Factory.StartNew(() =>
            {
                if (DownloadUpdate() && ProcessUpdate())
                {
                    MessageBox.Show(null, "Можете продолжить работу", "Обновление завершено!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            },
            token);
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
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

    }
}
