// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.WorkflowIntegration.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using System.Workflow.Runtime.Tracking;

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
