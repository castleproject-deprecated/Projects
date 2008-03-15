using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Runtime;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel;
using System.Workflow.Runtime.Hosting;
using System.Workflow.Activities;

namespace Castle.Facilities.WorkflowIntegration
{
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
