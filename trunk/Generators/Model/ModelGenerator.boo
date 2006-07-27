class ModelGenerator(NamedGeneratorBase):
	[Property(Fields)] _fields as (string)
	[Property(Properties)] _properties as (string)
	
	def Run():
		_fields = [arg.ToVarName() for arg in Argv].ToArray(string)
		_properties = [arg.ToClassName() for arg in Argv].ToArray(string)
		
		MkDir(ModelsBasePath)
		Process('Model.cs', "${ModelsBasePath}/${ClassName}.cs")
		MkDir(ModelsTestsBasePath)
		Process('Test.cs', "${ModelsTestsBasePath}/${ClassName}Test.cs")
	
	def Usage():
		return 'ModelName [Property1, Property2, ...]'
	
	def Help():
		return 'Generates an ActiveRecord model class'
				
	Namespace:
		get:
			return ModelsNamespace
			
	TestsNamespace:
		get:
			return ModelsTestsNamespace



