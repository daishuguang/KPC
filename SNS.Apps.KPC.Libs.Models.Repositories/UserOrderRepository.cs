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
	public sealed class UserOrderRepository : Base.RepositoryBase<UserOrderRepository>
	{
		#region "Constructs"
		private UserOrderRepository() { this.RepositoryKeys = new string[] { CNSTR_MEMCACHEKEY_USERORDER_ID, CNSTR_MEMCACHEKEY_USERORDER_FOLIO }; }
		#endregion

		#region "Public Methods"

		#region "Methods: Get"
		public UserOrder Get(long id, bool inForce = false)
		{
			var instance = default(UserOrder);

			if (inForce)
			{
				instance = Get_FromDB(id);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Get_FromDB(id);
				},
				CNSTR_MEMCACHEKEY_USERORDER_ID, id);
			}

			return instance;
		}

		public UserOrder Get(string folio, bool inForce = false)
		{
			var instance = default(UserOrder);

			if (inForce)
			{
				instance = Get_FromDB(folio);

				RefreshMem(instance);
			}
			else
			{
				instance = GetMem(() =>
				{
					return Get_FromDB(folio);
				},
				CNSTR_MEMCACHEKEY_USERORDER_FOLIO, folio);
			}

			return instance;
		}

		public IList<UserOrder> Get(UserOrderRequest requestInfo)
		{
			var user = (requestInfo.Filter.UserID.HasValue ? UserRepository.Instance.Get(requestInfo.Filter.UserID.Value) : null);
			var route = (requestInfo.Filter.RouteID.HasValue ? RouteRepository.Instance.Get(requestInfo.Filter.RouteID.Value) : null);

			if (user == null && route == null)
			{
				return new UserOrder[] { };
			}

			var orderType = requestInfo.Filter.OrderType.HasValue ? requestInfo.Filter.OrderType.Value : OrderType.All;
			var orderStatus = requestInfo.Filter.OrderStatus.HasValue ? requestInfo.Filter.OrderStatus.Value : OrderStatus.All;
			var page = requestInfo.Page.HasValue ? requestInfo.Page.Value : 0;
			var count = requestInfo.Count.HasValue ? requestInfo.Count.Value : 10;

			return Get_FromDB(user, route, orderType, orderStatus, page, count);
		}

		UserOrder Get_FromDB(long id)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var result = ctx.tbl_User_Order.FirstOrDefault(p => p.ID == id);

				if (result != null)
				{
					return new UserOrder
					{
						ID = result.ID,
						Folio = result.Folio,

						Requestor = UserRepository.Instance.Get(result.RequestorID),
						RequestorRole = (UserRole)Enum.Parse(typeof(UserRole), result.RequestorRole.ToString()),

						Supplier = UserRepository.Instance.Get(result.SupplierID),
						SupplierRole = (UserRole)Enum.Parse(typeof(UserRole), result.SupplierRole.ToString()),
						Route = RouteRepository.Instance.Get(result.RouteID),

						StartDate = result.StartDate,
						Charge = result.Charge,
						Note = result.Note,
						Status = result.Status.HasValue ? (OrderStatus)Enum.Parse(typeof(OrderStatus), result.Status.Value.ToString()) : OrderStatus.PendingForPayment,

						IsCancelled_Requestor = result.IsCancelled_Requestor,
						IsCancelled_Supplier = result.IsCancelled_Supplier,
						IsConfirmed_Requestor = result.IsConfirmed_Requestor,
						IsConfirmed_Supplier = result.IsConfirmed_Supplier,

						CreateDate = result.CreateDate,
						UpdateDate = result.UpdateDate
					};
				}

				return null;
			}
		}

		UserOrder Get_FromDB(string folio)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var result = ctx.tbl_User_Order.FirstOrDefault(p => string.Compare(p.Folio, folio, StringComparison.InvariantCultureIgnoreCase) == 0);

				if (result != null)
				{
					return new UserOrder
					{
						ID = result.ID,
						Folio = result.Folio,

						Requestor = UserRepository.Instance.Get(result.RequestorID),
						RequestorRole = (UserRole)Enum.Parse(typeof(UserRole), result.RequestorRole.ToString()),

						Supplier = UserRepository.Instance.Get(result.SupplierID),
						SupplierRole = (UserRole)Enum.Parse(typeof(UserRole), result.SupplierRole.ToString()),
						Route = RouteRepository.Instance.Get(result.RouteID),

						StartDate = result.StartDate,
						Charge = result.Charge,
						Note = result.Note,
						Status = result.Status.HasValue ? (OrderStatus)Enum.Parse(typeof(OrderStatus), result.Status.Value.ToString()) : OrderStatus.PendingForPayment,

						IsCancelled_Requestor = result.IsCancelled_Requestor,
						IsCancelled_Supplier = result.IsCancelled_Supplier,
						IsConfirmed_Requestor = result.IsConfirmed_Requestor,
						IsConfirmed_Supplier = result.IsConfirmed_Supplier,

						CreateDate = result.CreateDate,
						UpdateDate = result.UpdateDate
					};
				}

				return null;
			}
		}

		IList<UserOrder> Get_FromDB(User userInstance, Route routeInstance, OrderType orderType, OrderStatus orderStatus, int page = 0, int count = 10)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var lst = new List<UserOrder>();
				var results = default(IQueryable<DataStores.tbl_User_Order>);

				switch (orderType)
				{
					case OrderType.RequestByMe:
						if (routeInstance != null)
						{
							results = ctx.tbl_User_Order.Where(p => p.RequestorID == userInstance.ID && p.RouteID == routeInstance.ID && (orderStatus == OrderStatus.All || ((!p.Status.HasValue) && orderStatus == OrderStatus.PendingForPayment) || (p.Status.HasValue && p.Status.Value == (int)orderStatus))).OrderByDescending(p => p.CreateDate).Skip(page * count).Take(count);
						}
						else
						{
							results = ctx.tbl_User_Order.Where(p => p.RequestorID == userInstance.ID && (orderStatus == OrderStatus.All || ((!p.Status.HasValue) && orderStatus == OrderStatus.PendingForPayment) || (p.Status.HasValue && p.Status.Value == (int)orderStatus))).OrderByDescending(p => p.CreateDate).Skip(page * count).Take(count);
						}
						break;
					case OrderType.AssignToMe:
						if (routeInstance != null)
						{
							results = ctx.tbl_User_Order.Where(p => p.SupplierID == userInstance.ID && p.RouteID == routeInstance.ID && (orderStatus == OrderStatus.All || ((!p.Status.HasValue) && orderStatus == OrderStatus.PendingForPayment) || (p.Status.HasValue && p.Status.Value == (int)orderStatus))).OrderByDescending(p => p.CreateDate).Skip(page * count).Take(count);
						}
						else
						{
							results = ctx.tbl_User_Order.Where(p => p.SupplierID == userInstance.ID && (orderStatus == OrderStatus.All || ((!p.Status.HasValue) && orderStatus == OrderStatus.PendingForPayment) || (p.Status.HasValue && p.Status.Value == (int)orderStatus))).OrderByDescending(p => p.CreateDate).Skip(page * count).Take(count);
						}
						break;
					default:
						if (routeInstance != null)
						{
							results = ctx.tbl_User_Order.Where(p => (p.RequestorID == userInstance.ID || p.SupplierID == userInstance.ID) && p.RouteID == routeInstance.ID && (orderStatus == OrderStatus.All || ((!p.Status.HasValue) && orderStatus == OrderStatus.PendingForPayment) || (p.Status.HasValue && p.Status.Value == (int)orderStatus))).OrderByDescending(p => p.CreateDate).Skip(page * count).Take(count);
						}
						else
						{
							results = ctx.tbl_User_Order.Where(p => (p.RequestorID == userInstance.ID || p.SupplierID == userInstance.ID) && (orderStatus == OrderStatus.All || ((!p.Status.HasValue) && orderStatus == OrderStatus.PendingForPayment) || (p.Status.HasValue && p.Status.Value == (int)orderStatus))).OrderByDescending(p => p.CreateDate).Skip(page * count).Take(count);
						}
						break;
				}

				if (results != null)
				{
					foreach (var result in results)
					{
						var ur = new UserOrder
						{
							ID = result.ID,
							Folio = result.Folio,

							Requestor = UserRepository.Instance.Get(result.RequestorID),
							RequestorRole = (UserRole)Enum.Parse(typeof(UserRole), result.RequestorRole.ToString()),

							Supplier = UserRepository.Instance.Get(result.SupplierID),
							SupplierRole = (UserRole)Enum.Parse(typeof(UserRole), result.SupplierRole.ToString()),
							Route = RouteRepository.Instance.Get(result.RouteID),

							StartDate = result.StartDate,
							Charge = result.Charge,
							Note = result.Note,
							Status = result.Status.HasValue ? (OrderStatus)Enum.Parse(typeof(OrderStatus), result.Status.Value.ToString()) : OrderStatus.PendingForPayment,

							IsCancelled_Requestor = result.IsCancelled_Requestor,
							IsCancelled_Supplier = result.IsCancelled_Supplier,
							IsConfirmed_Requestor = result.IsConfirmed_Requestor,
							IsConfirmed_Supplier = result.IsConfirmed_Supplier,

							CreateDate = result.CreateDate,
							UpdateDate = result.UpdateDate
						};

						lst.Add(ur);
					}
				}

				return lst;
			}
		}
		#endregion

		#region "Methods: Create"
		public string Create(UserOrderCreateRequest requestInfo)
		{
			var requestor = UserRepository.Instance.Get(requestInfo.RequestorID);
			var supplier = UserRepository.Instance.Get(requestInfo.SupplierID);
			var supplier_route = RouteRepository.Instance.Get(requestInfo.RouteID);

			using (var ctx = new DataStores.KPCDataModels())
			{
				using (var ctx_transaction = ctx.Database.BeginTransaction())
				{
					var entity = ctx.tbl_User_Order.Create();

					entity.Folio = ctx.sp_Get_UserOrderFolio().First();
					entity.RequestorID = requestor.ID;
					entity.RequestorRole = (int)requestInfo.RequestorRole;
					entity.SupplierID = supplier.ID;
					entity.SupplierRole = (int)requestInfo.SupplierRole;
					entity.RouteID = supplier_route.ID;

					// 订单信息
					entity.StartDate = requestInfo.StartDate;
					entity.Charge = requestInfo.Charge;
					entity.Note = requestInfo.Note;
					entity.Status = (int)OrderStatus.PendingForPayment;
					entity.CreateDate = entity.UpdateDate = DateTime.Now;
					ctx.tbl_User_Order.Add(entity);

					// 用户信息
					if (requestor.UserRole != requestInfo.RequestorRole)
					{
						var entity_User = ctx.tbl_User.FirstOrDefault(p => p.ID == requestor.ID);

						if (entity_User != null)
						{
							entity_User.UserRole = (int)requestInfo.RequestorRole;
						}
					}
					/* End */

					ctx.SaveChanges();
					ctx_transaction.Commit();

					#region "Set to MemCache"
					var uo = new UserOrder
					{
						Folio = entity.Folio,

						Requestor = UserRepository.Instance.Get(entity.RequestorID, true),
						RequestorRole = (UserRole)Enum.Parse(typeof(UserRole), entity.RequestorRole.ToString()),

						Supplier = UserRepository.Instance.Get(entity.SupplierID),
						Route = RouteRepository.Instance.Get(entity.RouteID),

						StartDate = entity.StartDate,
						Charge = entity.Charge,
						Note = entity.Note,
						Status = entity.Status.HasValue ? (OrderStatus)Enum.Parse(typeof(OrderStatus), entity.Status.Value.ToString()) : OrderStatus.PendingForPayment,

						CreateDate = entity.CreateDate,
						UpdateDate = entity.UpdateDate
					};

					// 添加当前项到 MemCache
					RefreshMem(uo);
					#endregion

					return entity.Folio;
				}
			}
		}
		#endregion

		#region "Methods: Update"
		public bool Update(UserOrderUpdateRequest requestInfo)
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				var entity = ctx.tbl_User_Order.FirstOrDefault(p => string.Compare(p.Folio, requestInfo.Folio, StringComparison.InvariantCultureIgnoreCase) == 0 && (!p.Status.HasValue || p.Status.Value == (int)OrderStatus.PendingForPayment));

				if (entity != null)
				{
					if (requestInfo.StartDate != null && requestInfo.StartDate.HasValue)
					{
						entity.StartDate = requestInfo.StartDate;
					}

					entity.Charge = requestInfo.Charge;
					entity.Note = requestInfo.Note;
					entity.UpdateDate = DateTime.Now;

					ctx.SaveChanges();

					return true;
				}
			}

			return false;
		} 
		#endregion

		#region "Methods: Cancel"
		public bool Cance(Guid userID, string folio)
		{
			var orderID = 0L;
			var orderUserID = 0L;

			if (!ValidateUserOrder(folio, userID, out orderID, out orderUserID))
			{
				return false;
			}

			using (var ctx = new DataStores.KPCDataModels())
			{
				#region "Update to DB"
				var entity = ctx.tbl_User_Order.FirstOrDefault(p => p.ID == orderID);

				if (!entity.Status.HasValue || entity.Status.Value == (int)OrderStatus.PendingForPayment)
				{
					entity.Status = (int)OrderStatus.Cancelled;
					entity.UpdateDate = DateTime.Now;

					if (orderUserID == entity.RequestorID)
					{
						entity.IsCancelled_Requestor = true;
					}
					else
					{
						entity.IsCancelled_Supplier = true;
					}

					ctx.SaveChanges();

					// Set to MemCache
					RefreshMem(new UserOrder(entity));

					return true;
				}
				#endregion

				return false;
			}
		}
		#endregion

		#region "Methods: Confirm"
		public bool Confirm(Guid userID, string folio)
		{
			var orderID = 0L;
			var orderUserID = 0L;

			if (!ValidateUserOrder(folio, userID, out orderID, out orderUserID))
			{
				return false;
			}

			using (var ctx = new DataStores.KPCDataModels())
			{
				#region "Update to DB"
				var entity = ctx.tbl_User_Order.FirstOrDefault(p => p.ID == orderID);
				var isRequestor = (entity.RequestorID == orderUserID);

				if (entity.Status.HasValue && entity.Status.Value > (int)OrderStatus.PendingForPayment && entity.Status.Value < (int)OrderStatus.Completed)
				{
					if (isRequestor)
					{
						entity.IsConfirmed_Requestor = true;
					}
					else
					{
						entity.IsConfirmed_Supplier = true;
					}

					if (entity.IsConfirmed_Requestor.HasValue && entity.IsConfirmed_Requestor.Value && entity.IsConfirmed_Supplier.HasValue && entity.IsConfirmed_Supplier.Value)
					{
						entity.Status = (int)OrderStatus.Completed;
					}

					entity.UpdateDate = DateTime.Now;

					ctx.SaveChanges();

					// Set to MemCache
					RefreshMem(new UserOrder(entity));

					return true;
				}
				#endregion

				return false;
			}
		} 
		#endregion

		#endregion

		#region "Tasks: Sync_Status"
		public void Sync_Status()
		{
			using (var ctx = new DataStores.KPCDataModels())
			{
				ctx.sp_Sync_UserOrder_Status();
			}
		} 
		#endregion

        #region "Private Methods"
        bool ValidateUserOrder(string folio, Guid userID, out long orderID, out long orderUserID)
        {
            var uo = Get(folio);

            if (uo == null)
            {
                throw new Exception(string.Format("拼单号‘{0}’不存在！", folio));
            }

            var user = UserRepository.Instance.Get(userID);

            if (user == null)
            {
                throw new Exception(string.Format("用户‘{0}’不存在！", userID));
            }

            // 非订单干系人
            if (uo.Requestor.ID != user.ID && uo.Supplier.ID != user.ID)
            {
                throw new Exception(string.Format("用户‘{0}’无权处理拼单号‘{1}’！", userID, folio));
            }

            orderID = uo.ID;
            orderUserID = user.ID;

            return true;
        } 
        #endregion
	}
}