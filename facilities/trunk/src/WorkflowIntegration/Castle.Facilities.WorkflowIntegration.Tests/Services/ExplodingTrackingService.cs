using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Runtime.Tracking;

namespace Castle.Facilities.WorkflowIntegration.Tests.Services
{
    public class ExplodingTrackingService : TrackingService
    {
        protected override TrackingProfile GetProfile(Guid workflowInstanceId)
        {
            throw new NotImplementedException();
        }

        protected override TrackingProfile GetProfile(Type workflowType, Version profileVersionId)
        {
            throw new NotImplementedException();
        }

        protected override TrackingChannel GetTrackingChannel(TrackingParameters parameters)
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetProfile(Type workflowType, out TrackingProfile profile)
        {
            throw new NotImplementedException();
        }

        protected override bool TryReloadProfile(Type workflowType, Guid workflowInstanceId, out TrackingProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}
