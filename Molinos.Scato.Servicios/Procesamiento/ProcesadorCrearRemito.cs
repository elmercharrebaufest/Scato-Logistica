using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearRemito : ProcesadorComando<CrearRemito>
    {
        public ProcesadorCrearRemito(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearRemito comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearRemito para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var centroRecorrido = Repositorio.Obtener<Centro>(comando.CentroId);
                    var procedencia = Repositorio.Obtener<Localidad>(comando.Orden.ProcedenciaId);
                    var almacen = Repositorio.Obtener<Almacen>(comando.Orden.Almacen_Id);
                    var calle = Repositorio.Obtener<Calle>(comando.Orden.Calle_Id);
                    Proveedor proveedor = null;
                    Centro centro = null;
                    if (comando.Orden.EsRemitoProveedor)
                    {
                        proveedor = Repositorio.Obtener<Proveedor>(comando.Orden.OrigenId);
                    }
                    else
                    {
                        centro = Repositorio.Obtener<Centro>(comando.Orden.OrigenId);
                    }
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);

                    var recorrido = new Recorrido
                    {
                        InstanciaWorkflow = comando.InstanciaWorkflowId,
                        Usuario = comando.Usuario,
                        FechaInicio = DateTime.Now,
                        Workflow = workflow,
                        Chofer = chofer,
                        Centro = centroRecorrido,
                        Patente = comando.Orden.PatenteCamion,
                        Transportista = transportista,
                        TipoComercial = tipoComercial,
                        TipoDocumentoIngreso = TipoDocumentoIngreso.Remito,
                        NumeroDocumentoIngreso = comando.Orden.OrdenDeDescarga,
                        NumeroDocumentoIngresoLegal = (!string.IsNullOrEmpty(comando.Orden.Remito) ? comando.Orden.Remito.Replace('R','-') : ""),
                        WorkflowDefinicion = workflowDefinicion,
                        PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen,
                        PesoTaraOrigen = comando.Orden.PesoTaraOrigen,
                        Material = material,
                        TipoVehiculo = comando.Orden.TipoVehiculo,
                        Almacen = almacen,
                        Calle= calle
                    };
                    Repositorio.Agregar(recorrido);

                    var remito = new Remito
                    {
                        OrdenDeDescarga = comando.Orden.OrdenDeDescarga,
                        FechaOD = comando.Orden.FechaOD,
                        TipoComercial = tipoComercial,
                        Transportista = transportista,
                        Chofer = chofer,
                        OrdenRemito = comando.Orden.Remito,
                        PatenteCamion = comando.Orden.PatenteCamion,
                        PatenteAcoplado = comando.Orden.PatenteAcoplado,
                        CentroOrigen = centro,
                        DocLegalRemito = comando.Orden.DocLegalRemito,
                        ProveedorOrigen = proveedor,
                        Material = material,
                        AcuerdoMarco = comando.Orden.AcuerdoMarco,
                        CodigoAnexo = comando.Orden.CodigoAnexo,
                        PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen ?? 0,
                        PesoNetoOrigen = comando.Orden.PesoNetoOrigen ?? 0,
                        PesoTaraOrigen = comando.Orden.PesoTaraOrigen ?? 0,
                        Recorrido = recorrido,
                        KmRecorrer = comando.Orden.KmRecorrer,
                        Procedencia = procedencia,
                        Cosecha = comando.Orden.Cosecha,
                        CodEstab = comando.Orden.CodEstab
                    };

                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);

                    Repositorio.Agregar(remito);
                    Repositorio.GuardarCambios();
                    resultado.Id = comando.Orden.Id != 0 ? comando.Orden.Id : remito.Id;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear el Remito para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenDeDescarga_Error);
            }

            return resultado;
        }
    }
}
