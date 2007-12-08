<%@ Page Language="C#" %>
<aspView:properties>
<%
	string text = "some variable text";
%>
</aspView:properties>
Some view text
<component:CaptureFor id="capturedContent1">
This content should be rendered in the captured-for place holder no. 1
</component:CaptureFor>
<component:CaptureFor id="capturedContent2">
This content should be rendered in the captured-for place holder no. 2
</component:CaptureFor>
Some view text
The next text should be bolded:
<component:Bold>I should be bold, <%=text %>and within a BoldViewComponent</component:Bold>
Some view text - not bolded