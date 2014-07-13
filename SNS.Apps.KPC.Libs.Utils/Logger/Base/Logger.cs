using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SNS.Apps.KPC.Libs.Utils.Logger.Base
{
	public interface ILogger
	{
		#region "Properties"
		string Name { get; set; } 
		#endregion

		#region "Methods"
		void Log(LogLevel level, string msg);

		void LogFormat(LogLevel level, string format, params object[] args); 
		#endregion
	}

	public enum LogLevel
	{
		Debug,
		Info,
		Warn,
		Error,
		Fatal
	}

	public abstract class Logger<T> : ILogger
		where T : ILogger
	{
		#region "Fields"
		static Lazy<T> _instance = new Lazy<T>(() => 
		{
			return Activator.CreateInstance<T>();
		});
		
		log4net.ILog _logger = default(log4net.ILog);
		#endregion

		#region "Fields"
		protected const string CNSTR_USERIDENTITY_PATTERN = @"^(.+)\s+\((\d+)\,\s+([0-9a-f]{8}\-(?:[0-9a-f]{4}\-){3}[0-9a-f]{12})\)*$";
		protected const string CNSTR_PROPERTYNAME_BROWSER = "browser";
		protected const string CNSTR_PROPERTYNAME_HOSTNAME = "hostname";
		protected const string CNSTR_PROPERTYNAME_URL = "url";
		protected const string CNSTR_PROPERTYNAME_USERAGENT = "useragent";
		protected const string CNSTR_PROPERTYNAME_USERID = "userid";
		protected const string CNSTR_PROPERTYNAME_USERNAME = "username";
		#endregion

		#region "Constructs"
		public Logger(string name)
		{
			_logger = InitLogger(name);

			this.Name = name;
		} 
		#endregion

		#region "Properties"
		public static T Instance { get { return _instance.Value; } }

		public virtual string Name { get; set; }
		#endregion

		#region "Public Methods"
		public void Debug(string msg)
		{
			Log(LogLevel.Debug, msg);
		}

		public void Debug<T>(T msg)
			where T : Base.LogMessage
		{
			Log(LogLevel.Debug, msg);
		}

		public void DebugFormat(string format, params object[] args)
		{
			LogFormat(LogLevel.Debug, format, args);
		}

		public void Warn(string msg)
		{
			Log(LogLevel.Warn, msg);
		}

		public void Warn<T>(T msg)
			where T : Base.LogMessage
		{
			Log(LogLevel.Warn, msg);
		}

		public void WarnFormat(string format, params object[] args)
		{
			LogFormat(LogLevel.Warn, format, args);
		}

		public void Error(string msg)
		{
			Log(LogLevel.Error, msg);
		}

		public void Error<T>(T msg)
			where T : Base.LogMessage
		{
			Log(LogLevel.Error, msg);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			LogFormat(LogLevel.Error, format, args);
		}

		public void Info(string msg)
		{
			Log(LogLevel.Info, msg);
		}

		public void Info<T>(T msg)
			where T : Base.LogMessage
		{
			Log(LogLevel.Info, msg);
		}

		public void InfoFormat(string format, params object[] args)
		{
			LogFormat(LogLevel.Info, format, args);
		}

		public void Fatal(string msg)
		{
			Log(LogLevel.Fatal, msg);
		}

		public void Fatal<T>(T msg)
			where T : Base.LogMessage
		{
			Log(LogLevel.Fatal, msg);
		}

		public void FatalFormat(string format, params object[] args)
		{
			LogFormat(LogLevel.Fatal, format, args);
		} 
		
		public void Log(LogLevel level, string msg)
		{
			SetExtendProperties();

			RenderLogData(level, msg);
		}

		public void Log<T>(LogLevel level, T msg)
			where T : Base.LogMessage
		{
			SetExtendProperties(msg);

			RenderLogData(level, msg.Message);
		}

		public void LogFormat(LogLevel level, string format, params object[] args)
		{
			Log(level, string.Format(format, args));
		} 
		#endregion

		#region "Private Methods"
		log4net.ILog InitLogger(string name)
		{
			var instance = default(log4net.ILog);

			if (!string.IsNullOrEmpty(name))
			{
				instance = log4net.LogManager.GetLogger(name);
			}
			else
			{
				instance = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			}

			return instance;
		}

		void RenderLogData(LogLevel level, string msg)
		{
			switch (level)
			{
				case LogLevel.Debug:
					_logger.Debug(msg);
					break;
				case LogLevel.Error:
					_logger.Error(msg);
					break;
				case LogLevel.Fatal:
					_logger.Fatal(msg);
					break;
				case LogLevel.Info:
					_logger.Info(msg);
					break;
				case LogLevel.Warn:
					_logger.Warn(msg);
					break;
			}
		}

		void SetExtendProperties()
		{
			if (Configurations.ConfigStore.LoggingSettings.EnableExtend_App)
			{
				if (HttpContext.Current != null && HttpContext.Current.Request != null)
				{
					log4net.ThreadContext.Properties["Browser"] = HttpContext.Current.Request.Browser.Browser;
					log4net.ThreadContext.Properties["Url"] = HttpContext.Current.Request.RawUrl;
					log4net.ThreadContext.Properties["UserAgent"] = HttpContext.Current.Request.UserAgent;
					log4net.ThreadContext.Properties["HostName"] = HttpContext.Current.Request.UserHostName;

					if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
					{
						var match = Regex.Match(HttpContext.Current.User.Identity.Name, CNSTR_USERIDENTITY_PATTERN);

						if (match.Success)
						{
							log4net.ThreadContext.Properties["UserID"] = match.Groups[2].Value;
							log4net.ThreadContext.Properties["UserName"] = match.Groups[1].Value;
						}
					}
				}
			}
			else if (Configurations.ConfigStore.LoggingSettings.EnableExtend_WX)
			{

			}
		}

		void SetExtendProperties<T>(T msg)
			where T : Base.LogMessage
		{
			if (msg is Base.HttpLogMessage)
			{
				log4net.ThreadContext.Properties["Browser"] = (msg as Base.HttpLogMessage).Browser;
				log4net.ThreadContext.Properties["Url"] = (msg as Base.HttpLogMessage).Url;
				log4net.ThreadContext.Properties["UserAgent"] = (msg as Base.HttpLogMessage).UserAgent;
				log4net.ThreadContext.Properties["HostName"] = (msg as Base.HttpLogMessage).HostName;
				log4net.ThreadContext.Properties["UserID"] = (msg as Base.HttpLogMessage).UserID;
				log4net.ThreadContext.Properties["UserName"] = (msg as Base.HttpLogMessage).UserName;
			}
			else if (msg is Base.WXLogMessage)
			{
				log4net.ThreadContext.Properties["UserID"] = (msg as Base.WXLogMessage).UserID;
			}
		}
		#endregion
	}
}
