using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace Molinos.Scato.Actividades.Behaviour
{
    /// <summary>
    /// Behaviour custom para registrar las propiedades extra que se necesitan para el tracking de los workflows. Se utiliza el mecanismo property promotion de WF.
    /// </summary>
    public class PromotePropertiesBehavior : IServiceBehavior
    {
        private readonly XNamespace xNs = XNamespace.Get("http://scato.molinos.com/PropiedadesCustom");

        public virtual void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                                 Collection<ServiceEndpoint> endpoints,
                                                 BindingParameterCollection bindingParameters)
        {
        }

        public virtual void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var host = serviceHostBase as WorkflowServiceHost;
            if (host != null)
            {
                var store =
                    host.Description.Behaviors[typeof (SqlWorkflowInstanceStoreBehavior)] as
                    SqlWorkflowInstanceStoreBehavior;

                if (store != null)
                {
                    var nuevasPropiedades = new List<XName>
                        {
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadPatente),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadMaterialId),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadMaterial),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadTransportista),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadCuit),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadCalidad),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadTipoDocumentoDeIngreso),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadNumeroDocumentoDeIngreso),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadWorkflow),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadCentroId),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadCentroCodigoSap),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadCentro),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadMaterialCodigoSap),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadTipoVehiculo),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadActividad),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadProcedencia),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadEsGrano),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadEntregador),
                            xNs.GetName(ScatoPersistenceParticipant.PropiedadEsRechazado),
                        };

                    store.Promote("DatosWorkflow", nuevasPropiedades, null);
                    host.WorkflowExtensions.Add (() => new ScatoPersistenceParticipant());
                }
            }
        }

        public virtual void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}