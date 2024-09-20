using Microsoft.Data.SqlClient;
using System.Diagnostics;
//using Microsoft.BizTalk.ExplorerOM;

namespace HampusBizTalkUtil.Data
{
    public class BiztalkActions
    {
		public string[] GetBiztalkApplications()
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);

            SqlCommand sqlCommand = new SqlCommand("SELECT nvcName FROM [BizTalkMgmtDb].[dbo].[bts_application]", connection);
            connection.Open();

			var sqlResult = sqlCommand.ExecuteReader();

			var result = new List<string>();

			while (sqlResult.Read())
            {
                result.Add(sqlResult["nvcName"].ToString());
            }
            sqlResult.Close();
			connection.Close();

            return result.ToArray();
        }

		private void StartProcess(string cmd, string args)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = cmd,
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			Process process = new Process { StartInfo = startInfo };
			process.Start();

			//string output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
		}

        public void ExportBindings(string applicationName, string suffix = "")
        {
			var destination = $"{Config.BizTalkApplicationBindingsPath}\\{applicationName}";
			Directory.CreateDirectory(destination);

			string args = $"ExportBindings /Destination:\"{destination}\\{applicationName}{suffix}.xml\" /ApplicationName:\"{applicationName}\"";

			StartProcess("BTSTask", args);
		}

		public void ImportBindings(string applicationName, string suffix = "")
		{
			var destination = $"{Config.BizTalkApplicationBindingsPath}\\{applicationName}";
			Directory.CreateDirectory(destination);

			string args = $"ImportBindings /Source:\"{destination}\\{applicationName}{suffix}.xml\" /ApplicationName:\"{applicationName}\"";

			StartProcess("BTSTask",args);
		}

		public void StartApplication(string applicationName)
		{
			var args = @$"
				
				[void] [System.reflection.Assembly]::LoadWithPartialName('Microsoft.BizTalk.ExplorerOM')

				$Catalog = New-Object Microsoft.BizTalk.ExplorerOM.BtsCatalogExplorer
				$Catalog.ConnectionString = " + $"\'{Config.ConnectionStringOm}\'" + @"

				$application = $Catalog.Applications[" + $"\'{applicationName}\'" + @"]

				$application.Start('StartAll')
				$Catalog.SaveChanges()
			";

			StartProcess("powershell", args);
		}

		public void StopApplication(string applicationName)
		{
			var args = @$"
				
				[void] [System.reflection.Assembly]::LoadWithPartialName('Microsoft.BizTalk.ExplorerOM')

				$Catalog = New-Object Microsoft.BizTalk.ExplorerOM.BtsCatalogExplorer
				$Catalog.ConnectionString = " + $"\'{Config.ConnectionStringOm}\'" + @"

				$application = $Catalog.Applications[" + $"\'{applicationName}\'" + @"]

				$application.Stop('StopAll')
				$Catalog.SaveChanges()
			";

			StartProcess("powershell", args);
		}

		public void RestartHostInstances()
		{
			var scriptPath = Path.Combine(Config.ScriptPath, "RestartHostInstances.ps1");

			var args = $"-File \"{scriptPath}\"";

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "powershell",
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			Process process = new Process { StartInfo = startInfo };
			process.Start();

			string output = process.StandardOutput.ReadToEnd();
			Console.WriteLine(output);
			process.WaitForExit();
		}

		public string CheckNaming(string applicationName)
		{
			var scriptPath = Path.Combine(Config.ScriptPath, "CheckNaming.ps1");

			var args = "-File " + $"\"{scriptPath}\" \"{Config.BizTalkEnvironment}\" \"{applicationName}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "powershell",
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			Process process = new Process { StartInfo = startInfo };
			process.Start();

			string output = "";

			while(!process.StandardOutput.EndOfStream)
			{
				output += process.StandardOutput.ReadLine() + "\r\n";
			}

			process.WaitForExit();

			return output;
		}

		public List<string> GetBiztalkDependencies()
		{
			var scriptPath = Path.Combine(Config.ScriptPath, "GetDependencies.ps1");

			var args = "-File " + $"\"{scriptPath}\" \"{Config.ConnectionString}\"";

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "powershell",
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			Process process = new Process { StartInfo = startInfo };
			process.Start();

			var result = new List<string>();

			while (!process.StandardOutput.EndOfStream)
			{
				result.Add(process.StandardOutput.ReadLine());
			}

			process.WaitForExit();

			return result;
		}

		public List<string> GetBiztalkApplicationsByDependency(string dependency)
		{
			var scriptPath = Path.Combine(Config.ScriptPath, "GetApplicationsByDependency.ps1");

			var args = "-File " + $"\"{scriptPath}\" \"{Config.ConnectionString}\" \"{dependency}\" \"{Config.ScriptPath}\"";

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "powershell",
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			Process process = new Process { StartInfo = startInfo };
			process.Start();

			var result = new List<string>();

			while (!process.StandardOutput.EndOfStream)
			{
				result.Add(process.StandardOutput.ReadLine());
			}

			process.WaitForExit();

			return result;
		}

		public List<string> GetSavedBindingsForApplication(string applicationName)
		{
			var folder = $"{Config.BizTalkApplicationBindingsPath}\\{applicationName}";

			var files = Directory.GetFiles(folder);

			return new List<string>() { "works" };
		}
	}
}
