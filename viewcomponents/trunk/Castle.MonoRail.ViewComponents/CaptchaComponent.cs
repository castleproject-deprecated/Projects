// Copyright 2007 WickedNite Productions - http://www.wickednite.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License

namespace Castle.MonoRail.ViewComponents
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Drawing.Imaging;
	using System.Globalization;
	using System.IO;
	using System.Net;
	using System.Security.Cryptography;
	using System.Text;
	using System.Web;
	using System.Web.UI;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	[ViewComponentDetails("CaptchaComponent", Sections = "template")]
	public class CaptchaComponent : ViewComponent
	{
		private string m_id = "captcha";
		private string m_letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private string m_url = "captchaimage.ashx?d={0}";
		private string m_data;
		private string m_foregroundColor = "#000000";
		private string m_backgroundColor = "#FFFFFF";
		private string m_backgroundNoiseColor = "#CCCCCC";
		private string m_foregroundNoiseColor = "#666666";
		private int m_fontWarp = 1;
		private int m_width = 256;
		private int m_height = 64;
		private int m_length = 4;
		private int m_validFor = 15;

		public class EncryptionHelper
		{
			private static object s_syncRoot = new object();
			private static SymmetricAlgorithm s_algorithm;

			private static void Initialize()
			{
				s_algorithm = new DESCryptoServiceProvider();

				if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["captchaEncryptionKey"]))
				{
					Pair ivKeyPair = DeriveIVAndKey(ConfigurationManager.AppSettings["captchaEncryptionKey"]);
					s_algorithm.IV = ivKeyPair.First as byte[];
					s_algorithm.Key = ivKeyPair.Second as byte[];
				}
				else
				{
					//generate a default key for no-configuration use
					//this is why we need to store the instance in a static variable
					s_algorithm.GenerateIV();
					s_algorithm.GenerateKey();
				}
			}

			private static Pair DeriveIVAndKey(string singleKey)
			{
				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
				byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(singleKey));
				byte[] iv = new byte[8];
				byte[] key = new byte[8];

				Array.ConstrainedCopy(hash, 8, iv, 0, 8);
				Array.ConstrainedCopy(hash, 0, key, 0, 8);

				return new Pair(iv, key);
			}

			public static string Encrypt(string plain)
			{
				lock(s_syncRoot)
				{
					if (s_algorithm == null)
						Initialize();

					MemoryStream stream = new MemoryStream();
					CryptoStream encryptionStream = new CryptoStream(stream, s_algorithm.CreateEncryptor(), CryptoStreamMode.Write);
					StreamWriter encryptionWriter = new StreamWriter(encryptionStream);

					encryptionWriter.Write(plain);
					encryptionWriter.Close();
					encryptionStream.Close();

					byte[] buffer = stream.ToArray();

					stream.Close();

					return Convert.ToBase64String(buffer);
				}
			}

			public static string Decrypt(string cypher)
			{
				lock(s_syncRoot)
				{
					if (s_algorithm == null)
						Initialize();

					MemoryStream stream = new MemoryStream(Convert.FromBase64String(cypher));
					CryptoStream encryptionStream = new CryptoStream(stream, s_algorithm.CreateDecryptor(), CryptoStreamMode.Read);
					StreamReader encryptionReader = new StreamReader(encryptionStream);

					string plain = encryptionReader.ReadToEnd();

					encryptionReader.Close();
					encryptionStream.Close();
					stream.Close();

					return plain;
				}
			}
		}

		[ViewComponentParam]
		public string Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public string DataId
		{
			get { return Id + "_data"; }
		}

		[ViewComponentParam]
		public string Letters
		{
			get { return m_letters; }
			set { m_letters = value; }
		}

		[ViewComponentParam]
		public int Width
		{
			get { return m_width; }
			set { m_width = value; }
		}

		[ViewComponentParam]
		public int Height
		{
			get { return m_height; }
			set { m_height = value; }
		}

		[ViewComponentParam]
		public int Length
		{
			get { return m_length; }
			set { m_length = value; }
		}

		[ViewComponentParam]
		public string Url
		{
			get { return m_url; }
			set { m_url = value; }
		}

		public string Data
		{
			get { return m_data; }
			set { m_data = value; }
		}

		[ViewComponentParam]
		public string ForegroundColor
		{
			get { return m_foregroundColor; }
			set { m_foregroundColor = value; }
		}

		[ViewComponentParam]
		public string BackgroundColor
		{
			get { return m_backgroundColor; }
			set { m_backgroundColor = value; }
		}

		[ViewComponentParam]
		public string BackgroundNoiseColor
		{
			get { return m_backgroundNoiseColor; }
			set { m_backgroundNoiseColor = value; }
		}

		[ViewComponentParam]
		public string ForegroundNoiseColor
		{
			get { return m_foregroundNoiseColor; }
			set { m_foregroundNoiseColor = value; }
		}

		[ViewComponentParam]
		public int FontWarp
		{
			get { return m_fontWarp; }
			set { m_fontWarp = value; }
		}

		[ViewComponentParam]
		public int ValidFor
		{
			get { return m_validFor; }
			set { m_validFor = value; }
		}

		public override void Render()
		{
			string sequence = GenerateRandomSequence();

			Dictionary<string, string> captchaParameters = new Dictionary<string, string>();
			captchaParameters["sequence"] = sequence;
			captchaParameters["width"] = Width.ToString();
			captchaParameters["height"] = Height.ToString();
			captchaParameters["foregroundColor"] = ForegroundColor;
			captchaParameters["backgroundColor"] = BackgroundColor;
			captchaParameters["foregroundNoiseColor"] = ForegroundNoiseColor;
			captchaParameters["backgroundNoiseColor"] = BackgroundNoiseColor;
			captchaParameters["fontWarp"] = FontWarp.ToString();

			Data = EncryptionHelper.Encrypt(
				String.Concat(sequence, "|", 
							  DateTime.Now.AddMinutes(ValidFor).ToString("yyyyMMddHHmmss")));

			PropertyBag["fieldId"] = Id;
			PropertyBag["dataId"] = DataId;
			PropertyBag["data"] = Data;
			PropertyBag["width"] = Width;
			PropertyBag["height"] = Height;
			PropertyBag["url"] = String.Format(Url, HttpUtility.UrlEncode(PackParameters(captchaParameters)));
			PropertyBag[DataId] = Data;

			if (HasSection("template"))
			{
				RenderSection("template");
			}
			else
			{
				Show();
			}
		}

		public static string PackParameters(IDictionary<string, string> parameters)
		{
			StringBuilder output = new StringBuilder();
			
			foreach(string key in parameters.Keys)
			{
				if (!String.IsNullOrEmpty(parameters[key]))
				{
					output.AppendFormat(String.Format("{0}={1}&", key, HttpUtility.UrlEncode(parameters[key])));
				}
			}

			string text = output.ToString();
			return EncryptionHelper.Encrypt(text.Substring(0, text.Length - 1)); //remove the trailing & and encrypt
		}

		public static Dictionary<string, string> UnpackParameters(string text)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			text = EncryptionHelper.Decrypt(text);
			string[] parts = text.Split('&');

			foreach(string part in parts)
			{
				string[] temp = part.Split('=');

				if (temp.Length == 2) parameters[temp[0]] = HttpUtility.UrlDecode(temp[1]);
			}

			return parameters;
		}

		private void Show()
		{
			FormHelper form = new FormHelper();
			HtmlHelper html = new HtmlHelper();

			form.SetController(MonoRailHttpHandler.CurrentContext.CurrentController);
			html.SetController(MonoRailHttpHandler.CurrentContext.CurrentController);

			StringBuilder output = new StringBuilder();
			output.Append(form.HiddenField(DataId, Data));
			output.AppendFormat("<img src=\"{0}\" width=\"{1}\" height=\"{2}\" alt=\"captcha\" />", PropertyBag["url"],
			                    PropertyBag["width"], PropertyBag["height"]);
			output.Append(form.TextField(Id));

			RenderText(output.ToString());
		}

		private string GenerateRandomSequence()
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			StringBuilder sequence = new StringBuilder();
			byte[] buffer = new byte[Length];

			rng.GetBytes(buffer);

			foreach(byte b in buffer)
			{
				sequence.Append(Letters[b % Letters.Length]);
			}

			return sequence.ToString();
		}

		public static bool IsValid()
		{
			return IsValid(MonoRailHttpHandler.CurrentContext.CurrentController, "captcha");
		}

		public static bool IsValid(string id)
		{
			return IsValid(MonoRailHttpHandler.CurrentContext.CurrentController, id);
		}

		public static bool IsValid(Controller controller)
		{
			return IsValid(controller, "captcha");
		}

		public static bool IsValid(Controller controller, string id)
		{
			string input = controller.Form[id];
			string data = controller.Form[id + "_data"];

			if (!String.IsNullOrEmpty(input) && !String.IsNullOrEmpty(data))
			{
				string text = EncryptionHelper.Decrypt(data);
				string[] parts = text.Split('|');
				string sequence = parts[0];
				DateTime expires = DateTime.ParseExact(parts[1], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

				if (input == sequence && expires > DateTime.Now)
					return true;
			}

			return false;
		}
	}

	/// <summary>
	/// Based on: 
	/// http://www.codeproject.com/aspnet/CaptchaControl.asp
	/// http://www.codeproject.com/aspnet/CaptchaImage.asp
	/// </summary>
	public class CaptchaImage
	{
		public enum FontWarpFactor
		{
			None,
			Low,
			Medium,
			High,
			Extreme,
			Custom
		}

		private int m_width;
		private int m_height;
		private string m_sequence;
		private float m_warpDivisor = 6;
		private float m_rangeModifier = 1;
		private float m_fontSizeAdjust = 0.8f;
		private int m_noiseDivisor = 30;
		private FontWarpFactor m_fontWarp = FontWarpFactor.Low;
		private Random m_random;
		private Color m_foregroundColor = Color.Black;
		private Color m_backgroundColor = Color.White;
		private Color m_backgroundNoiseColor = Color.LightGray;
		private Color m_foregroundNoiseColor = Color.DarkGray;

		public CaptchaImage(string sequence, int width, int height)
		{
			m_random = new Random();
			m_sequence = sequence;
			m_width = width;
			m_height = height;
		}

		public int Width
		{
			get { return m_width; }
			set { m_width = value; }
		}

		public int Height
		{
			get { return m_height; }
			set { m_height = value; }
		}

		public string Sequence
		{
			get { return m_sequence; }
			set { m_sequence = value; }
		}

		public float WarpDivisor
		{
			get { return m_warpDivisor; }
			set
			{
				m_warpDivisor = value;
				m_fontWarp = FontWarpFactor.Custom;
			}
		}

		public float RangeModifier
		{
			get { return m_rangeModifier; }
			set
			{
				m_rangeModifier = value;
				m_fontWarp = FontWarpFactor.Custom;
			}
		}

		public float FontSizeAdjust
		{
			get { return m_fontSizeAdjust; }
			set
			{
				m_fontSizeAdjust = value;
				m_fontWarp = FontWarpFactor.Custom;
			}
		}

		public Color ForegroundColor
		{
			get { return m_foregroundColor; }
			set { m_foregroundColor = value; }
		}

		public Color BackgroundColor
		{
			get { return m_backgroundColor; }
			set { m_backgroundColor = value; }
		}

		public Color BackgroundNoiseColor
		{
			get { return m_backgroundNoiseColor; }
			set { m_backgroundNoiseColor = value; }
		}

		public Color ForegroundNoiseColor
		{
			get { return m_foregroundNoiseColor; }
			set { m_foregroundNoiseColor = value; }
		}

		public int NoiseDivisor
		{
			get { return m_noiseDivisor; }
			set { m_noiseDivisor = value; }
		}

		public FontWarpFactor FontWarp
		{
			get { return m_fontWarp; }
			set
			{
				m_fontWarp = value;
				switch(value)
				{
					case FontWarpFactor.None:
						m_warpDivisor = 7;
						m_rangeModifier = 1;
						m_fontSizeAdjust = 0.7f;
						break;
					case FontWarpFactor.Low:
						m_warpDivisor = 6;
						m_rangeModifier = 1;
						m_fontSizeAdjust = 0.8f;
						break;
					case FontWarpFactor.Medium:
						m_warpDivisor = 5;
						m_rangeModifier = 1.3f;
						m_fontSizeAdjust = 0.85f;
						break;
					case FontWarpFactor.High:
						m_warpDivisor = 4.5f;
						m_rangeModifier = 1.4f;
						m_fontSizeAdjust = 0.9f;
						break;
					case FontWarpFactor.Extreme:
						m_warpDivisor = 4;
						m_rangeModifier = 1.5f;
						m_fontSizeAdjust = 0.95f;
						break;
				}
			}
		}

		public Bitmap Create()
		{
			Bitmap image = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(image);
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle rect = new Rectangle(0, 0, Width, Height);

			Brush backgroundBrush = new SolidBrush(BackgroundColor);
			graphics.FillRectangle(backgroundBrush, rect);
			backgroundBrush.Dispose();

			AddBackgroundNoise(graphics, rect);

			int characterOffset = 0;
			double characterWidth = Width / Sequence.Length;

			foreach(char c in Sequence)
			{
				string s = new string(c, 1);

				Rectangle characterRect = new Rectangle((int) (characterOffset * characterWidth), 0, (int) characterWidth, Height);
				Font font = NextFont();
				GraphicsPath path = CreateTextPath(s, characterRect, font);

				WarpTextPath(path, characterRect);

				Brush brush = new SolidBrush(ForegroundColor);
				graphics.FillPath(brush, path);

				brush.Dispose();
				path.Dispose();
				font.Dispose();

				characterOffset += 1;
			}

			AddForegroundNoise(graphics, rect);

			graphics.Dispose();

			return image;
		}

		private void AddBackgroundNoise(Graphics graphics, Rectangle rect)
		{
			HatchBrush brush = new HatchBrush(HatchStyle.SmallConfetti, BackgroundNoiseColor, BackgroundColor);
			graphics.FillRectangle(brush, rect);
			brush.Dispose();
		}

		private void AddForegroundNoise(Graphics graphics, Rectangle rect)
		{
			HatchBrush brush = new HatchBrush(HatchStyle.LargeConfetti, ForegroundNoiseColor, BackgroundNoiseColor);

			int max = Math.Max(rect.Width, rect.Height);
			for(int i = 0; i < ((rect.Width * rect.Height) / NoiseDivisor); i++)
			{
				int x = m_random.Next(rect.Left, rect.Left + rect.Width);
				int y = m_random.Next(rect.Top, rect.Top + rect.Width);
				int w = m_random.Next(max / 50);
				int h = m_random.Next(max / 50);

				graphics.FillEllipse(brush, x, y, w, h);
			}

			brush.Dispose();
		}

		private void WarpTextPath(GraphicsPath path, Rectangle rect)
		{
			if (FontWarp == FontWarpFactor.None)
				return;

			RectangleF warpRect = new RectangleF(rect.Left, 0, rect.Width, rect.Height);
			int hRange = (int) (rect.Height / WarpDivisor);
			int wRange = (int) (rect.Width / WarpDivisor);
			int left = (int) (rect.Left - (wRange * RangeModifier));
			int top = (int) (rect.Top - (hRange * RangeModifier));
			int width = (int) (rect.Left + rect.Width + (wRange * RangeModifier));
			int height = (int) (rect.Top + rect.Height + (hRange * RangeModifier));

			left = Math.Max(0, left);
			top = Math.Max(0, top);
			width = Math.Min(Width, width);
			height = Math.Min(Height, height);

			PointF leftTop = RandomPoint(left, left + wRange, top, top + hRange);
			PointF rightTop = RandomPoint(width - wRange, width, top, top + hRange);
			PointF leftBottom = RandomPoint(left, left + wRange, height - hRange, height);
			PointF rightBottom = RandomPoint(width - wRange, width, height - hRange, height);

			PointF[] points = new PointF[] {leftTop, rightTop, leftBottom, rightBottom};
			Matrix matrix = new Matrix();
			matrix.Translate(0, 0);
			path.Warp(points, warpRect, matrix, WarpMode.Perspective, 0);
		}

		private PointF RandomPoint(int xMin, int xMax, int yMin, int yMax)
		{
			return new PointF(m_random.Next(xMin, xMax), m_random.Next(yMin, yMax));
		}


		private GraphicsPath CreateTextPath(string text, Rectangle rect, Font font)
		{
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Near;

			GraphicsPath path = new GraphicsPath();
			path.AddString(text, font.FontFamily, (int) font.Style, font.Size, rect, format);

			return path;
		}

		private Font NextFont()
		{
			return new Font("Arial", (int) (Height * FontSizeAdjust), FontStyle.Bold);
		}
	}

	public class CaptchaImageHandler : IHttpHandler
	{
		private Dictionary<string, string> GetParameters(HttpContext context)
		{
			return CaptchaComponent.UnpackParameters(context.Request.QueryString["d"]);
		}

		private Rectangle GetDimensions(Dictionary<string, string> parameters)
		{
			int width;
			int height;

			if (!Int32.TryParse(parameters["width"], out width))
				width = 256;

			if (!Int32.TryParse(parameters["height"], out height))
				height = 64;

			if (width <= 0)
				width = 256;

			if (height <= 0)
				height = 64;

			width = Math.Min(1024, width);
			height = Math.Min(1024, height);

			return new Rectangle(0, 0, width, height);
		}

		public void ProcessRequest(HttpContext context)
		{
			Dictionary<string, string> parameters;

			try
			{
				parameters = GetParameters(context);
			}
			catch(Exception)
			{
				throw new HttpException((int) HttpStatusCode.NotFound, "Not Found");
			}

			if (String.IsNullOrEmpty(parameters["sequence"]))
				throw new HttpException((int) HttpStatusCode.NotFound, "Not Found");

			Rectangle dimensions = GetDimensions(parameters);

			CaptchaImage captchaImage = new CaptchaImage(parameters["sequence"], dimensions.Width, dimensions.Height);

			if (parameters.ContainsKey("foregroundColor"))
				captchaImage.ForegroundColor = ColorTranslator.FromHtml(parameters["foregroundColor"]);

			if (parameters.ContainsKey("backgroundColor"))
				captchaImage.BackgroundColor = ColorTranslator.FromHtml(parameters["backgroundColor"]);

			if (parameters.ContainsKey("foregroundNoiseColor"))
				captchaImage.ForegroundNoiseColor = ColorTranslator.FromHtml(parameters["foregroundNoiseColor"]);

			if (parameters.ContainsKey("backgroundNoiseColor"))
				captchaImage.BackgroundNoiseColor = ColorTranslator.FromHtml(parameters["backgroundNoiseColor"]);

			if (parameters.ContainsKey("fontWarp"))
				captchaImage.FontWarp = (CaptchaImage.FontWarpFactor) Convert.ToInt32(parameters["fontWarp"]);

			Bitmap image = captchaImage.Create();

			image.Save(context.Response.OutputStream, ImageFormat.Jpeg);
			image.Dispose();

			context.Response.ContentType = "image/jpeg";
			context.Response.StatusCode = (int) HttpStatusCode.OK;
			context.ApplicationInstance.CompleteRequest();
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}