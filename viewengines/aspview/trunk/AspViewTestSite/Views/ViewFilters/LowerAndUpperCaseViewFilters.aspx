<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%
%>
Outside the filter
<filter:LowerCaseViewFilter>
Inside the LowerCaseViewFilter - this text should be viewed in lower case
</filter:LowerCaseViewFilter>
Outside the filter AGain
<filter:UpperCaseViewFilter>
Inside the UpperCaseViewFilter - this text should be viewed in upper case
</filter:UpperCaseViewFilter>
Finally - outside the filter