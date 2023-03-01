using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class EgresosMaterialNoProductivoGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        public InArgument<int> TransportistaId { get; set; }
        public InArgument<int> ChoferId { get; set; }
        public InArgument<string> Patente { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        public InArgument<int> ClienteId { get; set; }
        public InArgument<decimal> PesoNeto { get; set; }
        public InArgument<DateTime> Fecha { get; set; }

        public OutArgument<EgresosNoProductivosRequest> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            EgresosNoProductivosRequest request = null;
            try
            {
                var centroId = CentroId.Get<int>(context);
                var transportistaId = TransportistaId.Get<int>(context);
                var materialId = MaterialId.Get<int>(context);
                var clienteId = ClienteId.Get<int>(context);
                var choferId = ChoferId.Get<int>(context);
                var patente = Patente.Get<string>(context);
                var patenteAcolado = PatenteAcoplado.Get<string>(context);
                var fecha = Fecha.Get<DateTime>(context);

                var pesoNeto = PesoNeto.Get<decimal>(context);

                var centro = srvRepositorio.ObtenerCentro(centroId);
                var chofer = srvRepositorio.ObtenerChofer(choferId);
                var transportista = srvRepositorio.ObtenerTransportista(transportistaId);
                var material = srvRepositorio.ObtenerMaterial(materialId);
                var asignacion = srvRepositorio.ObtenerAsignacionDePuestoComando(InstanceId.Get<Guid>(context).ToString("D"));
                var almacen = srvRepositorio.ObtenerAlmacen(asignacion.AlmacenId);
                var cliente = srvRepositorio.ObtenerCliente(clienteId);
                var cartaPorteDerivadoGranario = srvRepositorio.ObtenerCartaPorteDerivadoGranarioPorGuid(context.WorkflowInstanceId);

                var centroSap = centro != null ? centro.CodigoSAP : "";
                var transportistaSap = transportista != null ? transportista.Cuit.Replace("-", string.Empty) : "";
                var clienteSap = cliente != null ? cliente.CodigoSap : "";

                request = new EgresosNoProductivosRequest
                    {
                        EgresosNoProductivos = new EgresosNoProductivos
                            {
                                Cliente = clienteSap,
                                Fecha = fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                PuestoExp = "",
                                PesoTotal = pesoNeto,
                                Transportista = transportistaSap,
                                UnidadPeso = material.UnidadDeMedidad,
                                DocChofer = chofer.NumeroDeDocumento,
                                TipoDocChofer = chofer.TipoDocumentoIdentidadCodigoSap,
                                NomChofer = chofer.NombreCompleto,
                                PatCamion = patente,
                                PatRemolque = !String.IsNullOrEmpty(patenteAcolado) ? patenteAcolado : patente,
                                IM_CPEDG = cartaPorteDerivadoGranario != null ? ($"{cartaPorteDerivadoGranario.Sucursal}{cartaPorteDerivadoGranario.NroOrden}") : string.Empty,
                                IM_CTG = cartaPorteDerivadoGranario != null ? cartaPorteDerivadoGranario.NroCTG : string.Empty,
                                Posiciones = new[]
                                    {
                                        new ZSDES9903
                                            {
                                                DESCRIPCION = material.Descripcion,
                                                CENTRO = centroSap,
                                                ALMACEN = almacen.CodigoSAP,
                                                CANTIDAD = pesoNeto,
                                                UNIDAD = material.UnidadDeMedidad,
                                                UNIDAD_PESO_ITEM = material.UnidadDeMedidad,
                                                PESO = pesoNeto
                                            }
                                    },
                            }
                    };


                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "EgresosMaterialNoProductivoGenerarRequest",
                                        Fecha = DateTime.Now,
                                        Comentario = request.ToXml(),
                                        NombreUsuario = "",
                                        WorkflowInstanceId = context.WorkflowInstanceId,
                                    }
                            });
                    }
                }
                catch (Exception e)
                {
                    resultado.Errores.Add("ControlRecorrido", e.Message);
                }
                

            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);
                
            }
            Request.Set(context,request);
            Resultado.Set(context, resultado);
        }
    }
}
