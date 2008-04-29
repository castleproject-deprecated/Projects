<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%@ Import namespace="Castle.MonoRail.Framework.Resources"%>
<script runat="server" type="aspview/properties">
	ResourceFacade MyResource;
</script>
<%=MyResource["name"] %>
