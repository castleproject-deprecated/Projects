<%@ Page Language="C#" %>

<aspView:properties>
<% 
	object capturedContent1;
	object capturedContent2;
%>
</aspView:properties>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from UsingMultipleViewComponents layout
    </div>
    <div>
		<h1>Under me should appear the regular content of the view</h1>
        <%=ViewContents%>
    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id="capturedContent1"</h1>
		<%=capturedContent1%>
    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id="capturedContent2"</h1>
		<%=capturedContent2%>
    </div>
</body>
</html>