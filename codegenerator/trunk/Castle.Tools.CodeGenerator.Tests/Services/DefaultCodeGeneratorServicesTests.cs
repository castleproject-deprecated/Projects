using Castle.MonoRail.Framework;
using Castle.Tools.CodeGenerator.Model;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.Tools.CodeGenerator.Services
{
	[TestFixture]
	public class DefaultCodeGeneratorServicesTests
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_controller = new TestController();
			_mocks = new MockRepository();
			_controllerReferenceFactory = _mocks.CreateMock<IControllerReferenceFactory>();
			_argumentConversionService = _mocks.CreateMock<IArgumentConversionService>();
			_runtimeInformationService = _mocks.CreateMock<IRuntimeInformationService>();
			_services = new DefaultCodeGeneratorServices(_controllerReferenceFactory, _argumentConversionService, _runtimeInformationService);
			_context = _mocks.CreateMock<IEngineContext>();
			Assert.AreEqual(_controllerReferenceFactory, _services.ControllerReferenceFactory);
			Assert.AreEqual(_argumentConversionService, _services.ArgumentConversionService);
			Assert.AreEqual(_runtimeInformationService, _services.RuntimeInformationService);
		}

		#endregion

		private MockRepository _mocks;
		private DefaultCodeGeneratorServices _services;
		private IControllerReferenceFactory _controllerReferenceFactory;
		private IArgumentConversionService _argumentConversionService;
		private IEngineContext _context;
		private IRuntimeInformationService _runtimeInformationService;
		private TestController _controller;

		[Test]
		public void GetAndSetController_Always_Work()
		{
			Assert.IsNull(_services.Controller);
			_services.Controller = _controller;
			Assert.AreEqual(_controller, _services.Controller);
		}

		[Test]
		public void GetAndSetRailsContext_Always_Works()
		{
			Assert.IsNull(_services.RailsContext);
			_services.RailsContext = _context;
			Assert.AreEqual(_context, _services.RailsContext);
		}
	}
}