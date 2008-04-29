<%@ Page Language="C#" %>
Some view text
<component:CaptureFor id="capturedContent1">
This content should be rendered in the captured-for place holder no. 1
</component:CaptureFor>
<component:CaptureFor id="capturedContent2">
This content should be rendered in the captured-for place holder no. 2
</component:CaptureFor>
Some view text
The next text should be bolded:
<component:Bold>I should be bold, and within a BoldViewComponent</component:Bold>
Some view text - not bolded