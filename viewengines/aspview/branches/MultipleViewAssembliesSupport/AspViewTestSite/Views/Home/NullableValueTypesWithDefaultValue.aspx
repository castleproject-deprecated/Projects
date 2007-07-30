<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%
	int? someIntegerWithDefaultValue = default(int);
%>
<%=someIntegerWithDefaultValue%>
