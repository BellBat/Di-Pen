using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace Pen
{
    public class PenGenerate : Task
    {
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"PenGenerate-Parameter:{nameof(Namespace)}={Namespace}");
            Log.LogMessage(MessageImportance.High, $"PenGenerate-Parameter:{nameof(InputDll)}={InputDll}");
            Log.LogMessage(MessageImportance.High, $"PenGenerate-Parameter:{nameof(OutputDirectory)}={OutputDirectory}");
            Log.LogMessage(MessageImportance.High, $"PenGenerate-Environment:{nameof(Environment.CurrentDirectory)}={Environment.CurrentDirectory}");
            Log.LogMessage(MessageImportance.High, $"PenGenerate-Environment:{nameof(Environment.CommandLine)}={Environment.CommandLine}");
            Log.LogMessage(MessageImportance.High, $"PenGenerate-BuildEngine:{nameof(BuildEngine6.ProjectFileOfTaskNode)}={BuildEngine6.ProjectFileOfTaskNode}");
            foreach(var entry in BuildEngine6.GetGlobalProperties())
            {
                Log.LogMessage(MessageImportance.High, $"PenGenerate-BuildEngine:{entry.Key}={entry.Value}");
            }

            try
            {
                Generate.Execute(Namespace, InputDll, OutputDirectory);
                return true;
            }
            catch(Exception ex)
            {
                Log.LogErrorFromException(ex, true, true, null);
                return false;
            }
        }

        [Required]
        public string Namespace { get; set; }

        [Required]
        public string InputDll { get; set; }

        [Required]
        public string OutputDirectory { get; set; }
    }
}
