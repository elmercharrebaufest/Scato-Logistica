using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresosEgresosFazonesGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<decimal> PesoNeto { get; set; }
        public InArgument<string> NumeroDocumento { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaIngreso { get; set; }
        public InArgument<decimal?> Km { get; set; }
        public InArgument<int?> LocalidadId { get; set; }
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> ClienteCodigoSap { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        public InArgument<int?> ProvinciaId { get; set; }
        [RequiredArgument]
        public InArgument<string> TipoMovimiento { get; set; }
        [RequiredArgument]
        public InArgument<int> TransportistaId { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        public InArgument<string> ComisionistaCodigoSap { get; set; }
        public InArgument<string> RemitenteCodigoSap { get; set; }
        public OutArgument<IngresosEgresosFazonesRequest> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            IngresosEgresosFazonesRequest request = null;
            try
            {
                var instanceId = InstanceId.Get<Guid>(context);
                var fechaIngreso = FechaIngreso.Get<DateTime>(context);
                var cliente = ClienteCodigoSap.Get<string>(context);
                var comisionista = ComisionistaCodigoSap.Get<string>(context);
                var remitente = RemitenteCodigoSap.Get<string>(context);
                var centroId = context.GetExtension<ScatoPersistenceParticipant>().CentroId ?? CentroId.Get<int>(context);
                var materialId = MaterialId.Get<int>(context);
                var patente = Patente.Get<string>(context);
                var tipoMovimiento = TipoMovimiento.Get<string>(context);
                var patenteAcoplado = PatenteAcoplado.Get<string>(context);
                var km = Km.Get<decimal?>(context);
                var pesoNeto = PesoNeto.Get<decimal>(context);
                var nroDocumento = NumeroDocumento.Get<string>(context);
                if (!String.IsNullOrEmpty(nroDocumento) && nroDocumento.Length == 12 && nroDocumento[4] != '-')
                {
                    nroDocumento = nroDocumento.Substring(0, 4) + "-" + nroDocumento.Substring(4, 8);
                }

                var recorrido = srvRepositorio.ObtenerRecorridoPorGuid(instanceId);
                var intermediarioFleteId = srvRepositorio.ObtenerOrdenCargaInternaFasonPorInstanceId(instanceId)?.IntermediarioFleteId;

                AlmacenDto almacen = null;

                var asignacion = srvRepositorio.ObtenerAsignacionDePuestoComando(instanceId.ToString("D"));
                almacen = srvRepositorio.ObtenerAlmacen(asignacion.AlmacenId);

                if(!almacen.EsSojaEPA && !almacen.EsSojaSustentable)
                {
                    if (srvRepositorio.MaterialEnviaASapAlmacenPredeterminado(instanceId))
                    {
                        almacen = srvRepositorio.ObtenerAlmacenPredeterminado(centroId, materialId);
                    }
                }

                var provincia = srvRepositorio.ObtenerProvincia(ProvinciaId.Get<int?>(context) ?? 0);
                var chofer = recorrido.Chofer;
                var orden = srvRepositorio.ObtenerDatoDerivadoGranarioPorRecorridoTipoDocumento(recorrido.Id, recorrido.TipoDocumentoIngreso);
                request = new IngresosEgresosFazonesRequest
                    {
                        IngresosEgresosFazones = new IngresosEgresosFazones
                            {
                                Almacen = almacen.CodigoSAP,
                                Cantidad = pesoNeto.ToString(CultureInfo.InvariantCulture),
                                DocLegal = tipoMovimiento == "ENT" && nroDocumento == Constantes.ValoresPorDefecto.NumeroRemitoGenerico && orden != null ? orden.NroCTG : nroDocumento,
                                FechaIng = fechaIngreso.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                Km = km ?? 0,
                                KmSpecified = km.HasValue,
                                Destino = tipoMovimiento == "ENT" ? recorrido.Centro.CodigoSAP : (!string.IsNullOrEmpty(comisionista) ? comisionista : (!string.IsNullOrEmpty(remitente) ? remitente : cliente)),
                                Material = recorrido.Material.CodigoSAP,
                                Patente = patente,
                                Procedencia = tipoMovimiento == "ENT" ? cliente : recorrido.Centro.CodigoSAP,
                                ProvinciaOrig = provincia != null ? provincia.CodigoAfip.ToString(CultureInfo.InvariantCulture) : string.Empty,
                                TipoMov = tipoMovimiento,
                                Transportista = recorrido.Transportista.Cuit.Replace("-", ""),
                                UniMedCant = recorrido.Material.UnidadDeMedidad,
                                NombreChofer = chofer.NombreCompleto,
                                NroDocumento = chofer.NumeroDeDocumento,
                                TipoDoc = chofer.TipoDocumentoIdentidadCodigoSap,
                                Patente2 = patenteAcoplado,
                                IM_NUM_SCATO = recorrido.Id.ToString(CultureInfo.InvariantCulture) ,
                                IM_CTG = orden != null ? orden.NroCTG : string.Empty,
                                IM_CPEDG =  orden != null ? orden.Sucursal + orden.NroOrden : string.Empty,
                        }
                    };

                if(intermediarioFleteId != null)
                {
                    var cuitIntermediario = srvRepositorio.ObtenerProveedor(intermediarioFleteId.GetValueOrDefault()).Cuil;
                    request.IngresosEgresosFazones.Transportista = cuitIntermediario.Replace("-", "");
                }

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "IngresosEgresosFazonesGenerarRequest",
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
