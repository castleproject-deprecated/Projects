

namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.ViewComponents;

    /// <summary>
    /// 
    /// </summary>
    [Layout("default")]
    public class FaqController : SmartDispatcherController
    {
        public void Index()
        {
            // Everything is in the view.
        }

        public void List()
        {
            // In real-life, this would probably be read from a database.
            QnA[] faqItems = new QnA[]
            {
                new QnA("Is MonoRail stable? Why it's not 1.0?", 
                    "<p>Yes, very stable, albeit there's always room for improvements. Check our issue tracker.</p>" +
                    "<p>We are not 1.0 because there is an important feature not implemented yet: Caching support.</p>"),
                new QnA("Is there any public site using MonoRail?", 
                    @"<p>See this <a href=""http://forum.castleproject.org/viewforum.php?f=6"" >forum section</a></p>"),
                new QnA("Where to ask for help?",
                    "<p>The best place for ask for help - and to check if your question hasn't been asked before - is our forum.</p>")
            };

            PropertyBag["faqItems"]=faqItems;
        }
    }
}
