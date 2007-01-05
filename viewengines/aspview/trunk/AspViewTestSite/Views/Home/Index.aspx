<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%
    string[] strings;
 %>
 
 
hello from index<br />
This are the strings:<br />
<%foreach (string s in strings) { %>
    <%=s %><br />
<%} %>
        
<br />
End of normal view
<br />
<%string message = "Hello";%>
<subView:SubViewSample message="message" number = "1" ></subView:SubViewSample>
<form action="Print.rails">
<input type="text" name="theText" />
<input type="submit" value="send" />
</form>