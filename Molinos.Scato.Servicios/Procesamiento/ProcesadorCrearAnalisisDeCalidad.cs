using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAnalisisDeCalidad : ProcesadorComando<CrearAnalisisDeCalidad>
    {
        private ICalculadoraDescuento calculadora;
        public ProcesadorCrearAnalisisDeCalidad(IRepositorio repositorio, IConversor conversor, ILogger log, ICalculadoraDescuento calculadora)
            : base(repositorio, conversor, log)
        {
            this.calculadora = calculadora;
        }

        public override Resultado Ejecutar(CrearAnalisisDeCalidad comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearAnalisisDeCalidad para la instancia de workflow = {0}", comando.WorkflowInstanceId);
                var calado = Repositorio.Obtener<Calado>(comando.CaladoId);
                var estado = Repositorio.ObtenerProyeccion<Recorrido, CaracteristicasAnalizadas>(
                    x => x.InstanciaWorkflow == calado.WorkflowInstanceId, x => x.CaracteristicasAnalizadasList.FirstOrDefault());
                if (estado == null)
                {
                    estado = new CaracteristicasAnalizadas();
                    estado.Recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == calado.WorkflowInstanceId);
                }
                var analisis = new AnalisisDeCalidad
                    {
                        NumeroOrden = comando.NumeroDeOrden,
                        WorkflowInstanceId = comando.WorkflowInstanceId,
                        Calado = calado,
                        FechaCreacion = DateTime.Now,
                        Usuario = comando.Usuario
                    };

                analisis.CaracteristicasAnalizadas = new List<AnalisisPorCaracteristica>();
                comando.Caracteristicas.ToList().ForEach(f => analisis.CaracteristicasAnalizadas.Add(new AnalisisPorCaracteristica
                    {
                        AnalisisDeCalidad = analisis,
                        CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(f.CaracteristicaId),
                        Rango = f.Rango,
                        Unidad = f.Unidad,
                        ValorAnalisis = f.ValorAnalisis,
                        ValorCalado = f.ValorCalado
                    }));
                //
                foreach (var caracteristica in analisis.CaracteristicasAnalizadas)
                {
                    var valor = caracteristica.ValorAnalisis ?? caracteristica.ValorCalado;
                    if (valor.HasValue)
                    {
                        caracteristica.DescuentoEnPorcentaje =
                            calculadora.CalcularDescuentoEnPorcentaje(caracteristica.CaracteristicaDeCalidad, valor.Value, comando.WorkflowInstanceId);
                        caracteristica.DescuentoEnKg =
                            calculadora.CalcularDescuentoEnKg(caracteristica.CaracteristicaDeCalidad, valor.Value, comando.PesoNetoOrigen, comando.WorkflowInstanceId);
                        caracteristica.HuboExcepcion = calculadora.ExisteExcepcionAlDescuento(comando.WorkflowInstanceId, caracteristica.CaracteristicaDeCalidad.Id);
                        caracteristica.EnviaACamara = calculadora.EnviaACamara(caracteristica, comando.WorkflowInstanceId);
                    }
                }

                calculadora.ActualizarMermaVolatil(calado, analisis, comando.PesoNetoOrigen);
                calculadora.ActualizarEstado(null, analisis, estado);
                calculadora.ActualizarEstadoEspecial(calado, analisis, estado);
                if (estado.Id == 0)
                {
                    Repositorio.Agregar(estado);
                }
                Repositorio.Agregar(analisis);
                Log.Info("Se modificará el recorrido con el nuevo análisis de calidad", comando.WorkflowInstanceId);
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(f => f.InstanciaWorkflow == comando.WorkflowInstanceId);
                recorridoAEditar.AnalisisDeCalidad = analisis;

                Repositorio.GuardarCambios();
                resultado.Id = (int)analisis.GetType().GetProperty("Id").GetValue(analisis, null);
                Log.Info("Se creó exitosamente el análisis de calidad para la instancia de workflow {0}", comando.WorkflowInstanceId);

            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar realizar el análisis de calidad para la instancia de workflow {0}", comando.WorkflowInstanceId);
                resultado.Error("", Textos.AnalisisDeCalidad_Error);
            }

            return resultado;

        }
        
    }
}
