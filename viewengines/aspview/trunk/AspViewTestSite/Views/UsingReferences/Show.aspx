<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%@ Import Namespace="TestModel" %>
<%
    Post post;
 %>
<form action="Save.rails">
    <input type="text" name="post.PublishDate" value='<%=post.PublishDate %>' />
    <input type="text" name="post.Content" value='<%=post.Content %>' />
    <input type="submit" value="Save" />
</form>
