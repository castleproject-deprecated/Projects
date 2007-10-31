namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
    using System.Security.Principal;
    using System.Web.Security;
    using Castle.MonoRail.Framework;

    /// <summary>
    /// 
    /// </summary>
    [Layout("default")]
    public class MaskedTextBoxController : SmartDispatcherController
    {
        /// <summary>
        /// 
        /// </summary>
        public void Index()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SSN"></param>
        /// <param name="Phone"></param>
        /// <param name="TaxId"></param>
        /// <param name="Date"></param>
        /// <param name="Productkey"></param>
        [AccessibleThrough(Verb.Post)]
        public void TestValues(string SSN, string Phone, string TaxId, string Date, string Productkey)
        {
            PropertyBag["SSN"] = SSN;
            PropertyBag["Phone"] = Phone;

            PropertyBag["TaxId"] = TaxId;
            PropertyBag["Date"] = Date;

            PropertyBag["Productkey"] = Productkey;
        }
    }
}
