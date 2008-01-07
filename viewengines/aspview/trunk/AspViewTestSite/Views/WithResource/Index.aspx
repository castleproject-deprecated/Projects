<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%@ Import namespace="Castle.MonoRail.Framework.Resources"%>
<aspView:properties>
<%
	ResourceFacade MyResource;
%>
</aspView:properties>
<%=MyResource["name"] %>
