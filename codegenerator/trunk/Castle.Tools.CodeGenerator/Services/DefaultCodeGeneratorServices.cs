using Castle.MonoRail.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
	public class DefaultCodeGeneratorServices : ICodeGeneratorServices
	{
		#region Methods

		private Controller _controller;
		private IControllerReferenceFactory _controllerReferenceFactory;
		private IEngineContext _railsContext;
		private IArgumentConversionService _argumentConversionService;
		private IRuntimeInformationService _runtimeInformationService;

		#endregion

		#region DefaultCodeGeneratorServices()

		public DefaultCodeGeneratorServices(IControllerReferenceFactory controllerReferenceFactory,
		                                    IArgumentConversionService argumentConversionService,
		                                    IRuntimeInformationService runtimeInformationService)
		{
			_controllerReferenceFactory = controllerReferenceFactory;
			_argumentConversionService = argumentConversionService;
			_runtimeInformationService = runtimeInformationService;
		}

		#endregion

		#region ICodeGeneratorServices Members

		public Controller Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}

		public IControllerReferenceFactory ControllerReferenceFactory
		{
			get { return _controllerReferenceFactory; }
		}

		public IEngineContext RailsContext
		{
			get { return _railsContext; }
			set { _railsContext = value; }
		}

		public IArgumentConversionService ArgumentConversionService
		{
			get { return _argumentConversionService; }
		}

		public IRuntimeInformationService RuntimeInformationService
		{
			get { return _runtimeInformationService; }
		}

		#endregion
	}
}
