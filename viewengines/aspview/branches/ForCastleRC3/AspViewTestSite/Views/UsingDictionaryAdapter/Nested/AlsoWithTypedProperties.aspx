<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime<AspViewTestSite.Interfaces.UsingDictionaryAdapter.Nested.IAlsoWithTypedPropertiesView>" %>
<%@ Import Namespace="TestModel" %>
<aspView:properties>
<%
%>
</aspView:properties>
<p id="No_<%=view.Id%>">
<%if (view.IsImportant.GetValueOrDefault(false))
	  Output("<strong>"); %>
Hello <%=view.Name%>
<%if (view.IsImportant.GetValueOrDefault(false))
	  Output("</strong>"); %>
</p>
<form action="Save.rails">
    <input type="text" name="post.PublishDate" value='<%=view.Post.PublishDate %>' />
    <input type="text" name="post.Content" value='<%=view.Post.Content %>' />
    <input type="submit" value="Save" />
</form>