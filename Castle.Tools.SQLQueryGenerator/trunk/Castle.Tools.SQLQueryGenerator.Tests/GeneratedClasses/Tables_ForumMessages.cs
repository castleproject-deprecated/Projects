namespace Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses
{
	public class Tables_ForumMessages : Runtime.Model.Table.AbstractTable
	{
		public Tables_ForumMessages(string alias) : base("dbo", "ForumMessages", alias)
		{
			Id = new Tables_ForumMessages_Id(this);
			ParentId = new Tables_ForumMessages_ParentId(this);
			Content = new Tables_ForumMessages_Content(this);
		}

		public Tables_ForumMessages() : base("dbo", "ForumMessages")
		{
			Id = new Tables_ForumMessages_Id(this);
			ParentId = new Tables_ForumMessages_ParentId(this);
			Content = new Tables_ForumMessages_Content(this);
		}

		public readonly Tables_ForumMessages_Id Id;
		public readonly Tables_ForumMessages_ParentId ParentId;
		public readonly Tables_ForumMessages_Content Content;

		public Tables_ForumMessages As(string alias)
		{
			return new Tables_ForumMessages(alias);
		}
	}
}
