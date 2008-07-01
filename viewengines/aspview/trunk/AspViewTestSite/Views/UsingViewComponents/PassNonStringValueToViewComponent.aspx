<%@ Page Language="C#"  %>
<p>
  Passing a non-null version to the component:
  <component:Version version="<%= Properties["version"] %>"></component:Version>
</p>

<p>
  Passing null to the component:
  <component:Version version="<%= null %>"></component:Version>
</p>
