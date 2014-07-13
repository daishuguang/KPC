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
	public class ValidationCoder
	{
		#region "Fields"
		int _bgWidth = 0;
		int _bgHeight = 0;

		string _fontFace = "Comic Sans MS";
		int _fontSize = 16;
		Color _foreColor = Color.White; /*Color.FromArgb(220, 220, 220);*/
		Color _backColor = Color.FromArgb(47, 139, 19);
		Color _mixedLineColor = Color.Green; /*Color.FromArgb(220, 220, 220);*/
		HatchStyle _hatchStyle = HatchStyle.Weave;
		int _mixedLineWidth = 1;
		int _mixedLineCount = 7;

		Graphics _g = null;
		#endregion

		#region "Constructs"
		private ValidationCoder() { }

		//~ValidationCoder() { if (_g != null) { _g.Dispose(); } }
		#endregion

		#region "Static Methods"
		public static ValidationCoder CreateInstance()
		{
			return new ValidationCoder();
		} 
		#endregion

		#region "Properties"
		public string ValidationCode { get; set; } 
		#endregion

		#region "Public Methods"
		public string GetCode(int length)
		{
			this.ValidationCode = GetRandomCode(length);

			return this.ValidationCode;
		}

		public byte[] GetImage(int length, bool allowMixedLines = true)
		{
			this.ValidationCode = GetRandomCode(length);

			return GenerateImage(allowMixedLines);
		}

		public byte[] GetImage(string code, bool allowMixedLines = true)
		{
			this.ValidationCode = code;

			return GenerateImage(allowMixedLines);
		}
		#endregion

		#region "Private Methods"
		byte[] GenerateImage(bool allowMixedLines = true)
		{
			var length = this.ValidationCode.Length;

			try
			{
				//校验码字体
				var font = new Font(_fontFace, _fontSize);

				//根据校验码字体大小算出背景大小
				_bgWidth = (int)font.Size * length + 4;
				_bgHeight = (int)font.Size * 2;

				//生成背景图片
				using (var img = new Bitmap(_bgWidth, _bgHeight))
				{
					_g = Graphics.FromImage(img);

					DrawBackground();
					DrawValidationCode(this.ValidationCode, font);

					if (allowMixedLines)
					{
						DrawMixedLine();
					}

					var imgData = default(byte[]);

					using (var mem = new MemoryStream())
					{
						img.Save(mem, System.Drawing.Imaging.ImageFormat.Png);

						imgData = mem.ToArray();
					}

					return imgData;
				}
			}
			finally
			{
				if (_g != null)
				{
					_g.Dispose();
				}
			}
		}

		void DrawBackground()
		{
			//设置填充背景时用的笔刷
			var hBrush = new HatchBrush(_hatchStyle, _backColor);

			//填充背景图片
			_g.FillRectangle(hBrush, 0, 0, _bgWidth, _bgHeight);
		}

		void DrawValidationCode(string vCode, Font font)
		{
			_g.DrawString(vCode, font, new SolidBrush(_foreColor), 2, 2);
		}

		void DrawMixedLine()
		{
			for (int i = 0; i < _mixedLineCount; i++)
			{
				_g.DrawBezier(new Pen(new SolidBrush(_mixedLineColor), _mixedLineWidth), RandomPoint(), RandomPoint(), RandomPoint(), RandomPoint());
			}
		}

		string GetRandomCode(int length)
		{
			var sb = new StringBuilder(6);

			for (int i = 0; i < length; i++)
			{
				sb.Append(Char.ConvertFromUtf32(RandomAZ09()));
			}

			return sb.ToString();
		}

		int RandomAZ09()
		{
			System.Threading.Thread.Sleep(15);

			var ram = new Random();
			var i = ram.Next(2);
			var result = 48;

			switch (i)
			{
				case 0:
					result = ram.Next(48, 58);
					break;
				case 1:
					result = ram.Next(65, 91);
					break;
			}

			return result;
		}

		Point RandomPoint()
		{
			System.Threading.Thread.Sleep(15);

			var ram = new Random();

			return new Point(ram.Next(_bgWidth), ram.Next(_bgHeight));
		} 
		#endregion
	}
}
