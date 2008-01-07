// Copyright 2007 WickedNite Productions - http://www.wickednite.com/
#region License
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

#endregion


namespace Castle.MonoRail.ViewComponents
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;
    #endregion

    /// <summary>
    /// ViewComponent to create a CAPTCHA ("<i>Completely Automated Public 
    /// Turing test to tell Computers and Humans Apart</i>") HTML form.
    /// </summary>
    /// <remarks>Can be used as a line component (create a asimple form) 
    /// or as a block component where the layout of the form is specified. <para/>
    /// 
    /// Requires <see cref="CaptchaImageHandler"/> to be installed as a httpHandler. (see examples) <para />
    /// Based on the techniques described in these articles:                          <para/>
    /// <i>CAPTCHA Image</i> By "BrainJar"                                                   <para/>
    /// http://www.codeproject.com/aspnet/CaptchaControl.asp                          <para/>
    /// and                                                                           <para/>
    /// <i>A CAPTCHA Server Control for ASP.NET</i> By "wumpus1" (aka Jeff Atwood, www.codinghorror.com) <para/>
    /// http://www.codeproject.com/aspnet/CaptchaImage.asp                            <para/>
    /// 
    /// <list type="table">
    /// <listheader>
    ///     <term>Component property</term>
    ///     <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>               Id                   </term>
    /// <description>string, id used to create data id and text field id, defaults to "captcha" </description>
    /// </item>
    /// <item>
    /// <term>              ForegroundColor      </term>
    /// <description>string, defaults to "#000000" (black)</description>
    /// </item>
    /// <item>
    /// <term>              BackgroundColor      </term>
    /// <description>string, defaults to "#FFFFFF" (white)</description>
    /// </item>
    /// <item>
    /// <term>              ForegroundNoiseColor      </term>
    /// <description>string, defaults to "#CCCCCC" (a light grey) </description>
    /// </item>
    /// <item>
    /// <term>              BackgroundNoiseColor      </term>
    /// <description>string defaults to "#666666" (a darker greenish grey)</description>
    /// </item>
    /// <item>
    /// <term>              Width      </term>
    /// <description>int, defaults to 256</description>
    /// </item>
    /// <item>
    /// <term>              Height      </term>
    /// <description>int, defaults to 64</description>
    /// </item>
    /// <item>
    /// <term>              Length      </term>
    /// <description>int, the number of characters in the image. defaults to 4.</description>
    /// </item>
    /// <item>
    /// <term>              FontWarp      </term>
    /// <description>(int 0 - 4), defaults to 1</description>
    /// </item>
    /// <item>
    /// <term>              Url      </term>
    /// <description>string, defaults to "captchaimage.ashx?d={0}"</description>
    /// </item>
    /// <item>
    /// <term>              Letters      </term>
    /// <description>string, Characters used to create the image. defaults to A-Z and 0-9</description>
    /// </item>
    /// <item>
    /// <term>              ValidFor      </term>
    /// <description>int, Minutes that the image is valid for, defaults to 15</description>
    /// </item>
    /// </list>
    /// <list type="table">
    /// <listheader><term>View Variable</term><description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>               DataId                   </term>
    /// <description>id of data field</description>
    /// </item>
    /// <item>
    /// <term>               FieldId                   </term>
    /// <description>id of text field</description>
    /// </item>
    /// <item>
    /// <term>               width                   </term>
    /// <description> </description>
    /// </item>
    /// <item>
    /// <term>               height                   </term>
    /// <description> </description>
    /// </item>
    /// <item>
    /// <term>               data                   </term>
    /// <description>encrypted data for validation</description>
    /// </item>
    /// <item>
    /// <term>               url                   </term>
    /// <description>url to image</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// Add to httpHandlers section of Web.config
    /// <code><![CDATA[
    /// <add verb="*" path="captchaimage.aspx" 
    ///     type="Castle.MonoRail.ViewComponents.CaptchaImageHandler, Castle.MonoRail.ViewcComponents"/> 
    /// ]]></code>
    /// 
    /// From your view (short version, brail): 
    /// <code><![CDATA[
    /// <% component CaptchaComponent, { } %> 
    /// ]]></code>
    /// From your controller (to validate - short version): 
    /// <code><![CDATA[
    /// CaptchaComponent.IsValid() 
    /// ]]></code>
    ///
    /// From your view (longer version): 
    /// <code><![CDATA[
    /// <% component CaptchaComponent, { 'id' : 'two', 'foregroundColor' : '#00FF00', 'fontWarp' : 3 }: %> 
    /// <% section template: %> 
    ///    ${FormHelper.HiddenField(dataId, data)} 
    ///    <img src="${url}" alt="captcha" width="${width}" height="${height}" /> 
    ///    ${FormHelper.TextField(fieldId)} 
    /// <% end %> 
    /// <% end %> 
    /// ]]></code>
    ///
    /// From your controller (to validate - longer version): 
    /// <code><![CDATA[
    ///   CaptchaComponent.IsValid("two") 
    /// ]]></code>
    /// 
    /// </example>
    [ViewComponentDetails("CaptchaComponent", Sections = "template")]
	public class CaptchaComponent : ViewComponent
    {
        #region private Instance fields
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
#endregion

        /// <summary>
        /// Handles Encrypting &amp; Decrypting text.
        /// </summary>
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

            /// <summary>
            /// Encrypts the specified plain text.
            /// </summary>
            /// <param name="plain">The plain text.</param>
            /// <returns></returns>
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

            /// <summary>
            /// Decrypts the specified cypher  text.
            /// </summary>
            /// <param name="cypher">The cypher.</param>
            /// <returns></returns>
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

        /// <summary>
        /// Gets or sets id used to create data id and text field id, defaults to "captcha"
        /// </summary>
        /// <value>The id.</value>
		[ViewComponentParam]
		public string Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

        /// <summary>
        /// Gets the id of data field.
        /// </summary>
        /// <value>The data id.</value>
		public string DataId
		{
			get { return Id + "_data"; }
		}

        /// <summary>
        /// Gets or sets the Characters used to create the image. defaults to A-Z and 0-9.
        /// </summary>
        /// <value>The letters.</value>
		[ViewComponentParam]
		public string Letters
		{
			get { return m_letters; }
			set { m_letters = value; }
		}

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
		[ViewComponentParam]
		public int Width
		{
			get { return m_width; }
			set { m_width = value; }
		}

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
		[ViewComponentParam]
		public int Height
		{
			get { return m_height; }
			set { m_height = value; }
		}

        /// <summary>
        /// Gets or sets the number of characters in the image..
        /// </summary>
        /// <value>The length.</value>
		[ViewComponentParam]
		public int Length
		{
			get { return m_length; }
			set { m_length = value; }
		}

        /// <summary>
        /// Gets or sets the URL. defaults to "captchaimage.ashx?d={0}"
        /// </summary>
        /// <value>The URL.</value>
		[ViewComponentParam]
		public string Url
		{
			get { return m_url; }
			set { m_url = value; }
		}

        /// <summary>
        /// Gets or sets the encrypted data for validation.
        /// </summary>
        /// <value>The data.</value>
		public string Data
		{
			get { return m_data; }
			set { m_data = value; }
		}

        /// <summary>
        /// Gets or sets the color of the foreground. defaults to "#000000"
        /// </summary>
        /// <value>The color of the foreground.</value>
		[ViewComponentParam]
		public string ForegroundColor
		{
			get { return m_foregroundColor; }
			set { m_foregroundColor = value; }
		}

        /// <summary>
        /// Gets or sets the color of the background, defaults to "#FFFFFF" 
        /// </summary>
        /// <value>The color of the background.</value>
		[ViewComponentParam]
		public string BackgroundColor
		{
			get { return m_backgroundColor; }
			set { m_backgroundColor = value; }
		}

        /// <summary>
        /// Gets or sets the color of the background noise. defaults to "#666666" (a darker greenish grey)
        /// </summary>
        /// <value>The color of the background noise.</value>
		[ViewComponentParam]
		public string BackgroundNoiseColor
		{
			get { return m_backgroundNoiseColor; }
			set { m_backgroundNoiseColor = value; }
		}

        /// <summary>
        /// Gets or sets the color of the foreground noise, defaults to "#CCCCCC" (a light grey)
        /// </summary>
        /// <value>The color of the foreground noise.</value>
		[ViewComponentParam]
		public string ForegroundNoiseColor
		{
			get { return m_foregroundNoiseColor; }
			set { m_foregroundNoiseColor = value; }
		}

        /// <summary>
        /// Gets or sets the font warp. (int 0 - 4), defaults to 1
        /// </summary>
        /// <value>The font warp.</value>
		[ViewComponentParam]
		public int FontWarp
		{
			get { return m_fontWarp; }
			set { m_fontWarp = value; }
		}

        /// <summary>
        /// Gets or sets the Minutes that the image is valid for, defaults to 15.
        /// </summary>
        /// <value>The valid for.</value>
		[ViewComponentParam]
		public int ValidFor
		{
			get { return m_validFor; }
			set { m_validFor = value; }
		}

        /// <summary>
        /// Renders this instance.
        /// </summary>
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
				sequence+"|"+ DateTime.Now.AddMinutes(ValidFor).ToString("yyyyMMddHHmmss"));

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

        /// <summary>
        /// Packs the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
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
            output.Length--;        //remove the trailing & 
			return EncryptionHelper.Encrypt(output.ToString()); 
		}

        /// <summary>
        /// Unpacks the parameters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
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
			
			form.SetController(EngineContext.CurrentController, EngineContext.CurrentControllerContext);
			form.SetContext(EngineContext);

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

        /// <summary>
        /// Determines whether the Captcha response is correct.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
		//public static bool IsValid()
		//{
		//    return IsValid(MonoRailHttpHandler.CurrentContext.CurrentController, "captcha");
		//}

		public static bool IsValid(Controller controller)
		{
			return IsValid(controller, "captcha");
		}

		/// <summary>
		/// Determines whether the Captcha response for the specified id is valid.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="id">The id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified id is valid; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsValid(Controller controller, string id)
		{
			return IsValid(controller.Context, id);
		}

        /// <summary>
        /// Determines whether the Captcha response for the specified controller is valid.
        /// </summary>
		/// <param name="context">The context.</param>
        /// <returns>
        /// 	<c>true</c> if the specified controller is valid; otherwise, <c>false</c>.
        /// </returns>
		public static bool IsValid(IEngineContext context)
		{
			return IsValid(context, "captcha");
		}

        /// <summary>
        /// Determines whether the Captcha response for the 
        /// specified id in the specified controller is valid.
        /// </summary>
		/// <param name="context">The context.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// 	<c>true</c> if the specified controller is valid; otherwise, <c>false</c>.
        /// </returns>
		public static bool IsValid(IEngineContext context, string id)
		{
			string input = context.Request.Form[id];
			string data = context.Request.Form[id + "_data"];

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
}