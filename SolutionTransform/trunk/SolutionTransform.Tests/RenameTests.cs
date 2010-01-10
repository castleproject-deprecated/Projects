using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SolutionTransform.Tests {
    [TestFixture]
    public class RenameTests {
        [Test]
        public void StandardRenameChangesDashExtension()
        {
            var rename = new StandardRename("-Silverlight");
            var renamed = rename.RenameCsproj(@"Folder\Test-vs2008.csproj");
            Assert.That(renamed, Is.EqualTo(@"Folder\Test-Silverlight.csproj"));
        }
    }
}
