namespace TestSiteRest.Controllers
{
	using System.Collections.Generic;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Rest;
	using Castle.MonoRail.Rest.Binding;
	using Models;

	[ControllerDetails(Area="V1")]
    public class CustomersController : RestfulController
    {
        public void Index()
        {
			Customer[] customers = Customer.FindAll();
            PropertyBag["customers"] = customers;

        	RespondTo(delegate(ResponseFormat format) {
        	          	format.Xml(delegate(Responder xml) {
        	          	                   	xml.Serialize(customers);
        	          	                   });
        	          });
        	RespondTo(delegate(ResponseFormat format) {
        	          	format.Html(delegate(Responder html) {
        	          	                    	html.DefaultResponse();
        	          	                    });
        	          });
            
        }

        public void Show(int ID)
        {
            Customer c = Customer.FindById(ID);

			RespondTo(delegate(ResponseFormat format)
			{
				format.Xml(delegate(Responder response)
				{
					response.Serialize(c);
				});
			});

			RespondTo(delegate(ResponseFormat format)
			{
				format.Html(delegate(Responder response)
				{
					response.DefaultResponse();
				});
			});
        }

        public void Create([XmlBind] Customer createMe)
        {
            Customer.AddNew(createMe);

			RespondTo(delegate(ResponseFormat format)
			{
				format.Xml(delegate(Responder response)
				{
				                   	response.Empty(201, delegate(IDictionary<string, string> headers) {
				                   	                    	headers["Location"] = UrlFor(createMe.ID.ToString());
				                   	                    });
				});
			});

        }

        public void New()
        {
			RespondTo(delegate(ResponseFormat format)
			{
				format.Xml(delegate(Responder response)
				{
					response.Serialize(new Customer());
				});
			});
        }

        public void Update(int id, [XmlBind] Customer customer)
        {
            customer.ID = id;
            Customer.UpdateCustomer(customer);

			RespondTo(delegate(ResponseFormat format)
			{
				format.Xml(delegate(Responder response)
				{
				                   	response.Empty(200);
				});
			});
        }

        public void Destroy(int ID)
        {
            Customer.Delete(ID);

			RespondTo(delegate(ResponseFormat format)
			{
				format.Xml(delegate(Responder response)
				{
					response.Empty(200);
				});
			});
        }

        public void Reset()
        {
            Customer.Reset();

			RespondTo(delegate(ResponseFormat format)
			{
				format.Xml(delegate(Responder response)
				{
					response.Empty(200);
				});
			});
        }
    }
}
