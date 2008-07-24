using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Rest
{
	public class DefaultRestVerbResolver : IRestVerbResolver
	{
		public string Resolve(IRequest request)
		{
			return request.HttpMethod;
		}
	}
}