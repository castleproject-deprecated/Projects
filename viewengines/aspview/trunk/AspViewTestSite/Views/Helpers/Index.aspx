<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<aspView:properties>
<%
    string s;
%>
</aspView:properties>
<%=this.Helpers.Form.TextField("object.field")%>