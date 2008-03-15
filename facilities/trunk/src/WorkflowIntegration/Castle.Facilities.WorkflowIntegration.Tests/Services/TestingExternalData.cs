using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities;
using Castle.Core;

namespace Castle.Facilities.WorkflowIntegration.Tests.Services
{
    [ExternalDataExchange]
    public interface ITestingExternalData
    {
        string MostRecentFullName { get; }
        string CreateFullName(string first, string last);
        event EventHandler<SurveyEventArgs> SurveyComplete;
    }

    [Serializable]
    public class SurveyEventArgs : ExternalDataEventArgs
    {
        public SurveyEventArgs(Guid instanceId, string name) : base(instanceId)
        {
            _name = name;
        }
        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    [Singleton]
    public class TestingExternalData : ITestingExternalData
    {
        string _mostRecentFullName;
        public string MostRecentFullName
        {
            get { return _mostRecentFullName; }
            set { _mostRecentFullName = value; }
        }

        string _fullNameFormat = "{0} {1}";
        public string FullNameFormat
        {
            get { return _fullNameFormat; }
            set { _fullNameFormat = value; }
        }

        public string CreateFullName(string first, string last)
        {
            string fullName = string.Format(_fullNameFormat, first, last);
            _mostRecentFullName = fullName;
            return fullName;
        }

        public void OnSurveyComplete(Guid instanceId, string name)
        {
            if (SurveyComplete != null)
                SurveyComplete(null, new SurveyEventArgs(instanceId, name));
        }

        public event EventHandler<SurveyEventArgs> SurveyComplete;
    }
}
