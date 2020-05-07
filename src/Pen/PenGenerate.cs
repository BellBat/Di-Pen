using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace Pen
{
    public class PenGenerate : Task
    {
        public override bool Execute()
        {
            Generate.Execute(Namespace, InputDll, OutputDirectory);
            return true;
        }

        [Required]
        public string Namespace { get; set; }

        [Required]
        public string InputDll { get; set; }

        [Required]
        public string OutputDirectory { get; set; }
    }
}
