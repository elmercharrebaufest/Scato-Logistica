using System;
using System.Reflection;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Filtros
{
    public sealed class HttpParamActionAttribute : ActionNameSelectorAttribute
    {
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            return ActionNameIsMethodName(actionName, methodInfo)
                   || ActionNameIsInParam(controllerContext, actionName, methodInfo);
        }

        private bool ActionNameIsInParam(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            return String.Equals(actionName, "Action", StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(controllerContext.RequestContext.HttpContext.Request["submitAction"], methodInfo.Name, StringComparison.OrdinalIgnoreCase);
        }

        private static bool ActionNameIsMethodName(string actionName, MethodInfo methodInfo)
        {
            return String.Equals(actionName, methodInfo.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

}