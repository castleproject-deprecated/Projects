// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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


namespace Castle.MonoRail.Framework.View.Xslt
{


	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Xml.Xsl;
	/// <summary>
	/// Class providing the ability to do multiple Xslt transformations
	/// in sequence.
	/// </summary>
	/// <remarks>
	/// Create an instance, create stages, add stages to pipeline and execute.
	/// The results from the first transformation will be the input of the 
	/// next and so on.
	/// </remarks>
	public class XsltPipeline
	{
		/// <summary>
		/// Initializes a new instance of the XsltPipeline class.
		/// </summary>
		/// <param name="arguments"></param>
		public XsltPipeline(XsltTransformationArguments arguments)
		{
			_Arguments = arguments;
		}

		private XsltTransformationArguments _Arguments;
		/// <summary>
		/// Gets or sets the transformation arguments.
		/// </summary>
		/// <value>The transformation arguments.</value>
		public XsltTransformationArguments TransformationArguments
		{
			get { return _Arguments; }
			set { _Arguments = value; }
		}



		private List<XsltPipelineStage> _stages = new List<XsltPipelineStage>();

		/// <summary>
		/// Adds the stage.
		/// </summary>
		/// <param name="stage">The stage.</param>
		public void AddStage(XsltPipelineStage stage)
		{
			_stages.Add(stage);
		}

		/// <summary>
		/// Executes the pipeline
		/// </summary>
		/// <param name="input">The input to the first transformation.</param>
		/// <param name="output">The writer to write the output of the final transformationstage to.</param>
		public void Execute(XmlTextReader input, XmlTextWriter output)
		{
			XmlReader currentInput = input;
			MemoryStream inStream = new MemoryStream(), outStream = inStream;
			XmlWriter currentOutput;
			for (int i = 0; i < _stages.Count; i++)
			{
				XsltPipelineStage stage = _stages[i];
				if (i == _stages.Count - 1)
				{
					//Final stage
					currentOutput = output;
				}
				else
				{
					//Interim stage, make sure the stream is not closed so
					//we can read from it later on.
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.CloseOutput = false;
					currentOutput = XmlWriter.Create(outStream, settings);
				}
				stage.Execute(currentInput, currentOutput, TransformationArguments);
		

				if (i != _stages.Count - 1)
				{
					//Make sure everything is in the memory stream
					if (currentOutput.WriteState != WriteState.Closed)
						currentOutput.Flush();

					//reset the output stream
					inStream = new MemoryStream();
					outStream.WriteTo(inStream);
					inStream.Position = 0;
					outStream.Close();

					//Ouput -> Input
					currentInput = new XmlTextReader(inStream);
					outStream = new MemoryStream();
				}
				else
				{
					inStream.Close();
					outStream.Close();
				}
			}
		}
	}
}
