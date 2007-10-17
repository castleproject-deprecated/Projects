<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime<AspViewTestSite.Interfaces.UsingDictionaryAdapter.IStupidView>" %>
<%
%>
<p><%=view.Message %></p>
<form action="DoStuff.rails">
	Name: <input type="text" name="name" value="<%= view.Name %>" /> <br />
	password: <input type="password" name="password" /> <br />
	<input type="submit" />
</form>