<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<script runat="server" type="aspview/properties">
    string message = "default";
    int number = 1;
    DateTime? date = null;
</script>
message: <%=message%>
number: <%=number%>
date is null?:  <%=date==null%>