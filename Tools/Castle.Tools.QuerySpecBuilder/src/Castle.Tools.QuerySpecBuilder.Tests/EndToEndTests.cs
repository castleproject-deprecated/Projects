// ReSharper disable AccessToStaticMemberViaDerivedType

namespace Castle.Tools.QuerySpecBuilder.Tests
{
	using NUnit.Framework;
	using NUnit.Framework.ExtensionMethods;

	using Builder;

	[TestFixture]
	public class EndToEndTests
	{
		[Test]
		public void SelectStar()
		{
			var expected =
@"SELECT
	*
FROM
	Customers
";
			var q = new QueryBuilder()
				.From
					.Table("Customers")
					.End
				;

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test, Ignore("Currently not needed")]
		public void SelectSelectFields()
		{
			var expected =
@"SELECT
	(
		SELECT
			Id
		FROM
			TBL
	) AS TblId
FROM
	Customers
";
			var inner = new QueryBuilder()
				.Select
					.Add("Id")
					.End
				.From
					.Table("TBL")
					.End;

			var q = new QueryBuilder()
				.Select
					.Add(inner, "TblId")
					.End
                .From
					.Table("Customers")
					.End;

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test]
		public void SelectSimpleFields()
		{
			var expected =
@"SELECT
	Id,
	Name
FROM
	Customers
";
			var q = new QueryBuilder();
			var s = q.Select;
			s.Add("Id");
			s.Add("Name");
			var f = q.From;
			f.Table("Customers");

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test]
		public void SimpleJoins()
		{
			var expected =
@"SELECT
	*
FROM
				Customers AS c
	INNER JOIN	Orders AS o ON (o.CustomerId = c.Id)
";
			var q = new QueryBuilder()
				.From
					.Table("Customers", "c")
					.Add(Joins.Table("Orders").As("o").On("o.CustomerId = c.Id"))
					.End
				;

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test]
		public void SelectFromQuery()
		{
			var expected =
@"SELECT
	i.Id
FROM
	(SELECT
		*
	FROM
		Customers
	) AS i
";
			var inner = new QueryBuilder()
				.From
					.Table("Customers")
					.End
				;
			var q = new QueryBuilder()
				.Select
					.Add("i.Id")
					.End
				.From
					.Query(inner, "i")
					.End
				;

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test]
		public void SelectStarWhere()
		{
			var expected =
@"SELECT
	*
FROM
	Customers
WHERE
		Name LIKE @Name
";
			var q = new QueryBuilder()
				.From
					.Table("Customers")
					.End
				.Where
					.Add("Name LIKE @Name")
					.End
				;

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test]
		public void SelectStarOrder()
		{
			var expected =
@"SELECT
	*
FROM
	Customers
ORDER BY
	Id,
	Name
";
			var q = new QueryBuilder()
				.From
					.Table("Customers")
					.End
				.OrderBy
					.Add("Id")
					.Add("Name")
					.End
				;

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test]
		public void SelectStarGroup()
		{
			var expected =
@"SELECT
	*
FROM
	Customers
GROUP BY
	Id,
	Name
";
			var q = new QueryBuilder()
				.From
					.Table("Customers")
					.End
				.GroupBy
					.Add("Id")
					.Add("Name")
					.End
				;

			q.GetQueryString().Should(Be.EqualTo(expected));
		}

		[Test, Ignore("Currently not needed")]
		public void SubQueries_AreBeingRegistered()
		{
		}

		[Test]
		public void SubQueries_DoesNotRenderOrderByClauseWithoutTop()
		{
			var expected =
@"SELECT
	COUNT (*)
FROM
	(SELECT
		*
	FROM
		Customers
	) AS inner
";
			var inner = new QueryBuilder()
				.From
					.Table("Customers")
					.End
				.OrderBy
					.Add("SHOULD NOT BE RENDERED")
					.End
				;

			var q = new QueryBuilder()
				.Select
					.Add("COUNT (*)")
					.End
				.From
					.Query(inner, "inner")
					.End
				;

			q.GetQueryString().Should(Be.EqualTo(expected));

		}
	}
}