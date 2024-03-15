using System.Text;

namespace StateController
{
    public static class CodeGenerator
    {
        /// <summary>
        /// 获得CodeBind代码
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="controllerName">控制器名</param>
        /// <param name="prefix">通配名</param>
        /// <param name="tab">制表符</param>
        /// <returns>代码字符串</returns>
        public static string GetCodeBindString(StateController controller, string controllerName, string prefix, string tab)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var dataName in controller.EditorGetAllDataNames())
            {
                stringBuilder.AppendLine($"{tab}public StateController.StateControllerData m_{dataName}{prefix};");
                stringBuilder.AppendLine($"{tab}public StateController.StateControllerData {dataName}{prefix}");
                stringBuilder.AppendLine($"{tab}{{");
                stringBuilder.AppendLine($"{tab}    if (m_{dataName}{prefix} == null)");
                stringBuilder.AppendLine($"{tab}    {{");
                stringBuilder.AppendLine($"{tab}        m_{dataName}{prefix} = {controllerName}.GetData(\"{dataName}\");");
                stringBuilder.AppendLine($"{tab}    }}");
                stringBuilder.AppendLine($"{tab}    return m_{dataName}{prefix};");
                stringBuilder.AppendLine($"{tab}}}");
                stringBuilder.AppendLine($"{tab}public static class {dataName}StateName");
                stringBuilder.AppendLine($"{tab}{{");
                foreach (var stateName in controller.EditorGetData(dataName).EditorStateNames)
                {
                    stringBuilder.AppendLine($"{tab}    public const System.string {stateName} = {stateName};");
                }
                stringBuilder.AppendLine($"{tab}}}");
            }
            return stringBuilder.ToString();
        }
    }
}