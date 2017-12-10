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
    ServiceResults results = new ServiceResults();

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

        if (config.RunSequential)
        {
            foreach (Service service in parameters.Services)
            {
                ProcessServiceRequest(config.Action, service, config.Timeout);
            }
        }
        else
        {
            Parallel.ForEach(parameters.Services, service =>
            {
                ProcessServiceRequest(config.Action, service, config.Timeout);
            });
        }

        switch (config.OutputType)
        {
            case OutputTypeType.Xml:
                result.ExitData = results.ToXml(config.PrettyPrint);
                break;
            case OutputTypeType.Yaml:
                result.ExitData = results.ToYaml();
                break;
            case OutputTypeType.Json:
                result.ExitData = results.ToJson(config.PrettyPrint);
                break;
        }

        OnLogMessage("Results", result.ExitData?.ToString());
        return result;
    }

    public void ProcessServiceRequest(ServiceAction action, Service service, int timeout)
    {
        string[] loadOrderGroupDependencies = service.LoadOrderGroupDependencies?.ToArray<String>();
        string[] serviceDependencies = service.ServiceDependencies?.ToArray<String>();

        ServiceConfig status = null;
        ServiceReturnCode rc = ServiceReturnCode.NotSupported;
        bool success = false;

        switch (action)
        {
            case ServiceAction.Create:
                rc = ServiceUtil.CreateService(service.Name, service.Server, service.DisplayName, service.Description, service.BinPath,
                                                service.StartMode, service.StartName, service.StartPassword,
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
            OnLogMessage(action.ToString(), "Name : [" + status.ServiceName + "] Status : [" + status.State + "]");
            results.Add(status);
        }
    }

    public override object GetConfigInstance()
    {
        return new ServiceHandlerConfig()
        {
            Action = ServiceAction.Start,
            OutputType = OutputTypeType.Yaml,
            PrettyPrint = true,
            RunSequential = true,
            StartOnCreate = true,
            Timeout = 30000
        };
    }

    public override object GetParametersInstance()
    {
        return new ServiceHandlerParameters()
        {
            Services = new List<Service>()
            {
                new Service()
                {
                    BinPath = @"c:\path\myService.exe",
                    Description = "service description",
                    DisplayName = "My Service",
                    InteractWithDesktop = false,
                    LoadOrderGroup = "myLoadOrderGroup",
                    LoadOrderGroupDependencies = new List<string>(){ "dep0", "dep1", "dep2" },
                    Name = "MyService",
                    OnError = ErrorControlAction.SystemAttemptStartWithGoodConfiguration,
                    Server = "localhost",
                    ServiceDependencies = new List<string>(){ "dep0", "dep1", "dep2" },
                    StartMode = ServiceStartMode.Automatic,
                    StartName = "startName",
                    StartParameters = new List<string>(){ "parm0", "parm1", "parm2" },
                    StartPassword = "mySuperSecretPassword",
                    Type = WindowsServiceType.OwnProcess
                }
            }
        };
    }
}