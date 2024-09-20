using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HampusBizTalkUtil.Data
{
	public class BindingActions
	{
		public string GenerateHtmlFromXML(string path)
		{
			string result = "";

			var xmlString = File.ReadAllText(path);

			var doc = XDocument.Parse(xmlString);

			var sb = new StringBuilder();
			sb.AppendLine("<ul id=\"myUL\">");

			foreach (var element in doc.Root.Elements())
			{
				GenerateHtml(element, sb, 0);
			}

			sb.AppendLine("</ul>");

			return sb.ToString();

			//var result = new List<List<(string, string)>>();

			//var bindingXmlString = await File.ReadAllTextAsync(path);

			//var xml = new XmlDocument();

			//xml.LoadXml(bindingXmlString);

			//foreach (XmlNode xmlNode in xml.ChildNodes)
			//{
			//	Console.WriteLine(xmlNode.Name);
			//}


			return result;
		}

		private void GenerateHtml(XElement element, StringBuilder sb, int level)
		{
			bool nested = element.HasElements;

			var indent = new string(' ', level * 2);

			sb.AppendLine($"{indent}<li><span class=\"caret\">{element.Name.LocalName}</span>");

			if (nested)
			{
				sb.AppendLine($"{indent}<ul class=\"nested\">");
				foreach (var childElement in element.Elements())
				{
					GenerateHtml(childElement, sb, level + 1);
				}
				sb.AppendLine($"{indent}</ul>");
			}
			else
			{
				sb.AppendLine($"{indent}{element.Value}");
			}

			sb.AppendLine($"{indent}</li>");
		}
	}
}
