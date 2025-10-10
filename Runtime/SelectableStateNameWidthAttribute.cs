using System;
using System.Diagnostics;

namespace StateController
{
    /// <summary>
    /// SelectableState名字宽度的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public class SelectableStateNameWidthAttribute : Attribute
    {
        public float Width { get; private set; }
        
        public SelectableStateNameWidthAttribute(float width)
        {
            Width = width;
        }
    }
}