require 'rest-open-uri'
require 'test/spec'
require 'rexml/document'

class Customer
  attr_accessor :id, :name

  def initialize
    @id = 0
  end

  def to_xml
    "<Customer><ID>#{@id}</ID><Name>#{@name}</Name></Customer>"
  end

  def self.parse(xml)
    doc = REXML::Document.new xml
    customers = []
    doc.elements.each("//Customer") do |customer_el|
      c = Customer.new
      c.id = customer_el.elements["ID"].text
      c.name = customer_el.elements["Name"].text
      customers << c
    end
    customers
  end

end

class HttpClient
  attr_accessor :endpoint

  def initialize(endpoint)
    @endpoint = endpoint
  end

  def options(opts = {})
    {'Accept' => 'application/xml'}.merge opts
  end

  def get(id=nil)

    r = nil
    if id
      r = open(@endpoint + '/' + id.to_s)
    else
      r = open(@endpoint,options) unless id
    end

    xml = r.read
    parsed = Customer.parse(xml)
    return parsed[0] if id
    parsed
  end

  def post(customer)
    if customer.id == 0 
      r = open(@endpoint,options(:method => :post, :body => customer.to_xml))
    end
  end

  def put(customer)
    r = open(@endpoint + '/' + customer.id.to_s + '.rails', options(:method => :put, :body => customer.to_xml))
  end

  def delete(id)
    r = open(@endpoint + '/' + id.to_s + '.rails', options(:method => :delete))
  end
end
