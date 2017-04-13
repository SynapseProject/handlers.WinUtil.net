using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Synapse.Core;
using Synapse.Handlers.Template;

// Things to remember when using this template :
// ---------------------------------------------------------
// TODO : Rename TemplateFileHandler Class and File.
// TODO : Edit AssemblyInfo.cs with Handler Name.
// TODO : Modify namespace in all files.
// TODO : Edit Project Properties.  Change AssemblyName and Default Namespace.
// TODO : Rename Project
// TODO : Rename Project Folder (delete and re-add to project)

public class TemplateHandler : HandlerRuntimeBase
{
    HandlerConfig config = null;
    HandlerParameters parameters = null;

    public override IHandlerRuntime Initialize(string configStr)
    {
        config = this.DeserializeOrDefault<HandlerConfig>(configStr);
        return base.Initialize(configStr);
    }

    public override ExecuteResult Execute(HandlerStartInfo startInfo)
    {
        ExecuteResult result = new ExecuteResult();
        result.Status = StatusType.Success;

        if (startInfo.Parameters != null)
            parameters = this.DeserializeOrDefault<HandlerParameters>(startInfo.Parameters);

        OnLogMessage("ConfigValues", "================================");
        OnLogMessage("ConfigValues", "ConfigValue1 = " + config.ConfigValue1);
        OnLogMessage("ConfigValues", "ConfigValue2 = " + config.ConfigValue2);
        foreach (String value in config.ConfigValues3)
            OnLogMessage("ConfigValues", "ConfigValue3 = " + value);

        OnLogMessage("ParamValues", "================================");
        OnLogMessage("ParamValues", "ParamValue1  = " + parameters.ParamValue1);
        OnLogMessage("ParamValues", "ParamValue2  = " + parameters.ParamValue2);        foreach (String value in config.ConfigValues3)
            foreach (String param in parameters.ParamValues3)
                OnLogMessage("ParamValues", "ParamValue3  = " + param);

        return result;
    }
}

