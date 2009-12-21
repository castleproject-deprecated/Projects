import SolutionTransform
import System.Text.RegularExpressions

solution.Transform(
	DontRename(), # rename rule: null performs an in-place change
	DontFilter(), 
	StandardTransforms.CastleStandardsTransform()
)

# This script ensures (some) compliance with the castle coding standards.  Note that it's an in place change.