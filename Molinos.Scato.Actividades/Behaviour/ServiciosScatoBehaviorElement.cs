using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Molinos.Scato.Actividades.Behaviour
{
    /// <summary>
    /// Elemento de configuración que permite agregar el behaviour custom. Ver <see cref="ServiciosScatoBehavior"/>
    /// </summary>
    public class ServiciosScatoBehaviorElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override Type BehaviorType
        {
            get { return typeof(ServiciosScatoBehavior); }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (properties == null)
                {
                    var props = new ConfigurationPropertyCollection();
                    properties = props;
                }
                return properties;
            }
        }

        protected override object CreateBehavior()
        {
            var behavior = new ServiciosScatoBehavior();
            return behavior;
        }
    }
}