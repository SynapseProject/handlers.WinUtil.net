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

public class IISAppPoolHandler : HandlerRuntimeBase
{
    IISAppPoolHandlerConfig config = null;
    IISAppPoolHandlerParameters parameters = null;
    AppPoolResults results = new AppPoolResults();

    public override IHandlerRuntime Initialize(string configStr)
    {
        config = this.DeserializeOrDefault<IISAppPoolHandlerConfig>(configStr);
        return base.Initialize(configStr);
    }

    public override ExecuteResult Execute(HandlerStartInfo startInfo)
    {
        ExecuteResult result = new ExecuteResult();
        result.Status = StatusType.Success;

        if (startInfo.Parameters != null)
            parameters = this.DeserializeOrDefault<IISAppPoolHandlerParameters>(startInfo.Parameters);

        if (config.RunSequential)
        {
            foreach (IISAppPool pool in parameters.IISAppPools)
            {
                ProcessServiceRequest(config.Action, pool, config.Timeout);
            }
        }
        else
        {
            Parallel.ForEach(parameters.IISAppPools, pool =>
            {
                ProcessServiceRequest(config.Action, pool, config.Timeout);
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

    public void ProcessServiceRequest(ServiceAction action, IISAppPool pool, int timeout)
    {
        AppPoolConfig status = null;
        bool success = false;

        switch (action)
        {
            case ServiceAction.Create:
                String username = pool.UserName;
                if (!String.IsNullOrWhiteSpace(pool.Domain))
                    username = pool.Domain + @"\" + pool.UserName;
                AppPoolUtil.Create(pool.Name, pool.Server, config.Timeout, pool.ManagedRuntimeVersion, (int)pool.IdentityType, username, pool.Password);
                break;
            case ServiceAction.Delete:
                AppPoolUtil.Delete(pool.Name, pool.Server, config.Timeout);
                break;
            case ServiceAction.Query:
                break;
            case ServiceAction.Start:
                success = AppPoolUtil.Start(pool.Name, pool.Server, config.Timeout);
                break;
            case ServiceAction.Stop:
                success = AppPoolUtil.Stop(pool.Name, pool.Server, config.Timeout, 3, 5000);
                break;
            case ServiceAction.Restart:
                success = AppPoolUtil.Recycle(pool.Name, pool.Server, config.Timeout);
                break;
            case ServiceAction.StartMode:
                break;

        }

        status = AppPoolUtil.QueryStatus(pool.Name, false, pool.Server);

        if (status != null)
        {
            OnLogMessage(action.ToString(), "Name : [" + status.AppPoolName + "] Status : [" + status.State + "]");
            results.Add(status);
        }

    }

    public override object GetConfigInstance()
    {
        return new IISAppPoolHandlerConfig()
        {
            Action = ServiceAction.Start,
            OutputType = OutputTypeType.Yaml,
            PrettyPrint = true,
            RunSequential = true,
            Timeout = 30000
        };
    }

    public override object GetParametersInstance()
    {
        return new IISAppPoolHandlerParameters()
        {
            IISAppPools = new List<IISAppPool>()
            {
                new IISAppPool()
                {
                    Domain = "domain",
                    IdentityType = AppPoolIdentityType.ApplicationPoolIdentity,
                    ManagedRuntimeVersion = "4.5.2",
                    Name = "AppPoolName",
                    Password = "mySuperSecretPassword",
                    Server = "localhost",
                    UserName = "UserName"
                }
            }
        };
    }
}