using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public static class Extensions
	{
		#region "Extend Methods"
		public static string GetDescription(this Enum obj)
		{
			return GetDescription(obj, false);
		}

		public static string GetDescription(this Enum obj, bool isTop)
		{
			if (obj == null)
			{
				return string.Empty;
			}

			try
			{
				Type _enumType = obj.GetType();
				DescriptionAttribute dna = null;

				if (isTop)
				{
					dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType, typeof(DescriptionAttribute));
				}
				else
				{
					FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, obj));

					dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
				}

				if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
				{
					return dna.Description;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}

			return obj.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		//public static T Clone<T>(this T obj)
		//{
		//	T targetDeepCopyObj;
		//	Type targetType = obj.GetType();

		//	//值类型  
		//	if (targetType.IsValueType == true)
		//	{
		//		targetDeepCopyObj = obj;
		//	}
		//	//引用类型   
		//	else
		//	{
		//		//创建引用对象
		//		targetDeepCopyObj = (T)Activator.CreateInstance(targetType);      
		//		MemberInfo[] memberCollection = obj.GetType().GetMembers();

		//		foreach (MemberInfo member in memberCollection)
		//		{
		//			if (member.MemberType == System.Reflection.MemberTypes.Field)
		//			{
		//				FieldInfo field = (FieldInfo)member;
		//				Object fieldValue = field.GetValue(obj);

		//				if (fieldValue is ICloneable)
		//				{
		//					field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
		//				}
		//				else
		//				{
		//					field.SetValue(targetDeepCopyObj, Clone(fieldValue));
		//				}

		//			}
		//			else if (member.MemberType == MemberTypes.Property)
		//			{
		//				PropertyInfo myProperty = (PropertyInfo)member;
		//				MethodInfo info = myProperty.GetSetMethod(false);

		//				if (info != null)
		//				{
		//					object propertyValue = myProperty.GetValue(obj, null);

		//					if (propertyValue is ICloneable)
		//					{
		//						myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
		//					}
		//					else
		//					{
		//						myProperty.SetValue(targetDeepCopyObj, Clone(propertyValue), null);
		//					}
		//				}
		//			}
		//		}
		//	}

		//	return targetDeepCopyObj;
		//} 
		#endregion
	}
}
