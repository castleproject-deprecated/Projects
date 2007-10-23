

namespace Castle.MonoRail.ViewComponents
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Castle.MonoRail.Framework;


    /// <summary>
    /// 
    /// </summary>
    public class WhitespaceTransformFilter : TransformFilter
    {
        ///private static readonly Regex _reg = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}");

        ///New simplified Regex found at http://blog.madskristensen.dk/post/Remove-whitespace-from-your-pages.aspx
        private static readonly Regex _betweenTags = new Regex(@">\s+<", RegexOptions.Compiled);
        private static readonly Regex _lineBreaks = new Regex(@"\n\s+", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="WhitespaceFilter"/> class.
		/// </summary>
		/// <param name="baseStream">The stream to write to after filtering.</param>
        public WhitespaceTransformFilter(Stream baseStream)
            : base(baseStream)
		{
		}


        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support writing. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <exception cref="T:System.ArgumentNullException">buffer is null. </exception>
        /// <exception cref="T:System.ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Closed) throw new ObjectDisposedException("WhitespaceTransformFilter");

            string content = Encoding.Default.GetString(buffer, offset, count);

            content = _betweenTags.Replace(content, "> <");
            content = _lineBreaks.Replace(content, string.Empty);

            byte[] output = Encoding.Default.GetBytes(content);
            BaseStream.Write(output, 0, output.Length);
        }
    }
}
