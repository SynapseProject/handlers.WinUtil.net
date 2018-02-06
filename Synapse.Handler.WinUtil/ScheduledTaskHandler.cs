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

public class ScheduledTaskHandler : HandlerRuntimeBase
{
    ScheduledTaskHandlerConfig config = null;
    ScheduledTaskHandlerParameters parameters = null;
    ScheduledTaskResults results = new ScheduledTaskResults();

    public override IHandlerRuntime Initialize(string configStr)
    {
        config = this.DeserializeOrDefault<ScheduledTaskHandlerConfig>(configStr);
        return base.Initialize(configStr);
    }

    public override ExecuteResult Execute(HandlerStartInfo startInfo)
    {
        ExecuteResult result = new ExecuteResult();
        result.Status = StatusType.Success;

        // TODO : Implement DryRun Functionality
        if (startInfo.IsDryRun)
            throw new NotImplementedException("Dry Run Functionality Has Not Yet Been Implemented.");

        if (startInfo.Parameters != null)
            parameters = this.DeserializeOrDefault<ScheduledTaskHandlerParameters>(startInfo.Parameters);

        if (config.RunSequential)
        {
            foreach (ScheduledTask task in parameters.Tasks)
            {
                ProcessServiceRequest(config.Action, task, config.Timeout);
            }
        }
        else
        {
            Parallel.ForEach(parameters.Tasks, task =>
            {
                ProcessServiceRequest(config.Action, task, config.Timeout);
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

    public void ProcessServiceRequest(ServiceAction action, ScheduledTask task, int timeout)
    {
        ScheduledTaskConfig status = null;
        bool success = false;

        switch (action)
        {
            case ServiceAction.Query:
                break;
            case ServiceAction.Start:
                success = ScheduledTaskUtil.Start(task.Name, task.Server);
                break;
            case ServiceAction.Stop:
                success = ScheduledTaskUtil.Stop(task.Name, task.Server);
                break;
            case ServiceAction.Restart:
                success = ScheduledTaskUtil.Stop(task.Name, task.Server);
                System.Threading.Thread.Sleep(5000);
                success = ScheduledTaskUtil.Start(task.Name, task.Server);
                break;
        }

        status = ScheduledTaskUtil.QueryStatus(task.Name, task.Server);

        if (status != null)
        {
            OnLogMessage(action.ToString(), "Name : [" + status.Name + "] Status : [" + status.State + "]");
            results.Add(status);
        }

    }

    public override object GetConfigInstance()
    {
        return new ScheduledTaskHandlerConfig()
        {
            Action = ServiceAction.Stop,
            OutputType = OutputTypeType.Yaml,
            PrettyPrint = true,
            RunSequential = true,
            Timeout = 30000
        };
    }

    public override object GetParametersInstance()
    {
        return new ScheduledTaskHandlerParameters()
        {
            Tasks = new List<ScheduledTask>()
            {
                new ScheduledTask()
                {
                    Name = "Task 0",
                    Server = "localhost"
                }
            }
        };
    }
}