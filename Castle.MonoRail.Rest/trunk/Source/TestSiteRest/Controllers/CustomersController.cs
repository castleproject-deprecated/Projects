using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Castle.MonoRail.Rest;
using TestSiteRest.Models;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Rest.Binding;

namespace TestSiteRest.Controllers
{
    [ControllerDetails(Area="V1")]
    public class CustomersController : RestfulController
    {
        public void Index()
        {
            var customers = Customer.FindAll();
            PropertyBag["customers"] = customers;

            RespondTo(format =>
            {
                format.Xml(xml => xml.Serialize(customers));
                format.Html(html => html.DefaultResponse());
            });

            
        }

        public void Show(int ID)
        {
            Customer c = Customer.FindById(ID);

            RespondTo(format =>
            {
                format.Xml(response => response.Serialize(c));
                format.Html(response => response.DefaultResponse());
            });
                       
        }

        public void Create([XmlBind] Customer createMe)
        {
            Customer.AddNew(createMe);

            RespondTo(format =>
                    format.Xml(response => response.Empty(201, headers => headers["Location"] = UrlFor(createMe.ID.ToString()))));

        }

        public void New()
        {
            RespondTo(format => format.Xml(response => response.Serialize(new Customer())));
        }

        public void Update(int id, [XmlBind] Customer customer)
        {
            customer.ID = id;
            Customer.UpdateCustomer(customer);
            RespondTo(format =>
                format.Xml(response => response.Empty(200)));
        }

        public void Destroy(int ID)
        {
            Customer.Delete(ID);

            RespondTo(format =>
                format.Xml(response => response.Empty(200)));
        }

        public void Reset()
        {
            Customer.Reset();

            RespondTo(format =>
                format.Xml(response => response.Empty(200)));
        }
    }
}
