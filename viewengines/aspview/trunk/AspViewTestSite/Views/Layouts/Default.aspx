<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>

<aspView:properties>
<% 
 %>
</aspView:properties>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from default layout
    </div>
    <div>
        <%=ViewContents%>
    </div>
</body>
</html>
