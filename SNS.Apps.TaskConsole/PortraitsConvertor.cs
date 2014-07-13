using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.TaskConsole
{
	class PortraitsConvertor
	{
		static string CNSTR_IMAGES_FOLDER = @"C:\Users\Alex\Documents\My Projects\SNS.Apps.KPC\_Publish\Portraits_Images";

		public static void Convert()
		{
			if (!Directory.Exists(CNSTR_IMAGES_FOLDER))
			{
				Console.WriteLine("Target Folder: {0} is not exist!", CNSTR_IMAGES_FOLDER);
				return;
			}

			var imgFiles = Directory.GetFiles(CNSTR_IMAGES_FOLDER, "*.*", SearchOption.TopDirectoryOnly).Where(
				p => p.ToLower().EndsWith("jpg") || p.ToLower().EndsWith("jpeg") || p.ToLower().EndsWith("png") || p.ToLower().EndsWith("gif")
			);

			if (imgFiles != null && imgFiles.Count() == 0)
			{
				Console.WriteLine("No Image File found!");
				return;
			}

			Console.WriteLine("Start to Converting ...");
			Console.WriteLine("Total: {0}", imgFiles.Count());

			var no = 0;

			foreach (var item in imgFiles)
			{
				Console.WriteLine("No.{0:D4}", (++no));

				try
				{
					ConvertImage(item);

					Console.WriteLine("Done!");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				finally
				{
					Console.WriteLine();
					Console.WriteLine();
				}
			}

			Console.WriteLine("Completed!");
		}

		static void ConvertImage(string imgFile)
		{
			if (!File.Exists(imgFile))
			{
				return;
			}

			var imgExt = imgFile.Substring(imgFile.LastIndexOf('.') + 1);
			var imgName = imgFile.Substring(imgFile.LastIndexOf('\\') + 1);

			imgName = imgName.Substring(0, imgName.IndexOf('.'));
			
			var img = Image.FromFile(imgFile);
			var imgThumb = img.GetThumbnailImage(70, 70, new System.Drawing.Image.GetThumbnailImageAbort(() => { return false; }), IntPtr.Zero);
			var imgThumbFile = string.Format(@"{0}\thumbs\{1}.{2}", CNSTR_IMAGES_FOLDER, imgName, imgExt);

			if (File.Exists(imgThumbFile))
			{
				File.Delete(imgThumbFile);
			}

			imgThumb.Save(imgThumbFile, GetImageFormat(imgExt));
		}

		static System.Drawing.Imaging.ImageFormat GetImageFormat(string imgExt)
		{
			switch (imgExt.ToLower())
			{
				case "jpg":
				case "jpeg":
					return System.Drawing.Imaging.ImageFormat.Jpeg;
				case "bmp":
					return System.Drawing.Imaging.ImageFormat.Bmp;
				case "gif":
					return System.Drawing.Imaging.ImageFormat.Gif;
				case "icon":
					return System.Drawing.Imaging.ImageFormat.Icon;
				case "png":
					return System.Drawing.Imaging.ImageFormat.Png;
				default:
					return System.Drawing.Imaging.ImageFormat.Jpeg;
			}
		}
	}
}
