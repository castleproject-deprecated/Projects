#region License
// Copyright (c) 2008, James M. Curran
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
// limitations under the License.
#endregion
#region References
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Web;

#endregion

namespace Castle.MonoRail.ViewComponents
{
    using Castle.MonoRail.Helpers;
    using Castle.MonoRail.Framework;

    /// <summary>
    /// 
    ///  ViewComponent to generate an IMG tag for a Gravatar icon.
    ///  
    /// </summary><remarks><para>
    ///  
    /// ViewComponent to generate IMG tag for a user-specific Gravatar 
    ///  icon, based on the user's email address. 
    ///  
    /// </para><para>
    ///  
    ///  Gravatar ("Globally Recognized Avatar"; http://en.gravatar.com)  is a service which allows 
    ///  users to register a personal icon (avatar) to their email address, and displays that avatar 
    ///  when requested.  This allows blogs and other websites which know a users email address
    ///  to display their personal avatar next to their comments.  For users who have not registered 
    ///  an icon, a graphic pattern based on a hash of thier email address can be displayed, so all people 
    ///  can have a personal icon which is consistant across websites.
    ///  
    ///  </para><para>
    ///  
    ///  There are three separate APIs for this functionality.  
    ///  
    ///  <list type="number"><item><description>
    ///  
    ///  Through the ViewComponent described here, for use in Views
    ///  
    ///  </description></item><item><description>
    ///  
    ///  Through the static CreateImgTag methed in <see cref="GravatarHelper"/>, for use in Controllers.
    ///  
    ///  </description></item><item><description>
    ///  
    ///  Through any of the instance CreateImgTag methed overloads in <see cref="GravatarHelper"/>, for use in wherever convenient.
    ///  
    ///  </description></item></list></para>
    ///  <para>
    /// 
    /// GravatarViewComponent is a line component with one required and five optional parameters.
    ///  
    /// </para><para><list type="table"><listheader>
    /// 
    /// <term>                                Section                                                             </term>
    /// <description>      Description                                                                       </description>
    /// </listheader>
    /// <item>
    /// <term>                                 Email                                                                 </term>
    /// <description>
    /// 
    /// The email address for the user for whom you want the Gravatar icon.  String, Required.
    /// 
    /// </description></item><item>
    /// <term>                                  Size                                                                 </term>
    /// 
    /// <description>
    /// 
    /// Size, in pixel of the desired icon.  (Icon is square so Size applies to both height &amp; width) Integer, Optional, defaults to 80 pixels
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 DefaultImage                                                                 </term>
    /// <description>
    /// 
    /// The image to display, if the email address does not have an image registered with Gravatar.com.  
    /// Either the fully-qualified URL to an image to use, -or- one of these special values which produce dynamic default images:
    /// "monsterid", "wavatar" or "identicon".   String, optional, defaults to Gravatar's Blue G logo.  (see descriptions below)
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 OutputGravatarSiteLink                                                        </term>
    /// <description>
    /// 
    /// Wraps the IMG tag in an A tag which links to gravatar.com, so users can get their own avatar.  Bool, defaults to <c>true</c>.
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 LinkTitle                                                        </term>
    /// <description>
    /// 
    /// Text for the title= attribute of the A tag wrapping the image.  Displays as a tool tip.  String, optional, defaults to 
    /// "Get your avatar".  Used only if OutputGravatarSiteLink is set to true.  
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 MaxAllowedRating                                             </term>
    /// <description>
    ///                                   String, one of "G", "PG", "R", "X".   Defaults to "G"  (See descriptions below)
    /// </description>
    /// </item>
    ///  </list>
    ///  </para><para>
    ///   <list type="table">
    /// <listheader>
    /// <term>Default Image keywords</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>                                 identicon                                             </term>
    /// <description>
    /// 
    ///   A randomly generated assortment of shapes, reflected to be symetrical (based on the work of Don Park, http://donpark.wordpress.com/2007/01/18/visual-security-9-block-ip-identification) 
    ///   (used by wordpress.com and stackoverflow.com, so it would be the most popular default).
    ///   
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 monsterid                                             </term>
    /// <description>
    /// 
    ///  A monster is created from the following parts: body (10 different), arms(5), legs(5), eyes (10), mouth(10) and hair(5). 
    ///  (based on the work of Andreas Gohr, http://www.splitbrain.org/projects/monsterid)
    ///  
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 wavatar                                             </term>
    /// <description>
    /// 
    ///  Can best be described as geometric shapes with faces.  (based on the work of Shamus Young, http://www.shamusyoung.com/twentysidedtale/?p=1462)
    ///  
    /// </description>
    /// </item>
    /// </list></para>
    /// <para>
    ///   <list type="table">
    /// <listheader>
    /// <term>MaxAllowedRating options</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>                                 G                                             </term>
    /// <description>
    /// 
    ///  Suitable for display on all websites with any audience type.
    ///  
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 PG                                             </term>
    /// <description>
    /// 
    ///  May contain rude gestures, provocatively dressed individuals, the lesser swear words, or mild violence.
    ///  
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 R                                             </term>
    /// <description>
    /// 
    ///  May contain such things as harsh profanity, intense violence, nudity, or hard drug use.
    ///  
    /// </description>
    /// </item>
    /// <item>
    /// <term>                                 X                                             </term>
    /// <description>
    /// 
    ///  May contain hardcore sexual imagery or extremely disturbing violence.
    ///  
    /// </description>
    /// </item>
    /// </list></para>
    /// </remarks>
    /// <example><code><![CDATA[
    /// 
    /// #component(Gravatar with "Email=jamescurran@mvps.org" "Size=40")
    /// 
    /// ]]></code>                              will generate HTML like this:                                          <code><![CDATA[
    /// 
    /// <a href="http://www.gravatar.com" title="Get your avatar">
    /// <img src="http://www.gravatar.com/avatar/98df3c7cc17f088af555c5accbdb2509.jpg?rating=G&size=40" 
    ///      alt="Gravatar" width="40" height="40">
    /// </a>
    /// 
    /// ]]></code></example>
    /// 
    public class GravatarViewComponent : ViewComponentEx
    {
        [ViewComponentParam("Email", Required=true)]
        public string Email {get; set; }

