using System;
using System.Activities;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ImpresionEnvioMuestraAuditoria : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<CaladoDto> Calado { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDeOrden { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<string> PatenteAcoplado { get; set; }
        [RequiredArgument]
        public InArgument<string> Proveedor { get; set; }
        [RequiredArgument]
        public InArgument<string> ProveedorCuit { get; set; }
        [RequiredArgument]
        public InArgument<string> Entregador { get; set; }
        [RequiredArgument]
        public InArgument<string> EntregadorCuit { get; set; }
        [RequiredArgument]
        public InArgument<string> Corredor { get; set; }
        [RequiredArgument]
        public InArgument<string> CorredorCuit { get; set; }
        [RequiredArgument]
        public InArgument<string> UsuarioCalado { get; set; }
        [RequiredArgument]
        public InArgument<string> Procedencia { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        public InArgument<int?> CantCopias { get; set; }

        public OutArgument<bool> SeEnvioMuestraAuditoria { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var calado = Calado.Get<CaladoDto>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var proveedor = Proveedor.Get<string>(context);
            var proveedorcuit = ProveedorCuit.Get<string>(context);
            var entregador = Entregador.Get<string>(context);
            var entregadorcuit = EntregadorCuit.Get<string>(context);
            var corredor = Corredor.Get<string>(context);
            var corredorcuit = CorredorCuit.Get<string>(context);
            var procedencia = Procedencia.Get<string>(context);
            var usuarioCalado = UsuarioCalado.Get<string>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;

            var logActividad = new LogActividadDto { Actividad = "Impresion Muestra Auditoria", WorkflowInstanceId = workflowId };
            logActividad.Fecha = DateTime.Now;
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }
            try
            {
                var usuarioCompleto = repositorio.ObtenerUsuarioId(usuarioCalado);
                var nombreUsuario = usuarioCompleto.Nombre + " " + usuarioCompleto.Apellido + " " + usuarioCompleto.Matricula;

                var seEnvioMuestraAuditoria = false;
                var materialPorCentro = repositorio.ObtenerMaterialPorCentroPorInstanceId(workflowId);
                var random = repositorio.ObtenerNumeroAleatorio();
                if (random < (materialPorCentro.PorcentajeMuestraAuditoria ?? 0))
                {
                    try
                    {
                        var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                        if (documento == null)
                        {
                            throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo));
                        }

                        var dto = new ImpIdentificacionMuestraAuditoriaDto
                            {
                                Impresora = documento.ImpresoraDireccion ?? "",
                                Centro = documento.CentroDescripcion,
                                Codigo = codigo,
                                NumeroDeOrden = numeroDeOrden,
                                Patente = patente,
                                PatenteAcoplado = patenteAcoplado,
                                WorkflowId = workflowId,
                                FechaHoraCalado = calado.FechaCreacion != null ? calado.FechaCreacion.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                FechaImpresion = DateTime.Now,
                                ProveedorCuit = proveedorcuit,
                                Proveedor = proveedor,
                                NroMuestra = repositorio.ObtenerNumeroMuestraAuditoriaGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(10,'0'),
                                CorredorCuit = corredorcuit,
                                Corredor = corredor,
                                Procedencia = procedencia,
                                EntregadorCuit = entregadorcuit,
                                Entregador = entregador,
                                UsuarioCalado = nombreUsuario,
                                Material = materialPorCentro.MaterialDesc,
                                MaterialId = materialPorCentro.MaterialId
                            };

                        resultado = servicio.Ejecutar(new ImprimirMuestraAuditoria { Dto = dto, CantidadCopias = cantCopias });

                        seEnvioMuestraAuditoria = true;
                    }
                    catch (Exception e)
                    {
                        resultado.Errores.Add("1", e.Message);
                    }

                    try
                    {
                        resultado = servicio.Ejecutar(new ModificarRecorridoMuestraAuditoria { InstanceId = workflowId });
                    }
                    catch (Exception)
                    {
                        resultado.Errores.Add("2", Textos.Recorrido_ErrorAlTerminar);
                    }

                    SeEnvioMuestraAuditoria.Set(context, seEnvioMuestraAuditoria);
                }
            }
            catch
            {
                resultado.Errores.Add("3", Textos.Error_Generico);
            }

            return resultado;
        }
    }
}
