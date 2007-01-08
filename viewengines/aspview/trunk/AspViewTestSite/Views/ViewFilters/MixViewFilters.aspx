<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<%
%>
outside the filter
<filter:SingleLineViewFilter>
first line
<filter:UpperCaseViewFilter>
THIS TEXT SHOULD BE IN UPPER CASE AND IN THE SAME LINE AS THE SURROUNDING TEXT
</filter:UpperCaseViewFilter>
second line
</filter:SingleLineViewFilter>
outside the filter again - first line
outside the filter again - second line