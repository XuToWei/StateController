using Sirenix.OdinInspector;

namespace StateController
{
    [LabelText("Boolean Mode")]
    public enum BooleanMode : byte
    {
        Single = 0,
        And = 1,
        Or = 2
    }
}
