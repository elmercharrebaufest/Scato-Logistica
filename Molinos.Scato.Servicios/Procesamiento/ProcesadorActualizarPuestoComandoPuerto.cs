using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarPuestoComandoPuerto : ProcesadorComando<ActualizarPuestoComandoPuerto>
    {
        public ProcesadorActualizarPuestoComandoPuerto(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarPuestoComandoPuerto comando)
        {
            Log.Debug("Iniciando actualizacion de puesto comando puerto");

            var resultado = new ResultadoPuestoComandoPuerto { Workflows = new List<DatosDeWorkflowDto>() };

            var puntodecarga = Repositorio.Obtener<PuntoDeCarga>(r => r.Id == comando.Dto.PuntoDeCargaId);
            var recorridos = Repositorio.Listar<Recorrido>(r => comando.Dto.InstanceIdsList.Contains(r.InstanciaWorkflow));
            var almacen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenId);
            var calle = Repositorio.Obtener<Calle>(comando.Dto.CalleId);
            var hidraulicas = Repositorio.Listar<PuestosDeCargaDescarga>(h => comando.Dto.HidraulicasId.Contains(h.Id));

            ValidarEntidades(resultado, almacen, calle, recorridos, comando);

            if (!resultado.HayErrores)
            {
                ProcesarRecorridos(resultado, recorridos, almacen, calle, puntodecarga, hidraulicas, comando);
                Repositorio.GuardarCambios();
                Log.Debug("Puesto comando puerto actualizado correctamente");
            }

            return resultado;
        }

        private void ValidarEntidades(Resultado resultado, Almacen almacen, Calle calle, IList<Recorrido> recorridos, ActualizarPuestoComandoPuerto comando)
        {
            if (almacen == null)
            {
                resultado.Error("AlmacenId", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando puerto almacen invalido");
            }

            if (calle == null)
            {
                resultado.Error("CalleId", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando puerto calle invalida");
            }

            if (recorridos.Count != comando.Dto.InstanceIdsList.Count)
            {
                resultado.Error("Recorrido", Textos.Error_Invalido);
                Log.Error("Error de validacion puesto comando puerto recorrido invalido");
            }
        }

        private void ProcesarRecorridos(ResultadoPuestoComandoPuerto resultado, IList<Recorrido> recorridos, Almacen almacen, Calle calle, PuntoDeCarga puntodecarga, IList<PuestosDeCargaDescarga> hidraulicas, ActualizarPuestoComandoPuerto comando)
        {
            foreach (var recorrido in recorridos)
            {
                ActualizarRecorrido(recorrido, almacen, calle, puntodecarga, hidraulicas, comando);
                AgregarWorkflow(resultado, recorrido, puntodecarga);
            }
        }

        private void ActualizarRecorrido(Recorrido recorrido, Almacen almacen, Calle calle, PuntoDeCarga puntodecarga, IList<PuestosDeCargaDescarga> hidraulicas, ActualizarPuestoComandoPuerto comando)
        {
            recorrido.Almacen = almacen;
            recorrido.Calle = calle;
            recorrido.PuntoDeCarga = puntodecarga;
            recorrido.PuestosDeCargaDescargas.Clear();

            foreach (var hidraulica in hidraulicas)
            {
                recorrido.PuestosDeCargaDescargas.Add(hidraulica);
            }

            recorrido.CorrespondeCaladoEnPlanta = comando.Dto.CorrespondeCaladoEnPlanta;
        }

        private void AgregarWorkflow(ResultadoPuestoComandoPuerto resultado, Recorrido recorrido, PuntoDeCarga puntodecarga)
        {
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
                EsSoja = recorrido.Material.CodigoSAP == Constantes.MaterialPagoRealizado.SojaSAP,
                PuntoDeCarga = puntodecarga.Descripcion
            });
        }



    }
}
