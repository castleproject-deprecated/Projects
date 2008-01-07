<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<aspView:properties>
<%
    string message = "default";
    int number = 1;
    DateTime? date = null;
%>
</aspView:properties>
message: <%=message%>
number: <%=number%>
date is null?:  <%=date==null%>