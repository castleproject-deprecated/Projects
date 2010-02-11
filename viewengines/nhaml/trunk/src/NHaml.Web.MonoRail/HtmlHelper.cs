using Castle.MonoRail.Framework.Helpers;

namespace NHaml.Web.MonoRail
{
    public static class UrlHelperExtensions 
    {
        public static string LinkTo(this UrlHelper urlHelper, string name, string action)
        {
            var str = urlHelper.For(DictHelper.Create(new[] { "action=" + action }));
            return string.Format("<a href=\"{0}\">{1}</a>", str, name);
        }

        public static string LinkTo(this UrlHelper urlHelper, string name, string controller, string action)
        {
            var str = urlHelper.For(DictHelper.Create(new[] { "controller=" + controller, "action=" + action }));
            return string.Format("<a href=\"{0}\">{1}</a>", str, name);
        }

        public static string LinkTo(this UrlHelper urlHelper, string name, string controller, string action, object id)
        {
            var str = urlHelper.For(DictHelper.Create(new[] { "controller=" + controller, "action=" + action }));
            return string.Format("<a href=\"{0}?id={1}\">{2}</a>", str, id, name);
        }

    }
}