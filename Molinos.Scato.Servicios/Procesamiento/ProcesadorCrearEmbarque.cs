using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEmbarque : ProcesadorComando<CrearEmbarque>
    {
        public ProcesadorCrearEmbarque(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearEmbarque comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorIngresarOrdenCargaInternaFason para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var vapor = Repositorio.Obtener<Vapor>(x => x.Nombre == comando.Embarque.NombreBuque) ?? new Vapor
                    {
                        Nombre = comando.Embarque.NombreBuque
                    };
                    var centro = Repositorio.Obtener<Centro>(comando.Embarque.CentroId);

                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var chofer = Repositorio.Obtener<Chofer>(x => x.Nombre == "Capitan") ?? 
                        Repositorio.Agregar(new Chofer{
                            Nombre = "Capitan",
                            Apellido = "Capitan",
                            NumeroDeDocumento = "30999999",
                            Cuil = "20-30999999-1",
                            TipoDocumentoIdentidad = Repositorio.Obtener<TipoDocumentoIdentidad>(1)
                        });
                    var recorrido = new Recorrido { InstanciaWorkflow = comando.InstanciaWorkflowId, Usuario = comando.Usuario,
                        Workflow = workflow, Centro = centro, Patente = comando.Embarque.Patente,
                        TipoDocumentoIngreso = TipoDocumentoIngreso.Embarque,
                        NumeroDocumentoIngreso = "1", FechaInicio = DateTime.Now,
                        WorkflowDefinicion = workflowDefinicion, TipoVehiculo = TipoVehiculo.Vapor,
                        Chofer = chofer, TipoComercial = Repositorio.Obtener<TipoComercial>(x => x.CodigoSap == "107")};

                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);
                    var embarque = new Embarque
                    {
                        Vapor = vapor,
                        FechaRecalada = comando.Embarque.FechaRecalada,
                        HoraRecalada = comando.Embarque.HoraRecalada,
                        Coordinadores = (comando.Embarque.Coordinadores != null)
                            ? Repositorio.Obtener<CoordinadorPuerto>(comando.Embarque.Coordinadores.Id) : null,
                        Agencias = (comando.Embarque.Agencias != null)
                            ? Repositorio.Obtener<AgenciaMaritimaPuerto>(comando.Embarque.Agencias.Id) : null,
                        ObligacionCarga = comando.Embarque.ObligacionCarga,
                        Senasa = comando.Embarque.Senasa,
                        Observaciones = comando.Embarque.Observaciones,
                        Vicentin = comando.Embarque.Vicentin,
                        Noryon = comando.Embarque.Noryon,
                        OtrosMuelles = comando.Embarque.OtrosMuelles,
                        Recorrido = recorrido,
                        Centro = centro,
                        Patente = comando.Embarque.Patente,
                        TipoBuque = comando.Embarque.TipoDeBuque != null ? comando.Embarque.TipoDeBuque.Nombre : "",
                        Freeboard = comando.Embarque.Freeboard,
                        Ubicacion = comando.Embarque.UbicacionDeBuque != null ? comando.Embarque.UbicacionDeBuque.Id : 0,
                        SanBenito = comando.Embarque.SanBenito,
                        ATA = (comando.Embarque.ATA != null)
                            ? Repositorio.Obtener<ATAPuerto>(comando.Embarque.ATA.Id) : null,
                        EsLiquido = comando.Embarque.EsLiquido,
                        //HORAS A LA ESPERA DE LIMPIEZA
                        FechaDesdeLimpieza = comando.Embarque.FechaDesdeLimpieza,
                        HoraDesdeLimpieza = comando.Embarque.HoraDesdeLimpieza,
                        FechaHastaLimpieza = comando.Embarque.FechaHastaLimpieza,
                        HoraHastaLimpieza = comando.Embarque.HoraHastaLimpieza,
                        MotivosLimpieza = (comando.Embarque.MotivosLimpieza != null)
                            ? Repositorio.Obtener<MotivosLimpieza>(comando.Embarque.MotivosLimpieza.Id) : null,
                        ObservacionesLimpieza = comando.Embarque.ObservacionesLimpieza,
                        //SHIP PARTICULAR
                        Destino = (comando.Embarque.Destino != null)
                            ? Repositorio.Obtener<Destino>(comando.Embarque.Destino.Id) : null,
                        PorteNeto = comando.Embarque.PorteNeto,
                        PorteBruto = comando.Embarque.PorteBruto,
                        Eslora = comando.Embarque.Eslora,
                        Manga = comando.Embarque.Manga,
                        Puntal = comando.Embarque.Puntal,
                        FechaLibrePlatica = comando.Embarque.FechaLibrePlatica,
                        HoraLibrePlatica = comando.Embarque.HoraLibrePlatica,
                    };
                    foreach (var mat in comando.Embarque.MaterialesPuertoCantidad.Where(y => y.Cantidad > 0))
                    {
                        Repositorio.Agregar(new MaterialPuertoCantidad
                        {
                            Cantidad = mat.Cantidad,
                            MaterialPuerto = Repositorio.Obtener<MaterialPuerto>(mat.MaterialId),
                            Embarque = embarque,
                            Color = mat.Color
                        });
                    }
                    Repositorio.Agregar(embarque);
                    Repositorio.GuardarCambios();
                    resultado.Id = (int)embarque.GetType().GetProperty("Id").GetValue(embarque, null);
                    recorrido.NumeroDocumentoIngreso = resultado.Id.ToString();
                    Repositorio.GuardarCambios();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear la orden de carga interna fason para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenCargaInterna_Error);
            }

            return resultado;
        }
    }
}
