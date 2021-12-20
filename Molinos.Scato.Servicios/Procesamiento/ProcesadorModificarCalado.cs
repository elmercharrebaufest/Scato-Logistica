using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCalado : ProcesadorComando<ModificarCalado>
    {
        private ICalculadoraDescuento calculadora;

        public ProcesadorModificarCalado(IRepositorio repositorio, IConversor conversor, ILogger log,
                                         ICalculadoraDescuento calculadora)
            : base(repositorio, conversor, log)
        {
            this.calculadora = calculadora;
        }

        public override Resultado Ejecutar(ModificarCalado comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Debug("{0} ModificarCalado",comando.Dto.WorkflowInstanceId);
                var caladoEditado = Repositorio.Obtener<Calado>(comando.Dto.Id);
                
                //Quito caladosPorCaracteristicas que pueden haber quedado de una ejecucion fallida de la actividad
                Log.Debug("{0} ModificarCalado Elimino posibles caladosPorCaract fallidos", comando.Dto.WorkflowInstanceId);
                var caladosABorrar = caladoEditado.CaladosPorCaracteristica.ToList();
                for (var index = 0; index < caladosABorrar.Count; index++)
                {
                    var caladoPorCaracteristica = caladosABorrar[index];
                    Repositorio.Remover(caladoPorCaracteristica);
                }
                caladoEditado.CaladosPorCaracteristica.Clear();

                var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == caladoEditado.WorkflowInstanceId);
                var estado = recorrido.CaracteristicasAnalizadas;
                if (estado == null)
                {
                    estado = new CaracteristicasAnalizadas();
                    estado.Recorrido = recorrido;
                }
                var tieneEntregador = recorrido != null && recorrido.Vehiculo != null && recorrido.Vehiculo.CartaPorte.Entregador != null && recorrido.Vehiculo.CartaPorte.Entregador.Id != 66;

                Log.Debug("{0} ModificarCalado Agrego los nuevos calados por caracteristicas", comando.Dto.WorkflowInstanceId);
                if (comando.CaladosPorCaracteristica.Length > 0)
                {
                    foreach (var caladoPorCaracteristicaDto in comando.CaladosPorCaracteristica)
                    {
                        var entidad = CrearEntidad(caladoPorCaracteristicaDto, comando.PesoNetoOrigen, comando.Dto.WorkflowInstanceId);
                        caladoEditado.CaladosPorCaracteristica.Add(entidad);
                        Log.Debug("{0} ModificarCalado Caracteristica {3} Valor: {1} Descuento: {2}", comando.Dto.WorkflowInstanceId,entidad.ValorCalado, entidad.DescuentoEnPorcentaje, entidad.CaracteristicaDeCalidad.Id);
                    }
                    caladoEditado.FechaCreacion = DateTime.Now;
                    calculadora.ActualizarMermaVolatil(caladoEditado, null, comando.PesoNetoOrigen);
                    calculadora.ActualizarEstado(caladoEditado, null, estado, tieneEntregador);
                    if (estado.Id == 0)
                    {
                        Repositorio.Agregar(estado);
                    }
                }
                Log.Debug("{0} ModificarCalado mapeo valores adicionales", comando.Dto.WorkflowInstanceId);
                caladoEditado.CicloDeCalado = comando.Dto.CicloDeCalado;
                caladoEditado.MuestraConjunto = comando.Dto.MuestraConjunto;
                caladoEditado.NumeroOrden = comando.Dto.NumeroOrden;
                caladoEditado.FechaCreacion = comando.Dto.FechaCreacion;
                caladoEditado.Usuario = comando.Dto.Usuario;
                caladoEditado.CalidadMaterial = Repositorio.Obtener<CalidadMaterial>(comando.Dto.CalidadMaterialId);
                caladoEditado.Comentario = comando.Dto.Comentario;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error en la carga de Calado");
                resultado.Error("", Textos.Calado_ErrorEnLaCarga);
            }

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            
            return resultado;
        }

        protected CaladoPorCaracteristica CrearEntidad(CaladoPorCaracteristicaDto caladoPorCaracteristica, int pesoNetoOrigen, Guid instanceId)
        {
            var caladoPorCaracteristicaEditado =
                Conversor.Convertir<CaladoPorCaracteristicaDto, CaladoPorCaracteristica>(caladoPorCaracteristica);
            caladoPorCaracteristicaEditado.CaracteristicaDeCalidad =
                Repositorio.Obtener<CaracteristicaDeCalidad>(caladoPorCaracteristica.CaracteristicaId);
            
            caladoPorCaracteristicaEditado.Unidad = caladoPorCaracteristicaEditado.CaracteristicaDeCalidad.UnidadDeMedida;
            caladoPorCaracteristicaEditado.Rango =
                caladoPorCaracteristicaEditado.CaracteristicaDeCalidad.CaladoMinimo.ToString("g0") + " - " +
                caladoPorCaracteristicaEditado.CaracteristicaDeCalidad.CaladoMaximo.ToString("g0");
            
            //
            caladoPorCaracteristicaEditado.DescuentoEnPorcentaje =
                calculadora.CalcularDescuentoEnPorcentaje(caladoPorCaracteristicaEditado.CaracteristicaDeCalidad,
                                                          caladoPorCaracteristicaEditado.ValorCalado.HasValue
                                                              ? caladoPorCaracteristicaEditado.ValorCalado.Value
                                                              : 0, instanceId);
            caladoPorCaracteristicaEditado.DescuentoEnKg =
                calculadora.CalcularDescuentoEnKg(caladoPorCaracteristicaEditado.CaracteristicaDeCalidad,
                                                  caladoPorCaracteristicaEditado.ValorCalado.HasValue
                                                      ? caladoPorCaracteristicaEditado.ValorCalado.Value
                                                      : 0, pesoNetoOrigen, instanceId);

            caladoPorCaracteristicaEditado.HuboExcepcion = calculadora.ExisteExcepcionAlDescuento(instanceId, caladoPorCaracteristica.CaracteristicaId);
            caladoPorCaracteristicaEditado.EnviaACamara = calculadora.EnviaACamara(caladoPorCaracteristicaEditado, instanceId);
            //

            if (caladoPorCaracteristicaEditado.CaracteristicaDeCalidad.EsHumedad && (caladoPorCaracteristicaEditado.DescuentoEnPorcentaje > 0 || caladoPorCaracteristicaEditado.DescuentoEnKg > 0))
            {
                caladoPorCaracteristicaEditado.AnalisisPreliminar = true;
            }
            return caladoPorCaracteristicaEditado;
        }
    }
}