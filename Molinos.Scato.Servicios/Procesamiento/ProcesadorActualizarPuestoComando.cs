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
    public class ProcesadorActualizarPuestoComando : ProcesadorComando<ActualizarPuestocomando>
    {
        public ProcesadorActualizarPuestoComando(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarPuestocomando comando)
        {
            Log.Debug("Iniciando actualizacion de puesto comando");
            var resultado = new ResultadoPuestoComando { Workflows = new List<DatosDeWorkflowDto>() };

            var recorridos = Repositorio.Listar<Recorrido>(r => comando.Dto.InstanceIdsList.Contains(r.InstanciaWorkflow));
            var almacen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenId);
            var calle = Repositorio.Obtener<Calle>(comando.Dto.CalleId);
            var hidraulicas = Repositorio.Listar<PuestosDeCargaDescarga>(h => comando.Dto.HidraulicasId.Contains(h.Id));

            Balanza balanzaBruto = null;
            Balanza balanzaTara = null;
            if (comando.BalanzasObligatorias)
            {
                balanzaBruto = Repositorio.Obtener<Balanza>(comando.Dto.BalanzaBrutoId);
                balanzaTara = Repositorio.Obtener<Balanza>(comando.Dto.BalanzaTaraId);

                if (balanzaBruto == null)
                {
                    resultado.Error("BalanzaBrutoId", Textos.Error_Invalido);
                    Log.Error("Error de validacion puesto comando balanza bruto invalida");
                }
                if (balanzaTara == null)
                {
                    resultado.Error("BalanzaTaraId", Textos.Error_Invalido);
                    Log.Error("Error de validacion puesto comando balanza tara invalida");
                }
            }

            if (almacen == null)
            {
                resultado.Error("AlmacenId", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando almacen invalido");
            }

            if (calle == null)
            {
                resultado.Error("CalleId", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando calle invalida");
            }

            if (hidraulicas.Count != comando.Dto.HidraulicasId.Length)
            {
                resultado.Error("HidraulicasId", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando hidraulica invalida");
            }

            if (recorridos.Count != comando.Dto.InstanceIdsList.Count)
            {
                resultado.Error("Recorrido", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando recorrido invalido");
            }

            if (!resultado.HayErrores)
            {
                foreach (var recorrido in recorridos)
                {
                    if (comando.BalanzasObligatorias)
                    {
                        recorrido.BalanzaBruto = balanzaBruto;
                        recorrido.BalanzaTara = balanzaTara;
                    }
                    recorrido.Almacen = almacen;
                    recorrido.Calle = calle;
                    recorrido.PuestosDeCargaDescargas.Clear();
                    foreach (var hidraulica in hidraulicas)
                    {
                        recorrido.PuestosDeCargaDescargas.Add(hidraulica);
                    }
                    recorrido.CorrespondeCaladoEnPlanta = comando.Dto.CorrespondeCaladoEnPlanta;

                    resultado.Workflows.Add(new DatosDeWorkflowDto
                    {
                        Codigo = recorrido.Workflow.Codigo,
                        InstanciaWorkflow = recorrido.InstanciaWorkflow,
                        WorkflowDefId = recorrido.WorkflowDefinicion.Id,
                        Patente = recorrido.Patente,
                        FechaCalado = recorrido.Calado != null && recorrido.Calado.FechaCreacion.HasValue ? recorrido.Calado.FechaCreacion.Value : recorrido.FechaInicio,
                        PatenteAcoplado = recorrido.Vehiculo != null ? recorrido.Vehiculo.PatenteAcoplado : "",
                        MaterialDescripcion = recorrido.Material.Descripcion,
                        MaterialEsGrano = recorrido.Material.EsGrano,
                        NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                        Calidad = recorrido.AnalisisDeCalidad != null ? "Analisis" : "",
                        Humedad = recorrido.CaracteristicasAnalizadas != null && recorrido.CaracteristicasAnalizadas.Humedad.HasValue ? recorrido.CaracteristicasAnalizadas.Humedad.Value.ToString() : "",
                        TieneDescuentos = recorrido.CaracteristicasAnalizadas != null && recorrido.CaracteristicasAnalizadas.TieneDescuentos,
                        EsSoja = recorrido.Material.CodigoSAP == "19908017"
                    });
                }

                Repositorio.GuardarCambios();
                Log.Debug("Puesto comando actualizado correctamente");
            }
            return resultado;
        }

    }


}