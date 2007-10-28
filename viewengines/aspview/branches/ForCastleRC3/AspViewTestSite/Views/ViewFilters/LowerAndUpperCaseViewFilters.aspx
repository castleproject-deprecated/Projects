<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<aspView:properties>
<%
%>
</aspView:properties>
Outside the filter
<filter:LowerCase>
Inside the LowerCaseViewFilter - this text should be viewed in lower case
</filter:LowerCase>
Outside the filter AGain
<filter:UpperCase>
Inside the UpperCaseViewFilter - this text should be viewed in upper case
</filter:UpperCase>
Finally - outside the filter