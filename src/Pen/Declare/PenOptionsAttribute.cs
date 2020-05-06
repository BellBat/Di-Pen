using System;
using System.Collections.Generic;
using System.Text;

namespace Pen.Declare
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class PenOptionsAttribute : Attribute
    {
        public PenOptionsAttribute(bool autoWire = false)
        {
            AutoWire = true;
        }

        public bool AutoWire { get; set; }
    }
}
