<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<aspView:properties>
<%
    string s = "default";
    int i = 1;
    DateTime? dt = null;
%>
</aspView:properties>
<%=s%>
<%=i%>
<%=dt==null%>
