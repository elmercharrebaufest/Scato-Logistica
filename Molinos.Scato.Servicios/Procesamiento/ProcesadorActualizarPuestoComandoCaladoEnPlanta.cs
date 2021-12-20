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
    public class ProcesadorActualizarPuestoComandoCaladoEnPlanta : ProcesadorComando<ActualizarPuestoComandoCaladoEnPlanta>
    {
        public ProcesadorActualizarPuestoComandoCaladoEnPlanta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarPuestoComandoCaladoEnPlanta comando)
        {
            Log.Debug("Iniciando actualizacion de puesto comando - calado en planta");
            var resultado = new ResultadoPuestoComando { Workflows = new List<DatosDeWorkflowDto>() };

            var recorridos = Repositorio.Listar<Recorrido>(r => comando.Dto.InstanceIdsList.Contains(r.InstanciaWorkflow));

            if (recorridos.Count != comando.Dto.InstanceIdsList.Count)
            {
                resultado.Error("Recorrido", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando recorrido invalido");
            }

            if (!resultado.HayErrores)
            {
                foreach (var recorrido in recorridos)
                {
                    recorrido.CorrespondeCaladoEnPlanta = comando.Dto.CorrespondeCaladoEnPlanta;
                }

                Repositorio.GuardarCambios();
                Log.Debug("Puesto comando - Calado en Planta actualizado correctamente");
            }
            return resultado;
        }

    }


}