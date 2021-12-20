using System;
using System.Web.Mvc;

namespace Molinos.Scato.Web
{
    public sealed class CultureAwareQueryStringValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            return new CultureAwareQueryStringValueProvider(controllerContext);
        }
    }
}