// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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


namespace Castle.Facilities.WorkflowIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using System.Workflow.Runtime;
    using System.Workflow.Runtime.Hosting;
    using System.Workflow.Activities;
    
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel;
    
    /// <summary>
    /// Facility which creates an instance of the WorkflowRuntime class
    /// and adds it to the Windsor container as an instance with a singleton
    /// lifestyle. Application classes created by the Windor container gain access to the
    /// workflow runtime through a constructor argument or public property.
    /// 
    /// Components that implement [ExternalDataExchange] attributed interfaces 
    /// are instantiated when they are added to the container and made available 
    /// to the WorkflowRuntime. This enables the CallExternalMethod and HandleExternalEvent 
    /// activities to utilize the interface's members.
    /// 
    /// Components that are descended from WorkflowRuntimeServices are instantiated
    /// and added as services to the WorkflowRuntime. This allows for workflow services
    /// such as persistence and tracking to be declared and configured without code.
    /// 
    /// </summary>
    public class WorkflowFacility : AbstractFacility
    {
        WorkflowRuntime _runtime;        
        List<IHandler> _pendingWorkflowRuntimeServices = new List<IHandler>();
        List<IHandler> _pendingExternalDataExchange = new List<IHandler>();

        protected override void Init()
        {
            Kernel.ComponentRegistered += Kernel_ComponentRegistered;

            _runtime = new WorkflowRuntime();
            Kernel.AddComponentInstance("workflowruntime", _runtime);
        }

        void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            Type implementationType = handler.ComponentModel.Implementation;

            if (IsWorkflowRuntimeService(implementationType))
            {
                lock (_pendingWorkflowRuntimeServices)
                    _pendingWorkflowRuntimeServices.Add(handler);
            }

            if (IsExternalDataExchange(implementationType))
            {
                lock (_pendingExternalDataExchange)
                    _pendingExternalDataExchange.Add(handler);
            }

            AttachServices();
        }

        bool IsWorkflowRuntimeService(Type type)
        {
            return typeof(WorkflowRuntimeService).IsAssignableFrom(type);
        }

        bool IsExternalDataExchange(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                object[] externalDataExchangeAttributes = interfaceType.GetCustomAttributes(typeof(ExternalDataExchangeAttribute), true);
                if (externalDataExchangeAttributes != null &&
                    externalDataExchangeAttributes.Length != 0)
                {
                    return true;
                }
            }
            return false;
        }

        void AttachServices()
        {
            // add any WorkflowRuntimeService based classes which are now creatable
            lock (_pendingWorkflowRuntimeServices)
            {
                List<IHandler> valid = new List<IHandler>();
                foreach (IHandler handler in _pendingWorkflowRuntimeServices)
                {
                    if (handler.CurrentState == HandlerState.Valid)
                        valid.Add(handler);
                }
                foreach (IHandler handler in valid)
                {
                    _pendingWorkflowRuntimeServices.Remove(handler);
                    _runtime.AddService(Kernel[handler.ComponentModel.Name]);
                }
            }

            // add any types with [ExternalDataExchange] attributed interfaces which are now creatable
            lock (_pendingExternalDataExchange)
            {
                List<IHandler> valid = new List<IHandler>();
                foreach (IHandler handler in _pendingExternalDataExchange)
                {
                    if (handler.CurrentState == HandlerState.Valid)
                        valid.Add(handler);
                }
                foreach (IHandler handler in valid)
                {
                    _pendingExternalDataExchange.Remove(handler);

                    ExternalDataExchangeService service = _runtime.GetService<ExternalDataExchangeService>();
                    if (service == null)
                    {
                        service = new ExternalDataExchangeService();
                        _runtime.AddService(service);
                    }
                    service.AddService(Kernel[handler.ComponentModel.Name]);
                }
            }
        }
    }
}
