using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CheckConfigBase
{
    public CheckResult result = new CheckResult();

    public MsdkEnv env = MsdkEnv.Instance;
    public ConfigSettings configSetting = ConfigSettings.Instance;
    public DeploySettings deploySetting = DeploySettings.Instance;

    public StringBuilder errorMsg = new StringBuilder();
    public StringBuilder warnningMsg = new StringBuilder();

    public virtual void Check() { }

    public CheckConfigBase(string projectPath)
    {
        result.projectPath = projectPath;
    }

}

