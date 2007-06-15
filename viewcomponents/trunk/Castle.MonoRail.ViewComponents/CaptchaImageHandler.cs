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
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Net;
    using System.Web;

    
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