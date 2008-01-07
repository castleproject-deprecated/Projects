<%@ Page Language="C#" %>
a. Some text, located before the capturedContent component
<component:CaptureFor id="capturedContent">
This content should be rendered in the captured-for place holder
<component:Bold>Bolded, yet still in the captured-for place holder</component:Bold>
Not bolded anymore, yet still in the captured-for place holder
</component:CaptureFor>
b. Some text, located after the capturedContent component
This text should be rendered right after text b.
