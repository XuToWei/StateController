using System;
using System.Diagnostics;

namespace StateController
{
    /// <summary>
    /// SelectableState序列化数据是否换行的特性
    /// </summary>
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