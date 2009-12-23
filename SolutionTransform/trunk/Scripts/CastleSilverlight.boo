import SolutionTransform
import System.Text.RegularExpressions
import SolutionTransform.ProjectFile

solution.Transform(
	RegexRename("-vs2008", "-Silverlight"),
	DontFilter(), 
	StandardTransforms.SilverlightTransform(),
	RebaseAssemblies(solution, """..\lib\silverlight-3.0""")
	# ,StandardTransforms.CastleStandardsTransform()
)

# This script is the script for converting a castle solution to the corresponding castle silverlight solution

# RegexFilter(["Castle.Core", "Castle.DynamicProxy", "Castle.MicroKernel", "Castle.Windsor"])