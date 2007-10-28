<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<aspView:properties>
<%
	int? someIntegerWithDefaultValue = default(int);
%>
</aspView:properties>
<%=someIntegerWithDefaultValue%>
