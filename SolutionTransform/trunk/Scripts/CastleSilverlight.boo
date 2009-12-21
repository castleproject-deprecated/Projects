import SolutionTransform
import System.Text.RegularExpressions

solution.Transform(
	RegexRename("-vs2008", "-Silverlight"),
	RegexFilter(["Castle.Core", "Castle.DynamicProxy", "Castle.MicroKernel", "Castle.Windsor"]), 
	StandardTransforms.SilverlightTransform()
	# ,StandardTransforms.CastleStandardsTransform()
)

# This script is the script for converting a castle solution to the corresponding castle silverlight solution