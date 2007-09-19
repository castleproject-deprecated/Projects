require 'rest-open-uri'
require 'test/spec'
require 'rexml/document'
require 'test_helper'

BASE_URL = 'http://localhost/api/v1/customers'


context "When working with the customer resource" do 

  setup do
    open(BASE_URL + '/reset.rails')
    @http = HttpClient.new(BASE_URL)
  end

  specify "collection hould include 5 customers" do    
    @customers = @http.get
    @customers.size.should.equal 5
  end

  specify "adding a customer should update collection to 6" do
    new_customer = Customer.new
    new_customer.name = 'chris'

    response = @http.post(new_customer)
    response.status.should.equal ["201","Created"]
    @customers = @http.get
    @customers.size.should.equal 6
  end

  specify "retrieving a customer by id should work" do
    bart = @http.get(2)
    bart.should.not.be nil
    bart.name.should.equal 'Bart'
  end

  specify "updating a customer should work" do
    lisa = @http.get(4)
    lisa.name.should.equal 'Lisa'
    lisa.name = "Harry"
    response = @http.put lisa
    response.status.should.equal ["200","OK"]

    lisa2 = @http.get(4)
    lisa2.name.should.equal 'Harry'
  end

  specify "deleting a customer should decrement count" do
    customers = @http.get
    customers.size.should.equal 5
    @http.delete customers[0].id
    customers = @http.get
    customers.size.should.equal 4
  end
end


