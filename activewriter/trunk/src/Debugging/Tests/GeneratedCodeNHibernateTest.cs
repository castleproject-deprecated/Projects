// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Debugging.Tests
{
    using System.Reflection;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class GeneratedCodeNHibernateTest
    {
        #region Setup and Helpers

        public static string GetResource(string resourceName) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (TextReader textReader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                return textReader.ReadToEnd();
        }

        #endregion

        [Test]
        public void CanGenerateHbmXml()
        {
            string resource = GetResource("Debugging.Tests.NHBlog.hbm.xml");
            Assert.IsNotNull(resource, "Could not get generated xml.hbm resource");
        }
    }
}
