using Devart.Data.Oracle;
//using Oracle.DataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Updater
{
    internal static partial class OracleDbService
    {
        internal static OracleConnection conn { get; set; }
        internal static string LocalConnString
        {
            get
            {
                return $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {AppConfig.OracleLocalHost})(PORT = {AppConfig.OracleLocalPort}))(CONNECT_DATA = "
                        + $"(SERVER = DEDICATED)(SERVICE_NAME = {AppConfig.OracleLocalSID})));Password={AppConfig.OracleLocalPassword};User ID={AppConfig.OracleLocalUser}";
            }
            set { LocalConnString = value; }
        }

        internal static void Configure(string connString)
        {
            OracleDbService.LocalConnString = connString;
            conn = new OracleConnection(connString);            
        }

        internal static void Configure()
        {
            conn = new OracleConnection(LocalConnString);
        }

        internal static void ConnOpen()
        {
            if (conn.State == ConnectionState.Open) return;
            conn.Open();
        }

        internal static OracleDataReader Select(OracleCommand command)
        {
            OracleDataReader reader;

            using (command)
            {
                reader = command.ExecuteReader();
            }

            return reader;
        }

        internal static void ExecuteCommand(List<OracleCommand> commands)
        {
            int c = commands.Count();
            for (int i = 1; i <= c; ++i)
            {
                ExecuteCommand(commands[i - 1]);
            }

        }
        
        internal static void ExecuteCommand(OracleCommand command)
        {
            using (command)
            {
                command.Connection = conn;                
                command.ExecuteNonQuery();                
            }
        }
        
        internal static string SelectSingleValue(string Sql)
        {
            OracleDataReader reader;
            string retVal = "";
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = OracleDbService.conn;
                command.CommandType = CommandType.Text;
                command.CommandText = Sql;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    retVal = reader[0].ToString();
                }
            }
            return retVal;
        }

        internal static void Insert(string Sql)
        {

            using (OracleCommand command = new OracleCommand(Sql))
            {
                command.Connection = OracleDbService.conn;
                command.ExecuteNonQuery();
            }
        }

        internal static void Insert(List<string> Sqls)
        {
            int c = Sqls.Count();
            for (int i = 1; i <= c; ++i)
            {
                using (OracleCommand command = new OracleCommand(Sqls[i - 1]))
                {
                    command.Connection = OracleDbService.conn;
                    command.ExecuteNonQuery();
                }
            }

        }
    }
}
