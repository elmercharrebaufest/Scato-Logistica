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
    public class ProcesadorCrearRecorrido : ProcesadorComando<CrearRecorrido>
    {
        public ProcesadorCrearRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearRecorrido comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearRecorrido para la instancia de workflow = {0}", comando.InstanceId);
                var recorridoViejo = Repositorio.Obtener<Recorrido>(f => f.InstanciaWorkflow == comando.InstanceIdViejo);

                if (recorridoViejo != null)
                {
                    Log.Info("Se procederá a crear el recorrido para el workflow {0}", comando.InstanceId);
                    var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(f => f.Id == comando.WorkflowDefinicionId);
                    var recorrido = new Recorrido
                        {
                            InstanciaWorkflow = comando.InstanceId,
                            Usuario = comando.Usuario,
                            FechaInicio = DateTime.Now,
                            Workflow = workflow,
                            Chofer = recorridoViejo.Chofer,
                            Centro = recorridoViejo.Centro,
                            Patente = recorridoViejo.Patente,
                            Transportista = recorridoViejo.Transportista,
                            TipoComercial = recorridoViejo.TipoComercial,
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                            Material = recorridoViejo.Material,
                            PesoBrutoOrigen = recorridoViejo.PesoBruto,
                            PesoTaraOrigen = recorridoViejo.PesoTara,
                            WorkflowDefinicion = workflowDefinicion,
                            Vehiculo = recorridoViejo.Vehiculo,
                            TipoDocumentoIngresoRelacionado = recorridoViejo.TipoDocumentoIngreso,
                            NumeroDocumentoIngresoRelacionado = recorridoViejo.NumeroDocumentoIngreso,
                            MotivoDemora = recorridoViejo.MotivoDemora
                        };
                    Repositorio.Agregar(recorrido);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);
                    Repositorio.GuardarCambios();
                    resultado.Id = recorrido.Id;
                }
                else
                {
                    Log.Error("Error al obtener el origen de datos con workflow id {0}", comando.InstanceIdViejo);
                    resultado.Error("", Textos.Recorrido_ErrorAlCrear);  
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear el recorrido para la instancia de workflow {0}", comando.InstanceId);
                resultado.Error("", Textos.CartaDePorte_Error);
            }
            return resultado;
        }
    }
}
