using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SNS.Apps.KPC.Libs.Models.SMS
{
	[DataContract]
	public class SMSMessageReceiveResult
	{
		#region "Constructs"
		public SMSMessageReceiveResult()
		{ }

		public SMSMessageReceiveResult(string content)
		{
			try
			{
				var xmlDoc = new XmlDocument();

				xmlDoc.LoadXml(content);

				var nodeCount = xmlDoc.SelectSingleNode("//GetSmsNum");

				if (nodeCount != null)
				{
					this.Count = int.Parse(nodeCount.InnerText);
				}

				if (this.Count != 0)
				{
					var lstItems = new List<SMSMessageReceiveResultItem>();
					var nodeSMSs = xmlDoc.SelectNodes("//UpSms");

					foreach (XmlNode node in nodeSMSs)
					{
						lstItems.Add(new SMSMessageReceiveResultItem(node));
					}

					this.Items = lstItems.ToArray();
				}

				this.Success = true;
			}
			catch (Exception ex)
			{
				this.Count = 0;
				this.Success = false;
				this.ErrorMsg = ex.Message;
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		IEnumerable<SMSMessageReceiveResultItem> Items { get; set; }

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public bool Success { get; set; }

		[DataMember]
		public string ErrorMsg { get; set; }
		#endregion
	}

	[DataContract]
	public class SMSMessageReceiveResultItem
	{
		#region "Constructs"
		public SMSMessageReceiveResultItem() { }

		public SMSMessageReceiveResultItem(XmlNode xmlNode)
		{
			foreach (XmlNode node in xmlNode.ChildNodes)
			{
				if (node.Name == "MobileNum")
				{
					this.Mobile = node.InnerText;
				}
				else if (node.Name == "UpSmsDate")
				{
					this.SendDate = DateTime.Parse(node.InnerText);
				}
				//else if (node.Name == "DestTermID")
				//{

				//}
				else if (node.Name == "MsgContent")
				{
					this.Content = node.InnerText;
				}
			}
		}
		#endregion

		#region "Properties"
		[DataMember]
		public string Mobile { get; set; }

		[DataMember]
		public DateTime? SendDate { get; set; }

		[DataMember]
		public string Content { get; set; }
		#endregion
	}
}
