using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Security.Cryptography;
using System.Security.Policy;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.IServices;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Models.Repositories;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Services
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
	public class InsuranceService : IInsuranceService
	{
		#region "Methods: Create"
		public InsuranceSubmitResult Create(InsuranceSubmitRequest requestInfo)
		{
			try
			{
				var uiInstance = UserInsuranceRepository.Instance.GetLatest(requestInfo.RequestorID);

				if (uiInstance != null)
				{
					return new InsuranceSubmitResult { Status = RequestStatus.OK, Data = uiInstance.Folio };
				}

				var extendRequestInfo = new InsuranceSubmitRequest<InsuranceSubmitRequestExtend_Metlife> { RequestorID = requestInfo.RequestorID, Request = requestInfo.Request };

				#region "Set Extend Fields"
				var ts = DateTime.Now.Ticks.ToString();

				extendRequestInfo.RequestExtend.InsureKey = string.Format("{0}{1:yyyyMMdd}{2}", ConfigStore.InsuranceSettings.Insurance_Settings_SystemName, DateTime.Now, (ts.Length > 20 ? ts.Substring(ts.Length - 20) : ts.PadLeft(20, '0')));
				extendRequestInfo.RequestExtend.SystemName = ConfigStore.InsuranceSettings.Insurance_Settings_SystemName;
				extendRequestInfo.RequestExtend.OccupationCode = ConfigStore.InsuranceSettings.Insurance_Settings_OccupationCode;
				extendRequestInfo.RequestExtend.PresentCode = ConfigStore.InsuranceSettings.Insurance_Settings_PresentCode;
				extendRequestInfo.RequestExtend.TSRCode = ConfigStore.InsuranceSettings.Insurance_Settings_TSRCode;
				extendRequestInfo.RequestExtend.IssueDate = DateTime.Now;
				extendRequestInfo.RequestExtend.EffectDate = DateTime.Now.AddDays(1).Date;
				extendRequestInfo.RequestExtend.ExpireDate = DateTime.Now.AddDays(31).Date; 
				#endregion

				// Call Service from Metlife
				using (var svcRef = new InsuranceService_Metlife_Ref.YSW2ICareSaveClient("insuranceService_Metlife"))
				{
					#region "Call Service"
					var insureNo = default(string);
					var errMsg = default(string);
					var xml = BuildXMLInput(extendRequestInfo);

					xml = svcRef.doRequest(xml);

					if (ParseXMLOutput(xml, out insureNo, out errMsg))
					{
						extendRequestInfo.RequestExtend.InsureNo = insureNo;
						extendRequestInfo.RequestExtend.IssueMemo = null;
						extendRequestInfo.RequestExtend.Status = InsuranceStatus.Pending;
					}
					else
					{
						extendRequestInfo.RequestExtend.InsureNo = null;
						extendRequestInfo.RequestExtend.IssueMemo = errMsg;
						extendRequestInfo.RequestExtend.Status = InsuranceStatus.InActive;
					}
					#endregion
				}

				// 保存到数据库
				return new InsuranceSubmitResult
				{
					Status = (extendRequestInfo.RequestExtend.Status != null && (extendRequestInfo.RequestExtend.Status.Value == InsuranceStatus.Active || extendRequestInfo.RequestExtend.Status.Value == InsuranceStatus.Pending)) ? (RequestStatus.OK) : (RequestStatus.Error),
					Data = UserInsuranceRepository.Instance.Save(extendRequestInfo),
					Message = (extendRequestInfo.RequestExtend.Status != null && extendRequestInfo.RequestExtend.Status.Value == InsuranceStatus.Fail) ? (extendRequestInfo.RequestExtend.IssueMemo) : (null)
				};
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat(
					"Service: [{0}], Method: [{1}]\r\nParameters: [{2}]\r\nException: {3}",
					"InsuranceService",
					"InsuranceSubmitResult Create(InsuranceSubmitRequest requestInfo)",
					string.Format("RequestInfo: {0}", requestInfo),
					ex.ToString()
				);
			}

			return null;
		} 
		#endregion

		#region "Private Methods"
		string BuildXMLInput(InsuranceSubmitRequest<InsuranceSubmitRequestExtend_Metlife> requestInfo)
		{
			using (var mem = new System.IO.MemoryStream())
			{
				var settings = new XmlWriterSettings() { Indent = false, NewLineOnAttributes = false, NewLineChars = string.Empty, Encoding = System.Text.Encoding.GetEncoding("utf-8") };
				var w = XmlWriter.Create(mem, settings);

				#region "Write XML"
				w.WriteStartDocument();

				#region "<Records />"
				w.WriteStartElement("Records");

				#region "--<Record />"
				w.WriteStartElement("Record");

				#region "----<Customer />"
				w.WriteStartElement("Customer");

				#region "------<Key />"
				w.WriteStartElement("Key");
				w.WriteString(requestInfo.RequestExtend.InsureKey);
				w.WriteEndElement();
				#endregion

				#region "------<FromSystem />"
				w.WriteStartElement("FromSystem");
				w.WriteString(requestInfo.RequestExtend.SystemName);
				w.WriteEndElement();
				#endregion

				#region "------<Name />"
				w.WriteStartElement("Name");
				w.WriteString(requestInfo.Request.Name);
				w.WriteEndElement();
				#endregion

				#region "------<Sex />"
				w.WriteStartElement("Sex");
				w.WriteString(requestInfo.Request.Gender.Value ? "Male" : "Female");
				w.WriteEndElement();
				#endregion

				#region "------<Birthday />"
				w.WriteStartElement("Birthday");
				w.WriteString(requestInfo.Request.Birthday.Value.ToString("yyyy-MM-dd"));
				w.WriteEndElement();
				#endregion

				#region "------<Document />"
				w.WriteStartElement("Document");
				w.WriteFullEndElement();
				#endregion

				#region "------<DocumentType />"
				w.WriteStartElement("DocumentType");
				w.WriteString("IdentityCard");
				w.WriteEndElement();
				#endregion

				#region "------<Email />"
				w.WriteStartElement("Email");
				w.WriteFullEndElement();
				#endregion

				#region "------<Mobile />"
				w.WriteStartElement("Mobile");
				w.WriteString(requestInfo.Request.Mobile);
				w.WriteEndElement();
				#endregion

				#region "------<ContactState />"
				w.WriteStartElement("ContactState");
				w.WriteStartElement("Name");
				w.WriteString(requestInfo.Request.City);
				w.WriteEndElement();
				w.WriteEndElement();
				#endregion

				#region "------<ContactCity />"
				w.WriteStartElement("ContactCity");
				w.WriteStartElement("Name");
				w.WriteString(requestInfo.Request.District);
				w.WriteEndElement();
				w.WriteEndElement();
				#endregion

				#region "------<ContactAddress />"
				w.WriteStartElement("ContactAddress");
				w.WriteString(requestInfo.Request.Location);
				w.WriteEndElement();
				#endregion

				#region "------<Occupation />"
				w.WriteStartElement("Occupation");
				w.WriteStartElement("Code");
				w.WriteString(requestInfo.RequestExtend.OccupationCode);
				w.WriteEndElement();
				w.WriteEndElement();
				#endregion

				#region "------<Description />"
				w.WriteStartElement("Description");
				w.WriteEndElement();
				#endregion

				w.WriteEndElement();
				#endregion

				#region "----<Task />"
				w.WriteStartElement("Task");

				#region "------<CallList />"
				w.WriteStartElement("CallList");
				w.WriteStartElement("Name");
				w.WriteFullEndElement();
				w.WriteFullEndElement();
				#endregion

				#region "------<Campaign />"
				w.WriteStartElement("Campaign");
				w.WriteStartElement("Name");
				w.WriteFullEndElement();
				w.WriteFullEndElement();
				#endregion

				w.WriteEndElement();
				#endregion

				#region "----<Activity />"
				w.WriteStartElement("Activity");

				#region "------<Code />"
				w.WriteStartElement("Code");
				w.WriteFullEndElement();
				#endregion

				#region "------<Present />"
				w.WriteStartElement("Present");
				w.WriteStartElement("Code");
				w.WriteString(requestInfo.RequestExtend.PresentCode);
				w.WriteEndElement();
				w.WriteEndElement();
				#endregion

				#region "------<TSR />"
				w.WriteStartElement("TSR");
				w.WriteStartElement("Code");
				w.WriteString(requestInfo.RequestExtend.TSRCode);
				w.WriteEndElement();
				w.WriteEndElement();
				#endregion

				#region "------<DonateTime />"
				w.WriteStartElement("DonateTime");
				w.WriteString(requestInfo.RequestExtend.IssueDate.Value.ToString("yyyy-MM-dd"));
				w.WriteEndElement();
				#endregion

				#region "------<SMS />"
				w.WriteStartElement("SMS");
				w.WriteString(ConfigStore.InsuranceSettings.Insurance_Settings_SMSEnabled ? "1" : "0");
				w.WriteEndElement();
				#endregion

				#region "------<FlghtNo />"
				w.WriteStartElement("FlghtNo");
				w.WriteEndElement();
				#endregion

				#region "------<ValidTime />"
				w.WriteStartElement("ValidTime");
				w.WriteEndElement();
				#endregion

				w.WriteEndElement();
				#endregion

				w.WriteEndElement();
				#endregion

				w.WriteEndElement();
				#endregion

				w.WriteEndDocument();
				w.Flush();
				w.Close();
				#endregion

				mem.Seek(0, System.IO.SeekOrigin.Begin);

				return (new System.IO.StreamReader(mem)).ReadToEnd();
			}
		}

		bool ParseXMLOutput(string xml, out string insureNo, out string errMsg)
		{
			insureNo = null;
			errMsg = null;

			if (string.IsNullOrEmpty(xml))
			{
				return false;
			}

			var xmlDoc = new XmlDocument();

			xmlDoc.LoadXml(xml);

			var xmlFlag = xmlDoc.SelectSingleNode("//Flag");
			var bFlag = (string.Compare(xmlFlag.ChildNodes[0].Value.Trim(), "true", true) == 0);

			if (bFlag)
			{
				insureNo = xmlDoc.SelectSingleNode("//FreeInsureNo").ChildNodes[0].Value.Trim();
				errMsg = null;
			}
			else
			{
				insureNo = null;

				var xmlMsg = xmlDoc.SelectSingleNode("//Message");

				if (xmlMsg != null && xmlMsg.ChildNodes.Count > 0)
				{
					errMsg = xmlMsg.ChildNodes[0].Value;
				}
			}

			return bFlag;
		} 
		#endregion
	}
}