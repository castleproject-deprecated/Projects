import SolutionTransform
import System.Text.RegularExpressions
import SolutionTransform.ProjectFile

solution.Transform(
	StandardRename("-Silverlight"),
	DontFilter(), 
	StandardTransforms.Silverlight30Transform(),
	RebaseAssemblies(solution.BasePath, """..\lib\silverlight-3.0"""),
	ChangeOutputPaths("""build\silverlight30""")
	# ,StandardTransforms.CastleStandardsTransform()
)

# This script is the script for converting a castle solution to the corresponding castle silverlight solution

# RegexFilter(["Castle.Core", "Castle.DynamicProxy", "Castle.MicroKernel", "Castle.Windsor"])