using System;
using System.Diagnostics;

namespace StateController
{
    /// <summary>
    /// State 名字宽度的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public class StateNameWidthAttribute : Attribute
    {
        public float Width { get; private set; }

        public StateNameWidthAttribute(float width)
        {
            Width = width;
        }
    }
}
