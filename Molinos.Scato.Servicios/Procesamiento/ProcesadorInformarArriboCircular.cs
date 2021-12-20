using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorInformarArriboCircular : ProcesadorComando<InformarArriboCircular>
    {
        private readonly IServicioCircular servicioCircular;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorInformarArriboCircular(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioCircular servicioCircular, IServicioRepositorio servicioRepositorio, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.servicioCircular = servicioCircular;
            this.servicioRepositorio = servicioRepositorio;
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(InformarArriboCircular comando)
        {
            var resultado = new ResultadoCircular();
            try
            {
                Log.Debug($"ProcesadorInformarArriboCamion= CentroId: {comando.CentroId}, CartaPorte: {comando.CartaPorte} Patente: {comando.Patente} MaterialId: {comando.MaterialId}");
                var informaCircular = Repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == comando.CentroId, x => x.InformaCircular);
                Log.Debug($"ProcesadorInformarArriboCamion= InformaEstadoCircular:{informaCircular}, CentroId: {comando.CentroId}, CartaPorte: {comando.CartaPorte} Patente: {comando.Patente} MaterialId: {comando.MaterialId}");

                if (informaCircular)
                {
                    var codigoEspecieMaterial = Repositorio.ObtenerProyeccion<Material, int?>(x => x.Id == comando.MaterialId, x => x.CodigoEspecie);

                    if (codigoEspecieMaterial == null)
                    {
                        resultado.Errores.Add("", "El Material ingresado no posee Código de Especie.");
                    }

                    if (resultado.HayErrores)
                    {
                        return resultado;
                    }

                    Log.Debug($"ProcesadorInformarArriboCamion: Patente: {comando.Patente} MaterialId: {comando.MaterialId} CodigoEspecieDelMaterial: {codigoEspecieMaterial}");

                    var respCircular = servicioCircular.InformarArribo(comando.CartaPorte, comando.Patente, codigoEspecieMaterial.ToString());

                    Log.Debug($"ProcesadorInformarArriboCamion: Patente: {comando.Patente} MaterialId: {comando.MaterialId} CodigoEspecieDelMaterial: {codigoEspecieMaterial} Respuesta: ExisteTurno = {respCircular.SacoTurnoConCircular} LlegoEnHorario = {respCircular.LlegoEnHorario}");

                    resultado.TurnoActivo = respCircular.LlegoEnHorario;

                    if (respCircular.SacoTurnoConCircular)
                    {
                        var simularTurnoActivoCircular = configuracion.AppSettings.Get("SimularTurnoActivoCircular");
                        if (!string.IsNullOrEmpty(simularTurnoActivoCircular) && simularTurnoActivoCircular.ToUpper() == "TRUE")
                        {
                            resultado.TurnoActivo = true;
                        }

                        var cargaDeCupo = Repositorio.Obtener<CargaDeCupo>(x => x.NumeroCartaPorte == comando.CartaPorte || x.CTG == comando.CartaPorte);
                        cargaDeCupo.SacoTurnoConCircular = respCircular.SacoTurnoConCircular;
                        cargaDeCupo.LlegoEnHorario = resultado.TurnoActivo;

                        Repositorio.GuardarCambios();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar el informe de arribo del camión a Circular App: {e.Message}");
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
