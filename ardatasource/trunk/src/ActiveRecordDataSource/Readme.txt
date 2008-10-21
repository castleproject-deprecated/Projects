This is an implementation of an ObjectDataSource for ActiveRecord. 

It was originally written by Michael Morton <mmorton@wickednite.com> - Here's what he said (lightly edited): 

---	
After playing around with ObjectDataSource and Active Record a month or two ago (and getting quite annoyed with it) 
I went ahead and wrote my own DataSourceControl for Active Record. It ties in nicely with Active Record 
and requires no modification of any of your models.  If you all are interested in putting it in contrib or something 
I’d be willing to maintain it.  I am currently using it in several projects and it has been working great.  It needs 
some documentation, but I can put that together pretty quickly.  A couple of samples of its use in an aspx page are 
below (overall pretty simple).

It supports paging, sorting, parameter collections, passing parameters to find functions (or just passing them as 
criteria to the normal find functions), updates, inserts, deletes, events that let you change things before they are 
sent off to AR, etc…  Pretty much just drop it in and point any of the ASP.NET data controls at it and it should work.

---

<shl:ActiveRecordDataSource

    runat="server"

    ID="subscriptionsDataSource"

    TypeName="SalesLogix.Orm.Models.ModuleSubscription"

    ThrowOnError="true"

    FindMethod="FindAllForSerial"

    >

    <FindParameters>

        <asp:FormParameter Name="serial" FormField="serial" />

    </FindParameters>   

</shl:ActiveRecordDataSource>

---

FindAllForSerial is a method of the ModuleSubscription class with the signature “public static ModuleSubscription[] FindAllForSerial(string name)”

 

and

---

<shl:ActiveRecordDataSource

        runat="server"

        ID="downloadsDataSource"

        TypeName="SalesLogix.Orm.Models.AccountLicense"

        EnablePaging="true" OnBeforeFind="downloadsDataSource_Find" ThrowOnError="true"

    >       

</shl:ActiveRecordDataSource>

---

Calls the OnBeforeFind event which fills in the criteria before sending it on to the SlicedFindAll function (since paging is enabled).

