using HampusBizTalkUtil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HampusBizTalkUtil.Data
{
    public class Tokenization
    {
        public Dictionary<string, List<BindingValue>> GetBasicBindingValues(string xml)
        {
			var result = new Dictionary<string, List<BindingValue>>();

			var doc = new XmlDocument();
			try
			{
				doc.LoadXml(xml);
			} catch
			{
				return result;
			}
			//doc.LoadXml(System.IO.File.ReadAllText(@"C:\Temp\IP0002.BindingInfo.xml"));


			var tempdoc = new XmlDocument();

			string xpath = "/BindingInfo/SendPortCollection/SendPort";
			XmlNodeList sendPorts = doc.SelectNodes(xpath);

			int index = 1;
			foreach (XmlNode sendPort in sendPorts)
			{
				string sendPortXpath = xpath + $"[{index}]";

				string type = sendPort.SelectSingleNode("PrimaryTransport/SendHandler/TransportType").Attributes["Name"].InnerText;
				string name = "SendPort - " + sendPort.Attributes["Name"].InnerText + " (" + type + ")";

				result.Add(name, new List<BindingValue>
				{
					{ new BindingValue(doc, "Description", sendPortXpath + "/Description") }
				});

				if (doc.SelectSingleNode(sendPortXpath + "/PrimaryTransport/Address") != null)
				{
					result[name].Add(new BindingValue(doc, "Address", sendPortXpath + "/PrimaryTransport/Address"));
				}

				switch (type)
				{
					case "FILE":
						result[name].AddRange(
							new BindingValue[]
							{
								new BindingValue (doc, "FileName", sendPortXpath + "/PrimaryTransport/TransportTypeData", true)
							});
						break;
					case "SFTP":
						result[name].AddRange(
							new BindingValue[]
							{
								new BindingValue (doc, "UserName", sendPortXpath + "/PrimaryTransport/TransportTypeData", true),
								new BindingValue (doc, "TargetFileName", sendPortXpath + "/PrimaryTransport/TransportTypeData", true),
								new BindingValue (doc, "FolderPath", sendPortXpath + "/PrimaryTransport/TransportTypeData", true),
								new BindingValue (doc, "ServerAddress", sendPortXpath + "/PrimaryTransport/TransportTypeData", true)
							});
						break;
				}

				index++;
			}


			xpath = "/BindingInfo/ReceivePortCollection/ReceivePort";
			XmlNodeList receivePorts = doc.SelectNodes(xpath);

			int receivePortIndex = 1;
			foreach (XmlNode receivePort in receivePorts)
			{
				XmlNodeList receiveLocations = receivePort.SelectNodes("ReceiveLocations/ReceiveLocation");

				int receiveLocationIndex = 1;
				foreach (XmlNode receiveLocation in receiveLocations)
				{
					string receiveLocationXpath = $"{xpath}[{receivePortIndex}]/ReceiveLocations/ReceiveLocation[{receiveLocationIndex}]";

					string type = receiveLocation.SelectSingleNode("ReceiveHandler/TransportType").Attributes["Name"].InnerText;
					string name = "ReceiveLocation - " + receiveLocation.Attributes["Name"].InnerText + " (" + type + ")";

					result.Add(name, new List<BindingValue>
					{
						{ new BindingValue(doc, "Description", receiveLocationXpath + "/Description") }
					});

					if (doc.SelectSingleNode(receiveLocationXpath + "/Address") != null)
					{
						result[name].Add(new BindingValue(doc, "Address", receiveLocationXpath + "/Address"));
					}

					switch (type)
					{
						case "FILE":
							result[name].AddRange(
							new BindingValue[]
							{
								new BindingValue (doc, "FileMask", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true)
							});
							break;
						case "SFTP":
							result[name].AddRange(
							new BindingValue[]
							{
								new BindingValue (doc, "FileMask", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true),
								new BindingValue (doc, "UserName", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true),
								new BindingValue (doc, "ServerAddress", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true),
								new BindingValue (doc, "FolderPath", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true)
							});
							break;
						case "WCF-SQL":
							result[name].AddRange(
							new BindingValue[]
							{
								new BindingValue (doc, "polledDataAvailableStatement", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true, "ReceiveWCF-SQL"),
								new BindingValue (doc, "pollingStatement", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true, "ReceiveWCF-SQL"),
								new BindingValue (doc, "pollingIntervalInSeconds", receiveLocationXpath + "/ReceiveLocationTransportTypeData", true, "ReceiveWCF-SQL")
							});

							break;
					}

					receiveLocationIndex++;
				}

				receivePortIndex++;
			}

			return result;
		}
    }
}
