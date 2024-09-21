using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HampusBizTalkUtil.Models
{
	public class BindingValue
	{
		public string Xpath;
		public string Name;
		public string Value;

		private bool NestedXML = false;

		public BindingValue() { }

		public BindingValue(XmlDocument doc, string Name, string Xpath, bool NestedXML = false, string specialCase = "")
		{
			this.Name = Name;
			this.Xpath = Xpath;

			if (NestedXML)
			{
				this.NestedXML = NestedXML;
				//this.Value = doc.SelectSingleNode(Xpath).InnerText;

				var tempdoc = new XmlDocument();
				tempdoc.LoadXml(doc.SelectSingleNode(Xpath).InnerText);

				if (string.IsNullOrEmpty(specialCase))
				{
					this.Value = tempdoc.SelectSingleNode($"CustomProps/{Name}").InnerText;
				}
				else
				{
					if (specialCase == "ReceiveWCF-SQL")
					{
						var customsProps = tempdoc.SelectSingleNode($"CustomProps/BindingConfiguration").InnerText;
						tempdoc.LoadXml(customsProps);
						var bindingConfiguration = tempdoc.SelectSingleNode("binding");
						this.Value = bindingConfiguration.Attributes[Name].InnerText;
					}
				}
			}
			else
			{
				this.Value = doc.SelectSingleNode(Xpath).InnerText;
			}
		}

		public void UpdateXml(XmlDocument doc)
		{
			// Kom ihåg att kolla specialfall som ReceiveWCF-Custom
		}
	}
}