        GravatarHelper.RatingType rating = GravatarHelper.RatingType.G;
        [ViewComponentParam]
        public string MaxAllowedRating
        {
            get   {       return rating.ToString();       }
            set { rating = (GravatarHelper.RatingType)Enum.Parse(typeof(GravatarHelper.RatingType), value); }
        }

        [ViewComponentParam("Size", Default=80)]
        public int Size {get; set;}

        [ViewComponentParam]
        public string DefaultImage {get; set;}

        string linkTitle;
        [ViewComponentParam]
        public string LinkTitle 
        {
            get { return linkTitle;    }
            set
            {
                linkTitle = value; 
                OutputGravatarSiteLink = true;
            }
        }

        [ViewComponentParam]
        bool OutputGravatarSiteLink { get; set; }

        public override void Render()
        {
            RenderText(GravatarHelper.CreateImageTag(Email, Size, DefaultImage, rating, OutputGravatarSiteLink, linkTitle));
        }
    }
}

namespace Castle.MonoRail.Helpers
{
    /// <summary>
    /// Helper methods to generate an IMG tag for a Gravatar icon.
    /// For a more detailed description, <see cref="GravatarViewComponent"/>.
    /// </summary>
    /// <remarks>
    /// Note: This code is based on the ASP.NET reference implementation provided by 
    /// Fresh Click Media (http://www.freshclickmedia.com), althought the actual code 
    /// probably isn't recognizeable anymore.
    /// </remarks>
    public class GravatarHelper
    {
        /// <summary>
        /// Options for Rating parameter.
        /// </summary>
        public enum RatingType 
        {
            /// <summary>
            /// Suitable for display on all websites with any audience type.
            /// </summary>
            G,
            /// <summary>
            /// May contain rude gestures, provocatively dressed individuals, the lesser swear words, or mild violence.
            /// </summary>
            PG,
            /// <summary>
            /// May contain such things as harsh profanity, intense violence, nudity, or hard drug use.
            /// </summary>
            R,
            /// <summary>
            /// May contain hardcore sexual imagery or extremely disturbing violence.
            /// </summary>
            X 
        }

        private const string defaultLinkText = "Get your avatar";

        // Instance overloads

        /// <summary>
        /// Creates the image tag.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>string of HTML.</returns>
        /// <remarks>For a more detailed description of the parameters, <see cref="GravatarViewComponent"/></remarks>
        public string CreateImageTag(string email)
        {
            return GravatarHelper.CreateImageTag(email, 80, null, RatingType.G, true, defaultLinkText);
        }

