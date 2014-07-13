using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.KPC.Libs.Utils
{
	public static class CommonUtility
	{
		#region "Reflection"
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static void CopyTo<T1, T2>(T1 source, T2 target)
		{
			if (source != null && target != null)
			{
				PropertyInfo[] arrSourceProps = typeof(T1).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance);
				PropertyInfo[] arrTargetProps = typeof(T2).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance);

				foreach (var t in arrTargetProps)
				{
					if (t.CanWrite)
					{
						var s = arrSourceProps.FirstOrDefault(item => string.Compare(item.Name, t.Name) == 0);

						if (s != null && s.CanRead)
						{
							if (s.PropertyType.Equals(t.PropertyType))
							{
								try
								{
									t.SetValue(target, s.GetValue(source, null));
								}
								catch { }
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="entity"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static R GetPropertyValue<T, R>(T entity, string name)
		{
			PropertyInfo p = entity.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);

			if (p != null && p.CanRead)
			{
				return (R)p.GetValue(entity, null);
			}

			return default(R);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="S"></typeparam>
		/// <param name="entity"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void SetPropertyValue<T, S>(T entity, string name, S value)
		{
			PropertyInfo p = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.GetProperty);

			if (p != null && p.CanWrite)
			{
				p.SetValue(entity, value, null);
			}
		}
		#endregion

		#region "Verify & Generate SignCode"
		public static string GenerateSignCode(Guid userGUID, Guid? routeGUID)
		{
			if (routeGUID.HasValue)
			{
				routeGUID = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
			}

			var match1 = userGUID.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
			var match2 = routeGUID.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

			if (match1 != null && match2 != null && match1.Length == match2.Length)
			{
				var values = new Int64[5];

				for (var i = 0; i < match1.Length; i++)
				{
					values[i] = Int64.Parse(match1[i], System.Globalization.NumberStyles.HexNumber) + Int64.Parse(match2[i], System.Globalization.NumberStyles.HexNumber);
				}

				var instants = new string[5];

				for (var i = 0; i < values.Length; i++)
				{
					var s = string.Format("{0:x}", values[i]);

					s = s.Substring(s.Length - ((i == 0) ? (8) : ((i == 4) ? (12) : (4))));

					instants[i] = s;
				}

				return GetMd5Str32(string.Format("{0}-{1}-{2}-{3}-{4}", instants[0], instants[1], instants[2], instants[3], instants[4]));
			}

			return null;
		}

		public static bool VerifySignCode(Guid userGUID, Guid routeGUID, string signCode)
		{
			return (string.Compare(GenerateSignCode(userGUID, routeGUID), signCode, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public static bool VerifySignParameter(string id)
		{
			const string CNSTR_GUID_Pattern = @"^[0-9a-f]{8}\-(?:[0-9a-f]{4}\-){3}[0-9a-f]{12}$";

			return ((!string.IsNullOrEmpty(id)) && System.Text.RegularExpressions.Regex.IsMatch(id, CNSTR_GUID_Pattern));
		}
		#endregion

		#region “Serialize & Deserialize”
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string Serialize<T>(T obj)
		{
			if (obj == null)
			{
				return null;
			}

			using (MemoryStream ms = new MemoryStream())
			{
				(new BinaryFormatter()).Serialize(ms, obj);

				ms.Seek(0, SeekOrigin.Begin);

				BinaryReader br = new BinaryReader(ms);
				byte[] data = br.ReadBytes((int)ms.Length);

				return Convert.ToBase64String(data, Base64FormattingOptions.None);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="str"></param>
		/// <returns></returns>
		public static T Deserialize<T>(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return default(T);
			}

			using (MemoryStream ms = new MemoryStream())
			{
				byte[] data = Convert.FromBase64String(str);
				BinaryWriter bw = new BinaryWriter(ms);

				bw.Write(data, 0, data.Length);

				var obj = (new BinaryFormatter()).Deserialize(ms);

				if (obj != null)
				{
					return (T)obj;
				}
				return default(T);
			}
		}
		#endregion

		#region "Common Methods"
		public static string FormatDistance(double? distance)
		{
			if (!distance.HasValue)
			{
				return string.Empty;
			}

			if (distance.Value < 1)
			{
				return string.Format("{0}m", Math.Floor(distance.Value * 1000));
			}
			else
			{
				return string.Format("{0}km", Math.Round(distance.Value, 1));
			}
		}

		public static string GetMd5Str32(string str)
		{
			MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

			// Convert the input string to a byte array and compute the hash.  
			char[] temp = str.ToCharArray();
			byte[] buf = new byte[temp.Length];

			for (int i = 0; i < temp.Length; i++)
			{
				buf[i] = (byte)temp[i];
			}

			byte[] data = md5Hasher.ComputeHash(buf);

			// Create a new Stringbuilder to collect the bytes  
			// and create a string.  
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data   
			// and format each one as a hexadecimal string.  
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.  
			return sBuilder.ToString();
		}

		public static string GetFixedNumber(int num, int length)
		{
			return string.Format("{0:D" + length + "}", num);
		}

		public static string GetFixedRandNumber(int length)
		{
			var min = 1;
			var max = Convert.ToInt32(new string('9', length > 6 ? 6 : length));

			return GetFixedNumber((new Random()).Next(min, max), length);
		}

		public static bool CheckIsInsuranceAvailableCity(string city)
		{
			const string CNSTR_IDX_CITIES = "Insurance_Available_Cities_{0}";
			var idxs = Configurations.ConfigStore.InsuranceSettings.Insurance_Available_Cities;

			if (!string.IsNullOrEmpty(idxs))
			{
				foreach (var idx in idxs.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
				{
					var idx_cities = System.Configuration.ConfigurationManager.AppSettings[string.Format(CNSTR_IDX_CITIES, idx)];

					if (!string.IsNullOrEmpty(idx_cities))
					{
						if (idx_cities.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(city))
						{
							return true;
						}
					}
				}
			}

			return false;
		}
		#endregion
	}
}
