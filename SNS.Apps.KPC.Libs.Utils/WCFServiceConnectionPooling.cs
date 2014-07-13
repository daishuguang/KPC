using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Utils
{
	public class WCFServiceConnectionPooling<IContract>
	{
		#region "Fields"
		//ConnectionContext class will hold the connection and perform all operations on it.
		//only one instance of the object will exist.Hence sort of singleton behavior implemented
		private static ConnectionContext<IContract> _connection;
		private static string _configurationName = null;

		//shared object for thread locking
		private static object _locker = new object();
		#endregion

		#region "Constructs"
		private WCFServiceConnectionPooling()
		{
			_connection = null;
		}
		#endregion

		#region "Methods"
		// get connection to wcf service
		public static IContract GetConnection()
		{
			//lock for each call till connection status is not know properly
			lock (_locker)
			{
				//if connection is expired
				if (IsCurrentConnectionExpired())
				{
					//create new connection
					ReCreateConnection();
				}
			}

			//return the connection
			return _connection.GetConnection();
		}

		//Recreate connection if one don't exist or is expired
		private static void ReCreateConnection()
		{
			// replace current connection by new one.
			_connection = new ConnectionContext<IContract>();
			_connection.CreateConnection();
		}

		//Check if current connection expired
		private static bool IsCurrentConnectionExpired()
		{
			//connection is expired if it is null or
			// has exceeded maximum allowed duration by ChannelContext
			if (_connection == null)
			{
				return true;
			}

			return _connection.IsExpired();
		}
		#endregion

		#region "Methods"
		public static void Initial(string configurationName)
		{
			_configurationName = configurationName;
		}
		#endregion

		private class ConnectionContext<T>
		{
			#region "Fields"
			//Connection info like connection object, id, stopwatch for that conneciton and connection state property
			T _connection = default(T);

			volatile bool _connectionExistToWCF = default(bool);
			volatile System.Diagnostics.Stopwatch _connectionTimeWatcher;
			volatile int _connectedUserCount; // number of users working on this connection

			static readonly int connectionExpireTime = 1800000; // milliseconds 
			#endregion

			#region "Constructs"
			// initialise connection details , but not the connection
			public ConnectionContext()
			{
				// initiliase connection but do not create wcf connection till get connection is called.
				_connectionTimeWatcher = new System.Diagnostics.Stopwatch();
				_connectionExistToWCF = false;
				_connectedUserCount = 0;
			}
			#endregion

			#region "Methods"
			//Get connection object
			public T GetConnection()
			{
				if (!_connectionExistToWCF)
				{
					CreateConnection();
				}

				_connectedUserCount++;

				return _connection;
			}

			//Create new connection
			public void CreateConnection()
			{
				if (!_connectionExistToWCF)
				{
					_connectionExistToWCF = true;

					// create the single instance of the type T using reflection
					//_connection = (T)Activator.CreateInstance(typeof(T), true);

					////get property connection State
					//var connectionType = _connection.GetType();
					//_connectionStateProperty = connectionType.GetProperty("State");

					_connection = WCFServiceClientUtility.CreateServiceChanel<T>(_configurationName);

					//start activity timer for that connection
					_connectionTimeWatcher.Start();
				}
			}

			//check if connection has expired
			public bool IsExpired()
			{
				if (_connectionTimeWatcher != null)
				{
					//if connection has been in memory for more than desired time, declare it as expired
					if (_connectionTimeWatcher.ElapsedMilliseconds >= connectionExpireTime)
					{
						_connectionTimeWatcher.Reset();

						//create thread to dispose this connection after it's use is done.
						//var t = new Thread(new ThreadStart(DisposeConnection));

						////t.IsBackground = true;
						//t.Priority = ThreadPriority.Normal;
						//t.Start();

						ThreadPool.QueueUserWorkItem(new WaitCallback(DisposeConnection), null);

						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return true;
				}
			}

			//Dispose connection after it's use
			private void DisposeConnection(object state)
			{
				//dispose the connection
				if (_connection != null && _connectionExistToWCF)
				{
					//var method = _connection.GetType().GetMethod("Close");
					//var connectionState = (CommunicationState)_connectionStateProperty.GetValue(_connection, null);

					////if connection is still opened, close it
					//if (connectionState == CommunicationState.Opened)
					//{
					//	method.Invoke(_connection, null);
					//}

					WCFServiceClientUtility.CloseServiceChannel<T>(_connection);

					_connectionExistToWCF = false;
					//_connectionStateProperty = null;
					_connectionTimeWatcher = null;
				}
			}
			#endregion
		}
	}
}