        /// <summary>
        /// Creates the image tag.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="size">The size in pixels (the height &amp; width, as the image is square)</param>
        /// <returns>string of HTML.</returns>
        /// <remarks>For a more detailed description of the parameters, <see cref="GravatarViewComponent"/></remarks>
        public string CreateImageTag(string email, int size)
        {
            return GravatarHelper.CreateImageTag(email, size, null, RatingType.G, true, defaultLinkText);
        }

        /// <summary>
        /// Creates the image tag.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="size">The size in pixels (the height &amp; width, as the image is square)</param>
        /// <param name="defaultImage">The default image, or keyword.</param>
        /// <returns>string of HTML.</returns>
        /// <remarks>For a more detailed description of the parameters, <see cref="GravatarViewComponent"/></remarks>
        public string CreateImageTag(string email, int size, string defaultImage)
        {
            return GravatarHelper.CreateImageTag(email, size, defaultImage, RatingType.G, true, defaultLinkText);
        }

        /// <summary>
        /// Creates the image tag.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="size">The size in pixels (the height &amp; width, as the image is square)</param>
        /// <param name="maxAllowedRating">The max allowed rating.</param>
        /// <param name="defaultImage">The default image, or keyword.</param>
        /// <param name="outputGravatarSiteLink">If set to <c>true</c> wrap image tag in anchor tag..</param>
        /// <param name="linkTitle">The title for the Gravatar site link..</param>
        /// <returns>string of HTML.</returns>
        /// <remarks>For a more detailed description of the parameters, <see cref="GravatarViewComponent"/></remarks>
        public string CreateImageTag(string email, int size, RatingType maxAllowedRating, string defaultImage, bool outputGravatarSiteLink, string linkTitle)
        {
            return GravatarHelper.CreateImageTag(email, size, defaultImage, maxAllowedRating, outputGravatarSiteLink, linkTitle);
        }

        /// <summary>
        /// Creates a Gravatar image tag.  
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="size">The size in pixels (the height &amp; width, as the image is square)</param>
        /// <param name="defaultImage">The default image, or keyword.</param>
        /// <param name="maxAllowedRating">The max allowed rating.</param>
        /// <param name="outputGravatarSiteLink">If set to <c>true</c> wrap image tag in anchor tag..</param>
        /// <param name="linkTitle">The title for the Gravatar site link..</param>
        /// <returns>string of HTML.</returns>
        /// <remarks><para>
        /// This is a static method, so that it can be called from a controller without having to 
        /// create an instance of GravatarHelper. </para>
        /// <para>The parameters here are in a slightly different order from the instance methods, to prevent ambigity.
        /// </para>
        /// For a more detailed description of the parameters, <see cref="GravatarViewComponent"/></remarks>
        /// 
        static public string CreateImageTag(string email, int size, string defaultImage, RatingType maxAllowedRating, bool outputGravatarSiteLink, string linkTitle)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email address must be specified", "email");

            if (size < 1 || size > 512)
                size = 80;

            // build up image url, including MD5 hash for supplied email:
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            byte[] hashedBytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(email.ToLower()));
            StringBuilder sb = new StringBuilder(hashedBytes.Length * 2);

            foreach(byte b in hashedBytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            string format = "";
            if (string.IsNullOrEmpty(defaultImage))
            {
                if (outputGravatarSiteLink)
                    format = @"<a href=""http://www.gravatar.com"" title=""{4}""><img src=""http://www.gravatar.com/avatar/{0}.jpg?rating={1}&size={2}"" height=""{2}"" width=""{2}"" alt=""Gravatar"" /></a>";
                else
                    format = @"<img src=""http://www.gravatar.com/avatar/{0}.jpg?rating={1}&size={2}"" height=""{2}"" width=""{2}"" alt=""Gravatar"" />";
            }
            else
            {
                if (outputGravatarSiteLink)
                    format = @"<a href=""http://www.gravatar.com"" title=""{4}""><img src=""http://www.gravatar.com/avatar/{0}.jpg?rating={1}&size={2}&default={3}"" height=""{2}"" width=""{2}"" alt=""Gravatar"" /></a>";
                else
                    format = @"<img src=""http://www.gravatar.com/avatar/{0}.jpg?rating={1}&size={2}&default={3}"" height=""{2}"" width=""{2}"" alt=""Gravatar"" />";
            }

            return string.Format(format, sb, maxAllowedRating, size, HttpUtility.UrlEncode(defaultImage), linkTitle);
        }
    }
}