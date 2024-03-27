using System;
using System.Diagnostics;

namespace StateController
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class SelectableStateDrawSettingAttribute : Attribute
    {
        public bool LineBreak { get; private set; }
        
        public SelectableStateDrawSettingAttribute(bool lineBreak)
        {
            LineBreak = lineBreak;
        }
    }
}