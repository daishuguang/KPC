using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Models;
using SNS.Apps.KPC.Libs.Utils;

namespace SNS.Apps.KPC.Libs.Models.Repositories
{
	public class UserPaymentRepository : Base.RepositoryBase<UserPaymentRepository>
	{
		#region "Constructs"
		private UserPaymentRepository() { }
		#endregion

		#region "Methods: Execute"
		public bool Execute(PaymentSubmitRequest requestInfo)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					var entity = ctx.tbl_User_Payment.SingleOrDefault(p => p.OrderID == requestInfo.ID);
					var isnew = (entity == null);

					if (isnew)
					{
						if (requestInfo.PayMethod != (int)PaymentMethod.DirectPay)
						{
							entity = ctx.tbl_User_Payment.Create();
							entity.CreateDate = entity.UpdateDate = DateTime.Now;
						}
						else
						{
							return true;
						}
					}
					else
					{
						entity.UpdateDate = DateTime.Now;
					}

					entity.OrderID = requestInfo.ID;
					entity.PayAmount = requestInfo.PayAmount;
					entity.PayMethod = (int)requestInfo.PayMethod;
					entity.PayStatus = (int)requestInfo.PayStatus;

					if (isnew)
					{
						ctx.tbl_User_Payment.Add(entity);
					}

					ctx.SaveChanges();
					ctx_transaction.Commit();

					return true;
				}
			}
		} 
		#endregion

		#region "Methods: Finish"
		public bool Finish(long orderID, string payFolio)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					var entity = ctx.tbl_User_Payment.SingleOrDefault(p => p.OrderID == orderID && (!p.PayStatus.HasValue || p.PayStatus.Value == (int)PaymentStatus.Pending));

					if (entity != null)
					{
						entity.PayFolio = payFolio;
						entity.PayStatus = (int)PaymentStatus.Success;

						var entity_order = ctx.tbl_User_Order.SingleOrDefault(p => p.ID == orderID && (!p.Status.HasValue || p.Status.Value == (int)OrderStatus.PendingForPayment));

						if (entity_order != null)
						{
							var payMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), entity.PayMethod.Value.ToString());
							
							if (payMethod == PaymentMethod.DirectPay)
							{
								entity_order.Status = (int)OrderStatus.PendingForPayment;
							}
							else if (((int)payMethod & (int)PaymentMethod.OnlinPay_Deposit) != 0)
							{
								entity_order.Status = (int)OrderStatus.FinishToPayment_Deposit;
							}
							else if (((int)payMethod & (int)PaymentMethod.OnlinPay_FullCash) != 0)
							{
								entity_order.Status = (int)OrderStatus.FinishToPayment_FullCash;
							}

							ctx.SaveChanges();
							ctx_transaction.Commit();

							return true;
						}
					}

					return false;
				}
			}
		} 

		public bool Validate(long orderID, string payFolio)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Payment.SingleOrDefault(p => p.OrderID == orderID && string.Compare(p.PayFolio, payFolio) == 0 && p.PayStatus.Value == (int)PaymentStatus.Success);

				return (entity != null);
			}
		}
		#endregion

		#region "Methods: Get"
		public PaymentSubmitResult Get(long orderID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Payment.SingleOrDefault(p => p.OrderID == orderID);

				if (entity != null)
				{
					return new PaymentSubmitResult(entity);
				}

				return null;
			}
		}

		public PaymentSubmitResult Get(string orderID)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity_order = ctx.tbl_User_Order.SingleOrDefault(p => string.Compare(p.Folio, orderID) == 0);

				if (entity_order != null)
				{
					var entity = ctx.tbl_User_Payment.SingleOrDefault(p => p.OrderID == entity_order.ID);

					if (entity != null)
					{
						return new PaymentSubmitResult(entity);
					}
				}

				return null;
			}
		} 
		#endregion
	}
}
