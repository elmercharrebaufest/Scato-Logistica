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
    public class ProcesadorCrearHojaDeRutaYerbatera : ProcesadorComando<CrearHojaDeRutaYerbatera>
    {
        public ProcesadorCrearHojaDeRutaYerbatera(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearHojaDeRutaYerbatera comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearHojaDeRutaYerbatera para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var proveedor = Repositorio.Obtener<Proveedor>(comando.Orden.ProveedorId);
                    var procedencia = Repositorio.Obtener<Localidad>(comando.Orden.ProcedenciaId);
                    var destinatario = Repositorio.Obtener<Proveedor>(comando.Orden.DestinatarioId);
                    var centroDestino = Repositorio.Obtener<Centro>(comando.Orden.CentroDestinoId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    Log.Debug("Se Obtuvieron los objetos del documento");
                    var hojaDeRutaYerbatera = new HojaDeRutaYerbatera
                        {
                            Proveedor = proveedor,
                            Procedencia = procedencia,
                            Destinatario = destinatario,
                            CentroDestino = centroDestino,
                            Transportista = transportista,
                            Chofer = chofer,
                            Material = material,
                            TipoComercial = tipoComercial,
                            Recorrido = new Recorrido
                                    {
                                        InstanciaWorkflow = comando.InstanciaWorkflowId,
                                        Usuario = comando.Usuario,
                                        FechaInicio = DateTime.Now,
                                        Workflow = workflow,
                                        Chofer = chofer,
                                        Centro = centro,
                                        Patente = comando.Orden.Patente,
                                        Transportista = transportista,
                                        TipoComercial = tipoComercial,
                                        TipoDocumentoIngreso = TipoDocumentoIngreso.HojaDeRutaYerbatera,
                                        Material = material,
                                        PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen,
                                        PesoTaraOrigen = comando.Orden.PesoTaraOrigen,
                                        NumeroDocumentoIngreso = comando.Orden.NroHojaDeRutaYerbatera,
                                        WorkflowDefinicion = workflowDefinicion,
                                        TipoVehiculo = comando.Orden.TipoVehiculo
                                    },
                            FechaCarga = comando.Orden.FechaCarga,
                            FechaEmision = comando.Orden.FechaEmision,
                            FechaVencimiento = comando.Orden.FechaVencimiento,
                            NroHojaDeRutaYerbatera = comando.Orden.NroHojaDeRutaYerbatera,
                            Patente = comando.Orden.Patente,
                            PatenteAcoplado = comando.Orden.PatenteAcoplado,
                            PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen,
                            PesoNetoOrigen = comando.Orden.PesoNetoOrigen,
                            PesoTaraOrigen = comando.Orden.PesoTaraOrigen,
                        };
                    
                    Log.Info("Se procederá a crear el recorrido para el workflow {0}", comando.NombreWorkflow);
                    Repositorio.Agregar(hojaDeRutaYerbatera);
                    Repositorio.GuardarCambios();
                    resultado.Id = (int)hojaDeRutaYerbatera.GetType().GetProperty("Id").GetValue(hojaDeRutaYerbatera, null);

                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear la hoja de ruta para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.CartaDePorte_Error);
            }
            return resultado;
        }
    }
}
