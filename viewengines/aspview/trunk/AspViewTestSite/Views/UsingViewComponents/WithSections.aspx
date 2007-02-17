<%@ Page Language="C#" %>
<%@ Import Namespace="Castle.MonoRail.Framework.Helpers" %>
<%
	IPaginatedPage items;
	string item;
%>
A simple viewcomponent, without a body and sections
<component:GridComponent source="<%=items%>">
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
</component:GridComponent>
I was supposed to be rendered after the viewcomponent