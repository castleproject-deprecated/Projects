using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolutionTransform.Solutions;
using NUnit.Framework;

namespace SolutionTransform.Tests {
    [TestFixture]
    public class SolutionRoundTrip {
        string iocSolutionJan2010 = @"
Microsoft Visual Studio Solution File, Format Version 10.00
# Visual Studio 2008
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Solution Items"", ""Solution Items"", ""{F9A88F20-64E5-442C-8C20-10C69A39EF4C}""
	ProjectSection(SolutionItems) = preProject
		Changes.txt = Changes.txt
		License.txt = License.txt
		Readme.txt = Readme.txt
	EndProjectSection
	ProjectSection(FolderStartupServices) = postProject
		{B4F97281-0DBD-4835-9ED8-7DFB966E87FF} = {B4F97281-0DBD-4835-9ED8-7DFB966E87FF}
	EndProjectSection
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Castle.MicroKernel-vs2008"", ""Castle.MicroKernel\Castle.MicroKernel-vs2008.csproj"", ""{8C6AADEB-D099-4D2A-8F5B-4EBC12AC9159}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Castle.Windsor-vs2008"", ""Castle.Windsor\Castle.Windsor-vs2008.csproj"", ""{60EFCB9B-E3FF-46A5-A2D2-D9F0EF75D5FE}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Castle.MicroKernel.Tests-vs2008"", ""Castle.MicroKernel.Tests\Castle.MicroKernel.Tests-vs2008.csproj"", ""{EF9313A4-C6E0-40F7-9E3F-530D547D3AEF}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""Castle.Windsor.Tests-vs2008"", ""Castle.Windsor.Tests\Castle.Windsor.Tests-vs2008.csproj"", ""{CB3A30A6-56D4-4F4F-9AEF-DA55E1FF6D74}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{8C6AADEB-D099-4D2A-8F5B-4EBC12AC9159}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{8C6AADEB-D099-4D2A-8F5B-4EBC12AC9159}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{8C6AADEB-D099-4D2A-8F5B-4EBC12AC9159}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{8C6AADEB-D099-4D2A-8F5B-4EBC12AC9159}.Release|Any CPU.Build.0 = Release|Any CPU
		{60EFCB9B-E3FF-46A5-A2D2-D9F0EF75D5FE}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{60EFCB9B-E3FF-46A5-A2D2-D9F0EF75D5FE}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{60EFCB9B-E3FF-46A5-A2D2-D9F0EF75D5FE}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{60EFCB9B-E3FF-46A5-A2D2-D9F0EF75D5FE}.Release|Any CPU.Build.0 = Release|Any CPU
		{EF9313A4-C6E0-40F7-9E3F-530D547D3AEF}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{EF9313A4-C6E0-40F7-9E3F-530D547D3AEF}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{EF9313A4-C6E0-40F7-9E3F-530D547D3AEF}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{EF9313A4-C6E0-40F7-9E3F-530D547D3AEF}.Release|Any CPU.Build.0 = Release|Any CPU
		{CB3A30A6-56D4-4F4F-9AEF-DA55E1FF6D74}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{CB3A30A6-56D4-4F4F-9AEF-DA55E1FF6D74}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{CB3A30A6-56D4-4F4F-9AEF-DA55E1FF6D74}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{CB3A30A6-56D4-4F4F-9AEF-DA55E1FF6D74}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(TextTemplating) = postSolution
		TextTemplating = 1
	EndGlobalSection
EndGlobal
";

        [Test]
        public void CanRoundTripSolutionFile()
        {
            var parser = new SolutionFileParser();
            var solutionFile = parser.Parse(new FilePath ("c:\\x.sln", false), iocSolutionJan2010.AsLines());
            var roundTripped = string.Join(Environment.NewLine, solutionFile.Lines().ToArray()) + Environment.NewLine;
            Assert.That(roundTripped, Is.EqualTo(iocSolutionJan2010), "Roundtripping loading the solution should not have changed it at all.");
        }
    }
}
