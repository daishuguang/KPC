using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SNS.Apps.KPC.Libs.Configurations;
using SNS.Apps.KPC.Libs.Tasks;
using SNS.Apps.KPC.Libs.Utils;
using SNS.Apps.KPC.Libs.Utils.Logger;

namespace SNS.Apps.TaskService
{
	class TaskServiceManager
	{
		#region "Fields"
		static Lazy<IEnumerable<ITask>> _tasks = new Lazy<IEnumerable<ITask>>(() =>
		{
			var lst = new List<ITask>();
			var tasks = ConfigurationManager.GetSection("taskServiceSettings") as TaskServiceConfiguration;

			if (tasks != null)
			{
				foreach (TaskServiceElement task in tasks.Tasks)
				{
					var ctors = Type.GetType(task.Type).GetConstructors();
					var instance = (ctors != null && ctors.Count() > 0) ? ((ITask)ctors[0].Invoke(new object[] { task.Name, task.IsEnabled, task.ExecuteDate, task.ExecuteTime })) : (null);

					if (instance != null)
					{
						lst.Add(instance);
					}
				}
			}

			return lst.ToArray();
		});
		static Timer _timer = null;
		#endregion

		#region "Constructs"
		/// <summary>
		/// 
		/// </summary>
		/// <param name="interval"></param>
		private TaskServiceManager() { }
		#endregion

		#region "Properties"
		public static bool Started { get; private set; }

		public static bool IsRunning { get; private set; }

		public static int Interval 
		{ 
			get
			{
				return (int)(ConfigStore.TaskServiceSettings.TaskService_Interval * 60000); 
			} 
		}

		public static IEnumerable<ITask> Tasks { get { return _tasks.Value; } }
		#endregion

		#region "Public Methods"
		/// <summary>
		/// 
		/// </summary>
		public static void Start()
		{
			if (Started)
			{
				return;
			}

			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}

			_timer = new Timer(new TimerCallback(Timer_OnTick), null, 1000 * 2, Interval);

			Started = true;
		}

		/// <summary>
		/// 
		/// </summary>
		public static void Stop()
		{
			if (!Started)
			{
				return;
			}

			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}

			Started = false;
		}
		#endregion

		#region "Events"
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		static void Timer_OnTick(object state)
		{
			if (IsRunning)
			{
				return;
			}

			IsRunning = true;

			foreach (var taskItem in Tasks)
			{
				try
				{
					if (taskItem.IsEnabled && taskItem.IsMatchCondition)
					{
						var t = DateTime.Now.Ticks;
						DBLogger.Instance.InfoFormat("Task '{0}' is ready to execute...", taskItem.Name);

						taskItem.Execute();

						DBLogger.Instance.InfoFormat("Task '{0}' is completed in {1} seconds", taskItem.Name, TimeSpan.FromTicks(DateTime.Now.Ticks - t).TotalSeconds);
					}
				}
				catch (Exception ex)
				{
					DBLogger.Instance.InfoFormat("Task '{0}' occur exception: {1}", taskItem.Name, ex.ToString());
				}
			}

			IsRunning = false;
		}
		#endregion
	}
}
