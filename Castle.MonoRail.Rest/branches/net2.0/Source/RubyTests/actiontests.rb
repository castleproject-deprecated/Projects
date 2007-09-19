require 'rest-open-uri'
require 'test/spec'
require 'redgreen'

BASE_URL = 'http://localhost/api/v1'
context "Basic action routing" do

  setup do
    @endpoint = BASE_URL + '/actiontests'
  end

  specify "should route request to controller name to index action" do
    resp = open(@endpoint + '.rails')
    resp.readlines[0].should.equal 'Index'
  end

  specify "should route post request to controller to create action" do
    r = open(@endpoint + '.rails', :method => :post)
    r.readlines[0].should.equal 'Create'
  end

  specify "should route get request to controller/id to show acton" do
    r = open(@endpoint + '/1.rails')
    r.readlines[0].should.equal 'Show'
  end

  specify "should route put request to controller/id to update action" do
    r = open(@endpoint + '/1.rails', :method => :put)
    r.readlines[0].should.equal 'Update'
  end

  specify "should route delete request to controller/id to destroy action" do
    r = open(@endpoint + '/1.rails', :method => :delete)
    r.readlines[0].should.equal 'Destroy'
  end
end

context "Response content type" do

  setup do 
    @endpoint = BASE_URL + '/customers/1.rails?format='
  end

  specify "xml requests should be application/xml" do
    r = open(@endpoint + 'xml')
    r.content_type.should.equal 'application/xml'
  end

  specify "html requests should be text/html" do
    r = open(@endpoint + 'html')
    r.content_type.should.equal 'text/html'
  end

end

context "Selected view" do
  setup do
    @endpoint = BASE_URL + '/selectedviews?format='
  end

  specify "should be based on accept types" do
    r = open(@endpoint + 'xml')
    r.readlines[0].should.equal 'Xml View'

    r2 = open(@endpoint + 'html')
    r2.readlines[0].should.equal 'Html View'
  end
end

context "Routing rules" do
  setup do
    @endpoint = BASE_URL + '/customers'
  end

  specify "can access collection with no extension" do
    r = open(@endpoint)
    r.status.should.equal ["200","OK"]
  end
end
