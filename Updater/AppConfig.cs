namespace Updater
{
    public static class AppConfig
    {
        public static string OracleLocalUser { get; set; } = $@"abt";
        public static string OracleLocalPassword { get; set; } = $@"tujybrnjytpyftn";

        public static string OracleLocalSID { get; set; } = $@"xe";
        public static string OracleLocalPort { get; set; } = $@"1521";
        public static string OracleLocalHost { get; set; } = $@"localhost";


        public static string OracleRemoteUser { get; set; } = $@"abt";
        public static string OracleRemotePassword { get; set; } = $@"tujybrnjytpyftn";

        public static string OracleRemoteSID { get; set; } = $@"xe";
        public static string OracleRemotePort { get; set; } = $@"1521";
        public static string OracleRemoteHost { get; set; } = $@"192.168.100.3";

        public static string OracleRemoteLinkName { get; set; } = $@"ORCL.UPDATE.WERA";
        public static string OracleRemoteLinkTNS => $"(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {OracleRemoteHost})(PORT = {OracleRemotePort}))(CONNECT_DATA = "
                        + $"(SERVER = DEDICATED)(SERVICE_NAME = {OracleRemoteSID})))";
        
    }
}
