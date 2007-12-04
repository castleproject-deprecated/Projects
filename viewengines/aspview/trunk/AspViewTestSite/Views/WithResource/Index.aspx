<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%@ Import namespace="Castle.MonoRail.Framework.Internal"%>
<aspView:properties>
<%
	ResourceFacade MyResource;
%>
</aspView:properties>
<%=MyResource["name"] %>
