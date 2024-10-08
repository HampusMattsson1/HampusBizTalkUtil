﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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

		public string RawValue;

		private bool CustomProps = false;
		private string SpecialCase = "";

		public BindingValue() { }

		public BindingValue(XmlDocument doc, string Name, string Xpath, bool CustomProps = false, string SpecialCase = "")
		{
			this.Name = Name;
			this.Xpath = Xpath;

			if (CustomProps)
			{
				this.CustomProps = CustomProps;
				this.SpecialCase = SpecialCase;

				var tempdoc = new XmlDocument();
				tempdoc.LoadXml(doc.SelectSingleNode(Xpath).InnerText);

				if (string.IsNullOrEmpty(SpecialCase))
				{
					this.Value = tempdoc.SelectSingleNode($"CustomProps/{Name}").InnerText;
					this.RawValue = tempdoc.SelectSingleNode($"CustomProps/{Name}").InnerXml;
				}
				else
				{
					if (SpecialCase == "ReceiveWCF-SQL")
					{
						var customsProps = tempdoc.SelectSingleNode($"CustomProps/BindingConfiguration").InnerText;
						tempdoc.LoadXml(customsProps);
						var bindingConfiguration = tempdoc.SelectSingleNode("binding");
						if (bindingConfiguration.Attributes[Name] != null)
						{
                            this.Value = bindingConfiguration.Attributes[Name].InnerText;
                            this.RawValue = SecurityElement.Escape(SecurityElement.Escape(bindingConfiguration.Attributes[Name].InnerXml));
                        }
					}
				}
			}
			else
			{
				this.Value = doc.SelectSingleNode(Xpath).InnerText;
			}

			// Andra speciella grejer
			if (Name == "Description")
			{
				if (doc.SelectSingleNode(Xpath).Attributes["xsi:nil"] != null)
				{
                    doc.SelectSingleNode(Xpath).Attributes.RemoveNamedItem("xsi:nil");
                }
            }
		}

		public void UpdateXml(XmlDocument doc)
		{
			XmlNode node = doc.SelectSingleNode(Xpath);
			if (node == null)
			{
				return;
			}

			// Kom ihåg att kolla specialfall som ReceiveWCF-Custom

			// 1 level nesting
			if (CustomProps)
			{
				var tempdoc = new XmlDocument();
				tempdoc.LoadXml(node.InnerText);

				if (string.IsNullOrEmpty(SpecialCase))
				{
					tempdoc.SelectSingleNode($"CustomProps/{Name}").InnerText = Value;
					RawValue = tempdoc.SelectSingleNode($"CustomProps/{Name}").InnerXml;
				}
				else
				{
					if (SpecialCase == "ReceiveWCF-SQL")
					{
						var bindingConfiguration = tempdoc.SelectSingleNode($"CustomProps/BindingConfiguration").InnerText;

						var tempdocSpecial = new XmlDocument();
						tempdocSpecial.LoadXml(bindingConfiguration);

						tempdocSpecial.SelectSingleNode("binding").Attributes[Name].InnerText = Value;
						RawValue = SecurityElement.Escape(SecurityElement.Escape(tempdocSpecial.SelectSingleNode("binding").Attributes[Name].InnerXml));

						tempdoc.SelectSingleNode($"CustomProps/BindingConfiguration").InnerText = tempdocSpecial.OuterXml;
					}
				}

				node.InnerText = tempdoc.OuterXml;
			}
			else
			{
				node.InnerText = Value;
				RawValue = node.InnerXml;
			}
		}
	}
}
