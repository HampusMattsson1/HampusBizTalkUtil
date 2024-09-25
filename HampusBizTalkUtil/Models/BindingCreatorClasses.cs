using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HampusBizTalkUtil.Models
{
	public class Binding
	{
		public string Xml;
		public XmlDocument Document = new();

		public List<SendPort> SendPorts = new();
		public List<ReceivePort> ReceivePorts = new();

		public void AddSendPort()
		{
			SendPorts.Add(new SendPort());
		}
		public void AddReceivePort()
		{
			ReceivePorts.Add(new ReceivePort());
		}
	}

	public class SendPort
	{
		public string Name = "Dummysend";
		public string Adapter = "FILE";
	}

	public class ReceivePort
	{
		public string Name = "Dummyreceive";
		public List<ReceiveLocation> ReceiveLocations = new List<ReceiveLocation>();

		public void AddReceiveLocation()
		{
			ReceiveLocations.Add(new ReceiveLocation());
		}
	};

	public class ReceiveLocation
	{
		public string Name = "Dummylocation";
		public string Adapter = "FILE";
	}
}
