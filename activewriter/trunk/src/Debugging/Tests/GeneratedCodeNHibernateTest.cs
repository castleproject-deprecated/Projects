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
            string resource = GetResource("Debugging.Tests.Debugging.Tests.NHBlog.hbm.xml");
            Assert.IsNotNull(resource, "Could not get generated xml.hbm resource");
        }
    }
}
