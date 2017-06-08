using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Synapse.Core;
using Synapse.Handlers.WinUtil;

public class ServiceHandler : HandlerRuntimeBase
{
    ServiceHandlerConfig config = null;
    ServiceHandlerParameters parameters = null;

    public override IHandlerRuntime Initialize(string configStr)
    {
        config = this.DeserializeOrDefault<ServiceHandlerConfig>(configStr);
        return base.Initialize(configStr);
    }

    public override ExecuteResult Execute(HandlerStartInfo startInfo)
    {
        ExecuteResult result = new ExecuteResult();
        result.Status = StatusType.Success;

        if (startInfo.Parameters != null)
            parameters = this.DeserializeOrDefault<ServiceHandlerParameters>(startInfo.Parameters);

        foreach (Service service in parameters.Services)
        {
            ProcessServiceRequest(config.Action, service, config.Timeout);
        }

        return result;
    }

    public void ProcessServiceRequest(ServiceAction action, Service service, int timeout)
    {
        string[] loadOrderGroupDependencies = service.LoadOrderGroupDependencies?.ToArray<String>();
        string[] serviceDependencies = service.ServiceDependencies?.ToArray<String>();

        OnLogMessage(service.Name, "Action = " + action);

        ServiceConfig status = null;
        ServiceReturnCode rc = ServiceReturnCode.NotSupported;
        bool success = false;

        switch (action)
        {
            case ServiceAction.Create:
                rc = ServiceUtil.CreateService(service.Name, service.Server, service.DisplayName, service.Description, service.BinPath,
                                                service.StartMode, service.StartAsUser, service.StartAsPassword,
                                                service.Type, service.OnError, service.InteractWithDesktop, service.LoadOrderGroup,
                                                loadOrderGroupDependencies, serviceDependencies);
                if (config.StartOnCreate == true)
                    success = ServiceUtil.Start(service.Name, service.Server, timeout, service.StartMode);

                status = ServiceUtil.QueryService(service.Name, service.Server);
                break;
            case ServiceAction.Delete:
                rc = ServiceUtil.DeleteService(service.Name, service.Server);
                break;
            case ServiceAction.Query:
                status = ServiceUtil.QueryService(service.Name, service.Server);
                break;
            case ServiceAction.Start:
                success = ServiceUtil.Start(service.Name, service.Server, timeout, service.StartMode, service.StartParameters?.ToArray<String>());
                status = ServiceUtil.QueryService(service.Name, service.Server);
                break;
            case ServiceAction.Stop:
                success = ServiceUtil.Stop(service.Name, service.Server, timeout, service.StartMode);
                status = ServiceUtil.QueryService(service.Name, service.Server);
                break;
            case ServiceAction.Restart:
                success = ServiceUtil.Stop(service.Name, service.Server, timeout, ServiceStartMode.Unchanged);
                Thread.Sleep(5000);
                success = ServiceUtil.Start(service.Name, service.Server, timeout, service.StartMode);
                status = ServiceUtil.QueryService(service.Name, service.Server);
                break;
            case ServiceAction.StartMode:
                rc = ServiceUtil.ChangeStartMode(service.Name, service.Server, service.StartMode);
                status = ServiceUtil.QueryService(service.Name, service.Server);
                break;

        }

        if (status != null)
        {
            OnLogMessage(service.Name, "Name        : " + status.ServiceName);
            OnLogMessage(service.Name, "DisplayName : " + status.DisplayName);
            OnLogMessage(service.Name, "Status      : " + status.State);
            OnLogMessage(service.Name, "Type        : " + status.ServiceType);
            OnLogMessage(service.Name, "ErrorCtrl   : " + status.ErrorControl);

        }
    }

    public override object GetConfigInstance()
    {
        throw new NotImplementedException();
    }

    public override object GetParametersInstance()
    {
        throw new NotImplementedException();
    }
}

