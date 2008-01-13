<%@ Page Language="C#" %>
<script runat="server" type="aspview/properties">
	string[] items;
	string item;
</script>
A simple viewcomponent, without a body and sections
<component:Repeater source="<%=items%>">
	<section:header>
	<table>
		<thead>
			<th>Id</th>
			<th>Word</th>
		</thead>
	</section:header>
	<section:item>
		<tr>
			<td>1</td>
			<td><%=item %></td>
		</tr>
	</section:item>
	<section:footer>
	</table>
	</section:footer>
</component:Repeater>
I was supposed to be rendered after the viewcomponent