using System;
using System.Activities;
using System.Configuration;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    ///     Retorna True solo si el chofer existe y tanto el como el camion estan habilitados
    /// </summary>
    public class VerificarSalidaFlete : CodeActivity<bool>
    {
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }

        [RequiredArgument]
        public InArgument<int> ChoferId { get; set; }

        [RequiredArgument]
        public InArgument<int> TransportistaId { get; set; }
        public InArgument<int?> IntermediarioId { get; set; }
        public OutArgument<bool> SalidaVerificada { get; set; }
        public OutArgument<string> MensajeError { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            
            var patente = Patente.Get<string>(context);
            var choferId = ChoferId.Get<int>(context);
            var transportistaId = TransportistaId.Get<int>(context);
            var intermediarioId = IntermediarioId.Get<int>(context);
            try
            {
                ChoferDto chofer = repositorio.ObtenerChofer(choferId);
                TransportistaDto transportista = repositorio.ObtenerTransportista(transportistaId);
                ProveedorDto intermediario = repositorio.ObtenerProveedor(intermediarioId);
                string cuit = intermediario != null ? intermediario.Cuil : transportista.Cuit;

                if (chofer.NumeroDeDocumento.Length == 7)
                {
                    chofer.NumeroDeDocumento = "0" + chofer.NumeroDeDocumento;
                }
                ComplianceV2(context, cuit, chofer, patente);
            }
            catch (Exception ex)
            {
                MensajeError.Set(context, MensajeError.Get(context) + "--(EX:" + ex.Message +")--" + Textos.VerificacionSalidaFlete_Error);
                SalidaVerificada.Set(context, false);
            }
            return SalidaVerificada.Get<bool>(context);
        }

        private void ComplianceV2(CodeActivityContext context, string cuit, ChoferDto chofer, string patente)
        {
            var servicioCompliance = context.GetExtension<DatosPort>();

            controlarDatosAgroacopiosRequest datosGranelesRequest = new controlarDatosAgroacopiosRequest()
            {
                datos = new Datos() {
                    cuit = cuit,
                    dni = chofer.NumeroDeDocumento,
                    patente1 = patente,
                    planta = context.GetExtension<ScatoPersistenceParticipant>().CentroId.ToString()
                }
            };
            MensajeError.Set(context, MensajeError.Get(context) + "----" + "1");
            try
            {
                var r = datosGranelesRequest.ToXml();
                MensajeError.Set(context, MensajeError.Get(context) + "----" + r);
                if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                {
                    var srv = context.GetExtension<IServicioComandos>();
                    srv.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "VerificarSalidaFleteRequest",
                            Fecha = DateTime.Now,
                            Comentario = r,
                            NombreUsuario = "",
                            WorkflowInstanceId = context.WorkflowInstanceId,
                        }
                    });
                }
            }
            catch (Exception e)
            {
            }
            MensajeError.Set(context, MensajeError.Get(context) + "----" + "2");
            var respuesta = servicioCompliance.controlarDatosAgroacopios(datosGranelesRequest);
            MensajeError.Set(context, MensajeError.Get(context) + "----" + "3");
            try
            {
                var r = respuesta.ToXml();
                MensajeError.Set(context, MensajeError.Get(context) + "----" + r);
                if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                {
                    var srv = context.GetExtension<IServicioComandos>();
                    srv.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "VerificarSalidaFleteResponse",
                            Fecha = DateTime.Now,
                            Comentario = r,
                            NombreUsuario = "",
                            WorkflowInstanceId = context.WorkflowInstanceId,
                        }
                    });
                }
            }
            catch (Exception e)
            {
            }

            if (respuesta.controlarDatosReturn.codigoError != 0 || respuesta.controlarDatosReturn.colorEmpresa == 1 ||
                respuesta.controlarDatosReturn.colorChofer == 1 ||
                respuesta.controlarDatosReturn.colorVehiculo1 == 1 || respuesta.controlarDatosReturn.colorVehiculo2 == 1)
            {
                SalidaVerificada.Set(context, false);

                MensajeError.Set(context,
                    patente + ": " +
                    ObtenerDescripcionMensaje(respuesta.controlarDatosReturn));
            }
            else
            {
                SalidaVerificada.Set(context, true);
            }
        }

        private string ObtenerDescripcionMensaje(Response respuesta)
        {
            string mensaje = string.Empty;
            if (respuesta.codigoError != 0)
            {
                switch (respuesta.codigoError)
                {
                    case 1:
                        mensaje = "No existe el vehículo T";
                        break;
                    case 2:
                        mensaje = "No existe el vehículo A";
                        break;
                    case 3:
                        mensaje = "El Vehiculo T no pertenece a la empresa";
                        break;
                    case 4:
                        mensaje = "El Vehiculo A no pertenece a la empresa";
                        break;
                    case 5:
                        mensaje = "El vehículo T es un A";
                        break;
                    case 6:
                        mensaje = "El vehículo A es un T";
                        break;
                    case 7:
                        mensaje = "El chofer no existe";
                        break;
                    case 8:
                        mensaje = "El chofer no pertenece a la empresa";
                        break;
                    case 9:
                        mensaje = "La empresa no existe";
                        break;
                    case 10:
                        mensaje = "10";
                        break;
                    case 11:
                        mensaje = "11";
                        break;
                    case 12:
                        mensaje = "12";
                        break;
                    case 13:
                        mensaje = "Chofer bloquedo por AVL";
                        break;
                    case 14:
                        mensaje = "Unidad Tractor bloqueada por AVL";
                        break;
                    case 15:
                        mensaje = "Unidad Acoplado bloqueada por AVL";
                        break;
                    case 16:
                        mensaje = "Chofer bloqueado por empresa subcontratista";
                        break;
                    case 17:
                        mensaje = "Unidad Tractor bloqueada por empresa subcontratista";
                        break;
                    case 18:
                        mensaje = "Unidad Acoplado bloqueada por empresa subcontratista";
                        break;
                    case 20:
                        mensaje = "Error de conexión";
                        break;
                }
                mensaje +=" ";
            }

            if (respuesta.colorChofer == 1)
            {
                mensaje += "El chofer se encuentra inhabilitado en Web Compliance";
                mensaje += " ";
            }
            if (respuesta.colorEmpresa == 1)
            {
                mensaje += "La empresa se encuentra inhabilitada en Web Compliance";
                mensaje += " ";
            }
            if (respuesta.colorVehiculo1 == 1 || respuesta.colorVehiculo2 == 1)
            {
                mensaje += "El vehículo se encuentra inhabilitado en Web Compliance";
            }
            return mensaje;
        }
    }
}