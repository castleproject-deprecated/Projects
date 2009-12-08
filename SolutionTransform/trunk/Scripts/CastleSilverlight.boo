import SolutionTransform
import System.Text.RegularExpressions

solution.Transform(
	{l|Regex.Replace(l, "-vs2008", "-Silverlight", RegexOptions.IgnoreCase)}, # rename rule
	StandardFilters.RegexFilter(["Castle.Core", "Castle.DynamicProxy", "Castle.MicroKernel", "Castle.Windsor"]), 
	StandardTransforms.SilverlightTransform(),
	StandardTransforms.CastleStandardsTransform()
)

# This script is the script for converting a castle solution to the corresponding castle silverlight solution