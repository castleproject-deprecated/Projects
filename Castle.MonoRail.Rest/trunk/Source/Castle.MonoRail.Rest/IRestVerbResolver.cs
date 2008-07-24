using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Rest
{
	public interface IRestVerbResolver
	{
		string Resolve(IRequest request);
	}
}