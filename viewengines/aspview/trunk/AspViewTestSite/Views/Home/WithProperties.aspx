<%@ Page Language="C#" Inherits="Castle.MonoRail.Views.AspView.ViewAtDesignTime" %>
<script runat="server" type="aspview/properties">
    string message;
    int number;
</script>
WithProperties view
message: <%= message %>
number: <%=number %>