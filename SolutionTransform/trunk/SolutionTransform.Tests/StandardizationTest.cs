using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SolutionTransform.CodingStandards;

namespace SolutionTransform.Tests {
    [TestFixture]
    public class StandardizationTest {
        [Test]
        public void CompilerDirectivesOutsideOfNamespaceShouldBePreserved()
        {
            string file = 
@"// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the ""License"");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an ""AS IS"" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SILVERLIGHT
namespace Castle.Core.Resource
{
	using System;

	/// <summary>
	/// 
	/// </summary>
	public class FileResourceFactory : IResourceFactory
	{
		public FileResourceFactory()
		{
		}

		public bool Accept(CustomUri uri)
		{
			return ""file"".Equals(uri.Scheme);
		}

		public IResource Create(CustomUri uri)
		{
			return Create(uri, null);
		}

		public IResource Create(CustomUri uri, String basePath)
		{
			if (basePath != null)
				return new FileResource(uri, basePath);
			else
				return new FileResource(uri);
		}
	}
}
#endif
";
            var standardizer = new CastleStandardizer();
            var result = standardizer.Standardize(null, file);
            Assert.IsTrue(result.Contains("#if"), "Preprocessor directive was stripped.  This is bad...");
        }
    }
}
