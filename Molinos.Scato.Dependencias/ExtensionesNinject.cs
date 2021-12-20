using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Molinos.Scato.Servicios;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

namespace Molinos.Scato.Dependencias
{
    public static class ExtensionesNinject
    {
        internal static void BindChannelFactory<TChannel>(this NinjectModule module, string endpointConfigurationName, string userName = null, string password = null, IEndpointBehavior behavior = null)
        {
            module.Bind<ChannelFactory<TChannel>>()
                .ToMethod(context => CreateChannelFactory<TChannel>(endpointConfigurationName, userName, password, behavior))
                .InSingletonScope()
                .OnDeactivation(CloseCommunicationObject);

            module.Bind<TChannel>()
                .ToMethod(context => context.Kernel.Get<ChannelFactory<TChannel>>().CreateChannel())
                .InRequestScope()
                .OnDeactivation(channel => CloseCommunicationObject((ICommunicationObject)channel));
        }

        internal static void BindWorkflowChannelFactory<TChannel>(this NinjectModule module)
        {
            module.Bind<ChannelFactory<TChannel>>()
                .ToMethod(context => new ChannelFactory<TChannel>(new BasicHttpBinding("CommonBinding")))
                .InSingletonScope()
                .OnDeactivation(CloseCommunicationObject);

            module.Bind<TChannel>()
                .ToMethod(context =>
                    {
                        var uri = (string) context.Parameters.First(p => p.Name == "uri").GetValue(context, null);
                        return context.Kernel.Get<ChannelFactory<TChannel>>().CreateChannel(new EndpointAddress(uri));
                    })
                .InRequestScope()
                .OnDeactivation(channel => CloseCommunicationObject((ICommunicationObject)channel));
        }

        private static ChannelFactory<TChannel> CreateChannelFactory<TChannel>(string endpointConfigurationName, string userNameSetting,
                                                                     string passwordSetting, IEndpointBehavior behavior)
        {
            var factory = new ChannelFactory<TChannel>(endpointConfigurationName);
            if (userNameSetting != null)
            {
                factory.Credentials.UserName.UserName = ConfigurationManager.AppSettings[userNameSetting];
                factory.Credentials.UserName.Password = Encriptador.Decrypt(ConfigurationManager.AppSettings[passwordSetting]);
            }
            if (behavior != null)
            {
                factory.Endpoint.Behaviors.Add(behavior);
            }
            return factory;
        }

        private static void CloseCommunicationObject(ICommunicationObject comObject)
        {
            try
            {
                if (comObject.State != CommunicationState.Faulted)
                {
                    comObject.Close();
                }
            }
            catch
            {
                try
                {
                    comObject.Abort();
                }
                catch
                {
                }
            }
        }
    }
}
