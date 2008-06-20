// ReSharper disable AccessToStaticMemberViaDerivedType

namespace Castle.Tools.QuerySpecBuilder.Tests.ClausesTests
{
	using NUnit.Framework;
	using NUnit.Framework.ExtensionMethods;

	using Builder.Clauses;

	[TestFixture]
	public class SelectClauseTests
	{
		[Test]
		public void Add_WhenAddingLiteral_Adds()
		{
			var expected = @"SELECT
	Id,
	Name";
			var clause = new SelectClause(null);
			clause.Add("Id");
			clause.Add("Name");
			clause.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test]
		public void Add_WhenAddingSubquery_Adds()
		{
			var expected = @"SELECT
	Id,
	Name";
			var clause = new SelectClause(null);
			clause.Add("Id");
			clause.Add("Name");
			clause.GetQueryString().Should(Be.EqualTo(expected));
		}

	}
}

// ReSharper restore AccessToStaticMemberViaDerivedType
