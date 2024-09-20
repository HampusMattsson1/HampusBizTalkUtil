using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HampusBizTalkUtil
{
	public static class Config
	{
		public static string ConfigPath = "C:\\HampusBizTalkUtil";
        public static string BizTalkEnvironment = "BIZDEV-WS11";

        public static string BizTalkApplicationBindingsPath = Path.Combine(ConfigPath, "\\BizTalkApplications");

        public static string ConnectionString = $"Data Source={BizTalkEnvironment};Initial Catalog=BizTalkMgmtDb;Integrated Security=True;TrustServerCertificate=True";
        public static string ConnectionStringOm = $"SERVER={BizTalkEnvironment};DATABASE=BizTalkMgmtDb;Integrated Security=SSPI";

		public static string ScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "Scripts").ToString();
	}
}
