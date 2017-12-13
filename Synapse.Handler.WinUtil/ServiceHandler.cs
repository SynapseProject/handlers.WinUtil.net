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
    ServiceHandlerConfig _config = null;
    ServiceHandlerParameters _parameters = null;
    ServiceResults _results = new ServiceResults();
    bool _isDryRun = false;

    public override IHandlerRuntime Initialize(string configStr)
    {
        _config = this.DeserializeOrDefault<ServiceHandlerConfig>( configStr );
        return base.Initialize( configStr );
    }

    public override ExecuteResult Execute(HandlerStartInfo startInfo)
    {
        ExecuteResult result = new ExecuteResult
        {
            Status = StatusType.Success
        };

        _isDryRun = startInfo.IsDryRun;

        if( startInfo.Parameters != null )
            _parameters = this.DeserializeOrDefault<ServiceHandlerParameters>( startInfo.Parameters );


        if( _config.RunSequential )
            foreach( Service service in _parameters.Services )
                ProcessServiceRequest( _config.Action, service, _config.Timeout );
        else
            Parallel.ForEach( _parameters.Services,
                service => ProcessServiceRequest( _config.Action, service, _config.Timeout ) );


        switch( _config.OutputType )
        {
            case OutputTypeType.Xml:
            {
                result.ExitData = _results.ToXml( _config.PrettyPrint );
                break;
            }
            case OutputTypeType.Yaml:
            {
                result.ExitData = _results.ToYaml();
                break;
            }
            case OutputTypeType.Json:
            {
                result.ExitData = _results.ToJson( _config.PrettyPrint );
                break;
            }
        }

        OnLogMessage( "Results", result.ExitData?.ToString() );
        return result;
    }

    public void ProcessServiceRequest(ServiceAction action, Service service, int timeout)
    {
        string[] loadOrderGroupDependencies = service.LoadOrderGroupDependencies?.ToArray<String>();
        string[] serviceDependencies = service.ServiceDependencies?.ToArray<String>();

        ServiceConfig status = null;
        ServiceReturnCode rc = ServiceReturnCode.NotSupported;
        bool success = _isDryRun;

        switch( action )
        {
            case ServiceAction.Create:
            {
                rc = ServiceUtil.CreateService( service.Name, service.Server, service.DisplayName, service.Description, service.BinPath,
                                                service.StartMode, service.StartName, service.StartPassword,
                                                service.Type, service.OnError, service.InteractWithDesktop, service.LoadOrderGroup,
                                                loadOrderGroupDependencies, serviceDependencies );
                if( _config.StartOnCreate == true )
                    success = ServiceUtil.Start( service.Name, service.Server, timeout, service.StartMode );

                status = ServiceUtil.QueryService( service.Name, service.Server );
                break;
            }
            case ServiceAction.Delete:
            {
                rc = ServiceUtil.DeleteService( service.Name, service.Server );
                break;
            }
            case ServiceAction.Query:
            {
                status = ServiceUtil.QueryService( service.Name, service.Server );
                break;
            }
            case ServiceAction.Start:
            {
                if( !_isDryRun )
                    success = ServiceUtil.Start( service.Name, service.Server, timeout, service.StartMode, service.StartParameters?.ToArray<String>() );
                status = ServiceUtil.QueryService( service.Name, service.Server );
                break;
            }
            case ServiceAction.Stop:
            {
                if( !_isDryRun )
                    success = ServiceUtil.Stop( service.Name, service.Server, timeout, service.StartMode );
                status = ServiceUtil.QueryService( service.Name, service.Server );
                break;
            }
            case ServiceAction.Restart:
            {
                if( !_isDryRun )
                {
                    success = ServiceUtil.Stop( service.Name, service.Server, timeout, ServiceStartMode.Unchanged );
                    Thread.Sleep( 5000 );
                    success = ServiceUtil.Start( service.Name, service.Server, timeout, service.StartMode );
                }
                status = ServiceUtil.QueryService( service.Name, service.Server );
            }
            break;
            case ServiceAction.StartMode:
            {
                if( !_isDryRun )
                    rc = ServiceUtil.ChangeStartMode( service.Name, service.Server, service.StartMode );
                status = ServiceUtil.QueryService( service.Name, service.Server );
                break;
            }
        }

        if( status != null )
        {
            OnLogMessage( action.ToString(), "Name : [" + status.ServiceName + "] Status : [" + status.State + "]" );
            _results.Add( status );
        }
    }

    public override object GetConfigInstance()
    {
        return new ServiceHandlerConfig()
        {
            Action = ServiceAction.Stop,
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