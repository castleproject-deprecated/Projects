using System;
using Castle.Tools.CodeGenerator.Model.TreeNodes;

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	public class WizardControllerTreeNode : ControllerTreeNode
	{
		private string[] _wizardStepPages;

		public string[] WizardStepPages
		{
			get { return _wizardStepPages; }
			set { _wizardStepPages = value; }
		}

		public WizardControllerTreeNode(string name, string controllerNamespace, string[] wizardStepPages)
			: base(name, controllerNamespace)
		{
			_wizardStepPages = wizardStepPages;
		}

		public override string ToString()
		{
			if (_wizardStepPages.Length > 0)
			{
				string types = string.Empty;

				Array.ForEach(_wizardStepPages, delegate(string wizardStepPage) { types += wizardStepPage + ","; });
				types = types.Remove(types.Length - 1);

				return String.Format("{0}<{1}>", base.ToString(), types);
			}
			else
				return base.ToString();
		}
	}
}