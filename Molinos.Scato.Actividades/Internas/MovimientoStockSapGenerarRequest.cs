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
    public class MovimientoStockSapGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<int> Cantidad { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroCosteId { get; set; }
        [RequiredArgument]
        public InArgument<string> ClaseExp { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaContab { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaDoc { get; set; }
        [RequiredArgument]
        public InArgument<string> NroDocumento { get; set; }

        public OutArgument<MovAjuste> Request { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var esCPE = srvRepositorio.ObtenerSiEsCPEporWf(InstanceId.Get<Guid>(context));
            var centroId = CentroId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var patente = Patente.Get<string>(context);
            var cantidad = Cantidad.Get<int>(context);
            var centroCosteId = CentroCosteId.Get<int>(context);
            var claseExp = ClaseExp.Get<string>(context);
            var fechaContab = FechaContab.Get<DateTime>(context);
            var fechaDoc = FechaDoc.Get<DateTime>(context);
            var nroDocumento = NroDocumento.Get<string>(context);

            if(esCPE.HasValue && esCPE.Value)
            {
                if (nroDocumento.PadLeft(12, '0').IndexOf("-", StringComparison.Ordinal) == -1) {
                    nroDocumento = nroDocumento.PadLeft(12, '0').Substring(0, 4) + "-" + nroDocumento.PadLeft(12, '0').Substring(4, 8);
                }
            } else if (nroDocumento.Length == 12 && nroDocumento[4] != '-')
            {
                nroDocumento = nroDocumento.Substring(0, 4) + "-" + nroDocumento.Substring(4, 8);
            }

            var asignacion = srvRepositorio.ObtenerAsignacionDePuestoComando(InstanceId.Get<Guid>(context).ToString("D"));
            var almacen = srvRepositorio.ObtenerAlmacen(asignacion.AlmacenId);
            var centro = srvRepositorio.ObtenerCentro(centroId);
            var material = srvRepositorio.ObtenerMaterial(materialId);
            var centroCoste = srvRepositorio.ObtenerCentro(centroCosteId);

            var almacenSap = almacen != null ? almacen.CodigoSAP : "";
            var centroSap = centro != null ? centro.CodigoSAP : "";
            var materialSap = material != null ? material.CodigoSAP : "";
            var centroCosteSap = centroCoste != null ? centroCoste.CodigoSAP : "";
            var uniMed = material != null ? material.UnidadDeMedidad : "";
            var request = new MovAjuste
            {
                Almacen = almacenSap,
                Centro = centroSap,
                Material = materialSap,
                Patente = patente,
                Cantidad = cantidad.ToString(CultureInfo.InvariantCulture),
                CeCo = centroCosteSap,
                ClaseExpedicion = claseExp.ToString(CultureInfo.InvariantCulture),
                FechaContab = fechaContab.ToString("yyyy-MM-dd"),
                FechaDoc = fechaDoc.ToString("yyyy-MM-dd"),
                NroDocumento = nroDocumento,
                UniMed = uniMed
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
                                    Actividad = "MovimientoStockSapGenerarRequest",
                                    Fecha = DateTime.Now,
                                    Comentario = request.ToXml(),
                                    NombreUsuario = "",
                                    WorkflowInstanceId = context.WorkflowInstanceId,
                                }
                        });
                }
            }
            catch
            {
            }
                
            

            Request.Set(context,request);
        }
    }
}
