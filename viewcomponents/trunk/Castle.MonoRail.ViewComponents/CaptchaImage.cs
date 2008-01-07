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
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;

    /// <summary>
    /// Based on: 
    /// http://www.codeproject.com/aspnet/CaptchaControl.asp
    /// http://www.codeproject.com/aspnet/CaptchaImage.asp
    /// </summary>
    public class CaptchaImage
    {
		/// <summary>
		/// Describe the amount of contortion added to the characters displayed.
		/// </summary>
        public enum FontWarpFactor
        {
			/// <summary>
			/// Characters are no distorted
			/// </summary>
            None,
			/// <summary>
			/// Characters are slightly distorted
			/// </summary>
            Low,
			/// <summary>
			/// Characters are distorted
			/// </summary>
            Medium,
			/// <summary>
			/// Characters are very distorted
			/// </summary>
            High,
			/// <summary>
			/// Characters are extremely distorted
			/// </summary>
            Extreme,
			/// <summary>
			/// Characters are distorted based on user specified
			/// WarpDivisor, RangeModifier and FontSizeAdjust.
			/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="CaptchaImage"/> class.
		/// </summary>
		/// <param name="sequence">The sequence of characters to render.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
        public CaptchaImage(string sequence, int width, int height)
        {
            m_random = new Random();
            m_sequence = sequence;
            m_width = width;
            m_height = height;
        }

		/// <summary>
		/// Gets or sets the width of the image
		/// </summary>
		/// <value>The width.</value>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

		/// <summary>
		/// Gets or sets the height of the image.
		/// </summary>
		/// <value>The height.</value>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

		/// <summary>
		/// Gets or sets the characters to display in the image.
		/// </summary>
		/// <value>The sequence.</value>
        public string Sequence
        {
            get { return m_sequence; }
            set { m_sequence = value; }
        }

		/// <summary>
		/// Gets or sets the warp divisor.
		/// Automatically sets FontWarp to "Custom"
		/// </summary>
		/// <value>The warp divisor.</value>
        public float WarpDivisor
        {
            get { return m_warpDivisor; }
            set
            {
                m_warpDivisor = value;
                m_fontWarp = FontWarpFactor.Custom;
            }
        }

		/// <summary>
		/// Gets or sets the range modifier.
		/// Automatically sets FontWarp to "Custom"
		/// </summary>
		/// <value>The range modifier.</value>
        public float RangeModifier
        {
            get { return m_rangeModifier; }
            set
            {
                m_rangeModifier = value;
                m_fontWarp = FontWarpFactor.Custom;
            }
        }

		/// <summary>
		/// Gets or sets the font size adjust.
		/// Automatically sets FontWarp to "Custom"
		/// </summary>
		/// <value>The font size adjust.</value>
        public float FontSizeAdjust
        {
            get { return m_fontSizeAdjust; }
            set
            {
                m_fontSizeAdjust = value;
                m_fontWarp = FontWarpFactor.Custom;
            }
        }

		/// <summary>
		/// Gets or sets the color of the foreground.
		/// </summary>
		/// <value>The color of the foreground.</value>
        public Color ForegroundColor
        {
            get { return m_foregroundColor; }
            set { m_foregroundColor = value; }
        }

		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
        public Color BackgroundColor
        {
            get { return m_backgroundColor; }
            set { m_backgroundColor = value; }
        }

		/// <summary>
		/// Gets or sets the color of the background noise.
		/// </summary>
		/// <value>The color of the background noise.</value>
        public Color BackgroundNoiseColor
        {
            get { return m_backgroundNoiseColor; }
            set { m_backgroundNoiseColor = value; }
        }

		/// <summary>
		/// Gets or sets the color of the foreground noise.
		/// </summary>
		/// <value>The color of the foreground noise.</value>
        public Color ForegroundNoiseColor
        {
            get { return m_foregroundNoiseColor; }
            set { m_foregroundNoiseColor = value; }
        }

		/// <summary>
		/// Gets or sets the noise divisor.
		/// </summary>
		/// <value>The noise divisor.</value>
        public int NoiseDivisor
        {
            get { return m_noiseDivisor; }
            set { m_noiseDivisor = value; }
        }

		/// <summary>
		/// Gets or sets the font warp.
		/// </summary>
		/// <value>The font warp.</value>
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

		/// <summary>
		/// Creates the image.
		/// Called by CaptchaImageHandler.
		/// </summary>
		/// <returns></returns>
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
}