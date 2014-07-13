using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public sealed class UserInsuranceRepository : Base.RepositoryBase<UserInsuranceRepository>
	{
		#region "Fields"
		static object _lock_Running = new object();
		static volatile bool _isRunning = false;
		#endregion

		#region "Constructs"
		private UserInsuranceRepository() { }
		#endregion

		#region "Methods: Get"
		public UserInsurance Get(string folio, bool inForce = false)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Insurance.SingleOrDefault(p => string.Compare(p.Folio, folio, StringComparison.InvariantCultureIgnoreCase) == 0);

				return (entity != null) ? (new UserInsurance(entity) { Requestor = new UserPrivy(UserRepository.Instance.Get(entity.RequestorID)) }) : (null);
			}
		}

		public UserInsurance Get(long id, bool inForce = false)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Insurance.SingleOrDefault(p => p.ID == id);

				return (entity != null) ? (new UserInsurance(entity) { Requestor = new UserPrivy(UserRepository.Instance.Get(entity.RequestorID)) }) : (null);
			}
		}

		public UserInsurance Get(long userID, long orderID, bool inForce = false)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Order.SingleOrDefault(p => p.ID == orderID && (p.RequestorID == userID || p.SupplierID == userID));

				if (entity != null)
				{
					var entity_insurance = default(DataStores.tbl_User_Insurance); 
					var entity_order_insurance = ctx.tbl_User_Order_Insurance.SingleOrDefault(p => p.UserID == userID && p.OrderID == orderID);

					if (entity_order_insurance != null)
					{
						entity_insurance = ctx.tbl_User_Insurance.SingleOrDefault(p => p.ID == entity_order_insurance.InsuranceID);
					}

					if (entity_insurance == null)
					{
						entity_insurance = ctx.tbl_User_Insurance.SingleOrDefault(p => p.RequestorID == userID && (p.Status != null && p.Status.HasValue && p.Status.Value == (int)InsuranceStatus.Active));
					}

					return (entity_insurance != null) ? (new UserInsurance(entity_insurance) { Requestor = new UserPrivy(UserRepository.Instance.Get(entity.RequestorID)) }) : (null);
				}

				return null;
			}
		}
		#endregion

		#region "Methods: GetLatest"
		public UserInsurance GetLatest(long userID, bool includeInActive = false, bool inForce = false)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Insurance.Where(p => p.RequestorID == userID && (includeInActive || !includeInActive && p.Status != null && p.Status.HasValue && (p.Status.Value == (int)InsuranceStatus.Active || p.Status.Value == (int)InsuranceStatus.Pending))).OrderByDescending(p => p.ID).FirstOrDefault();

				return (entity != null) ? (new UserInsurance(entity) { Requestor = new UserPrivy(UserRepository.Instance.Get(userID)) }) : (null);
			}
		}
		#endregion

		#region "Methods: Save"
		public string Save(InsuranceSubmitRequest<InsuranceSubmitRequestExtend_Metlife> requestInfo)
		{
			#region "Create New Request"
			using (var ctx = new DataStores.KPCDataModels())
			{
				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					var entity = ctx.tbl_User_Insurance.Create();

					#region "Set Fields"
					entity.RequestorID = requestInfo.RequestorID;
					entity.ICID = (int)requestInfo.RequestExtend.InsuranceCompany;
					entity.Folio = ctx.sp_Get_UserInsuranceFolio().First();

					entity.InsureKey = requestInfo.RequestExtend.InsureKey;
					entity.InsureNo = requestInfo.RequestExtend.InsureNo;

					entity.Province = requestInfo.Request.Province;
					entity.City = requestInfo.Request.City;
					entity.District = requestInfo.Request.District;
					entity.Location = requestInfo.Request.Location;

					entity.OccupationCode = requestInfo.RequestExtend.OccupationCode;
					entity.PresentCode = requestInfo.RequestExtend.PresentCode;
					entity.TSRCode = requestInfo.RequestExtend.TSRCode;

					entity.IssueDate = requestInfo.RequestExtend.IssueDate;
					entity.EffectDate = requestInfo.RequestExtend.EffectDate;
					entity.ExpireDate = requestInfo.RequestExtend.ExpireDate;
					entity.Status = (requestInfo.RequestExtend.Status != null && requestInfo.RequestExtend.Status.HasValue) ? ((int)requestInfo.RequestExtend.Status.Value) : ((int)InsuranceStatus.Fail);
					
					entity.CreateDate = entity.UpdateDate = DateTime.Now;
					#endregion

					ctx.tbl_User_Insurance.Add(entity);

					#region "Table: tbl_User & tbl_User_External"
					var entity_user = ctx.tbl_User.SingleOrDefault(p => p.ID == requestInfo.RequestorID);

					if (entity_user != null)
					{
						entity_user.Name = requestInfo.Request.Name;
						entity_user.IdentityNo = requestInfo.Request.IdentityNo;
						entity_user.Birthday = requestInfo.Request.Birthday;
						entity_user.Gender = requestInfo.Request.Gender;
					}
					else if (ConfigStore.CommonSettings.ExternalData_Enabled)
					{
						var entity_user_external = ctx.tbl_User_External.SingleOrDefault(p => p.ID == requestInfo.RequestorID);

						if (entity_user_external != null)
						{
							entity_user_external.Name = requestInfo.Request.Name;
							entity_user_external.IdentityNo = requestInfo.Request.IdentityNo;
							entity_user_external.Birthday = requestInfo.Request.Birthday;
							entity_user_external.Gender = requestInfo.Request.Gender;
						}
					}
					#endregion

					ctx.SaveChanges();
					ctx_transaction.Commit();

					return entity.Folio;
				}
			}
			#endregion
		}
		#endregion

		#region "Tasks: Maintain"
		public void Maintain()
		{
			if (!_isRunning)
			{
				_isRunning = true;

				try
				{
					// 设置过期的保险
					Maintain_ExpireInsuranceRecords();

					// 激活生效的保险
					Maintain_ActiveInsuranceRecords();
				}
				finally
				{
					_isRunning = false;
				}
			}
		}

		void Maintain_ExpireInsuranceRecords()
		{
			try
			{
				var lstIDs = new List<string>();

				using (var ctx = new DataStores.KPCDataModels())
				{
					var results = ctx.tbl_User_Insurance.Where(p => p.Status != null && p.Status.HasValue && p.Status.Value == (int)InsuranceStatus.Active && p.ExpireDate.Value.CompareTo(DateTime.Now) < 0);
					
					if (results != null)
					{
						#region "Set Status"
						foreach (var result in results)
						{
							result.Status = (int)InsuranceStatus.InActive;

							lstIDs.Add(result.Folio);
						}

						ctx.SaveChanges();
						#endregion
					}
				}

				if (lstIDs.Count > 0)
				{
					var sb = new StringBuilder();

					lstIDs.ForEach(p => { sb.AppendFormat("{0}, ", p); });

					if (ConfigStore.CommonSettings.Trace_Mode)
					{
						DBLogger.Instance.InfoFormat("-- Task: UserInsuranceActiveTask - Success to expire {0} Insurance Record(s): [{1}]", lstIDs.Count, sb.ToString().TrimEnd(' ', ','));
					}
				}
			}
			catch(Exception ex)
			{
				DBLogger.Instance.ErrorFormat("-- Task: UserInsuranceActiveTask - Fail to expire Insurance Record\r\nException: {0}", ex.ToString());
			}
		}

		void Maintain_ActiveInsuranceRecords()
		{
			try
			{
				var lstIDs = new List<string>();

				using (var ctx = new DataStores.KPCDataModels())
				{
					var results = ctx.tbl_User_Insurance.Where(p => p.Status != null && p.Status.HasValue && p.Status.Value == (int)InsuranceStatus.Pending && p.EffectDate.Value.CompareTo(DateTime.Now) < 0);

					if (results != null)
					{
						#region "Set Status"
						foreach (var result in results)
						{
							result.Status = (int)InsuranceStatus.Active;

							lstIDs.Add(result.Folio);
						}

						ctx.SaveChanges();
						#endregion
					}
				}

				if (lstIDs.Count > 0)
				{
					var sb = new StringBuilder();

					lstIDs.ForEach(p => { sb.AppendFormat("{0}, ", p); });

					if (ConfigStore.CommonSettings.Trace_Mode)
					{
						DBLogger.Instance.InfoFormat("-- Task: UserInsuranceActiveTask - Success to active {0} Insurance Record(s): [{1}]", lstIDs.Count, sb.ToString().TrimEnd(' ', ','));
					}
				}
			}
			catch (Exception ex)
			{
				DBLogger.Instance.ErrorFormat("-- Task: UserInsuranceActiveTask - Fail to active Insurance Record\r\nException: {0}", ex.ToString());
			}
		}
		#endregion
	}
}