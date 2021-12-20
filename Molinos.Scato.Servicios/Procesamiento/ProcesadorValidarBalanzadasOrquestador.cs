using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarBalanzadasOrquestador : ProcesadorComando<ValidarBalanzadasOrquestador>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        public ProcesadorValidarBalanzadasOrquestador(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(ValidarBalanzadasOrquestador comando)
        {
            var resultado = new ResultadoValidarConsistenciaBalanzadas();
            try
            {
                Log.Info("Se está ejecutando ValidarBalanzadasOrquestador con balanza: {0}", comando.CodigoBalanza);
                var balanza = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoBalanza == comando.CodigoBalanza);
                var ultimoRegistroBalanzaPuerto = Repositorio.ObtenerMayor<RegistroBalanzaPuerto, int>(x => x.NumeroBalanza == balanza.CodigoBalanza,
                                                                                                    x => x.Id);
                if (ultimoRegistroBalanzaPuerto != null)
                {
                    this.ValidarRecursivo(balanza, ultimoRegistroBalanzaPuerto.Id);
                }
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ValidarBalanzadasOrquestador.");
                resultado.Error("", e.Message);
            }

            return resultado;
        }

        private void ValidarRecursivo(BalanzaPuerto balanza, int ultimoRegistroBalanzaPuertoId)
        {
            
            
                int iterCantValidaciones = 0;
                int siguienteId = ultimoRegistroBalanzaPuertoId + 1;

                bool validado = this.ValidarBalanzada(siguienteId, balanza.OffSetPlc, balanza.CodigoDispositivo, balanza.CodigoBalanza);
                while (iterCantValidaciones < balanza.IntentosValidacion && !validado)
                {
                    validado = this.ValidarBalanzada(siguienteId, balanza.OffSetPlc, balanza.CodigoDispositivo, balanza.CodigoBalanza);
                    System.Threading.Thread.Sleep(2000);
                    iterCantValidaciones++;
                }
                if (!validado)
                {
                    Log.Info("No hubo novedades de la balanza {0};", balanza.CodigoBalanza);
                }
                else
                {
                    this.ValidarRecursivo(balanza, ultimoRegistroBalanzaPuertoId + 1);
                }
            
        }

        private bool ValidarBalanzada(int idBalanzada, int offSetPlc, string codigoDispositivo, string balanza)
        {
            Log.Info("Buscando la balanzada: {0}. OffSet {1}", idBalanzada, offSetPlc);

            var balanzada = orquestador.Ejecutar(new EjecutarConsultaBalanzada { CodigoDispositivo = codigoDispositivo, IdBalanzada = idBalanzada - offSetPlc });

            if (balanzada.Mensaje.Codigo == 100)
            {
                Log.Error("No se encuentra la balanza con código {0}", codigoDispositivo);
            }
            else
            if (balanzada.Mensaje.Codigo == 0)
            {

                Type type = balanzada.GetType();
                IEnumerable props = type.GetRuntimeProperties();

                foreach (PropertyInfo prop in props)
                {
                    if (String.Equals(prop.Name, "ValoresBalanzada", StringComparison.OrdinalIgnoreCase))
                    {
                        var valores = (Dictionary<string, string>)prop.GetValue(balanzada);

                        if (valores != null)
                        {
                            servicioComandos.Ejecutar(new CrearLecturaBalanzada { CodigoDispositivo = balanza, Informacion = valores });
                            return true;
                        }
                        else
                        {
                            Log.Info("No se encontro la balanzda: {0}. OffSet: {1}", idBalanzada, offSetPlc);
                        }
                    }
                }
            }
            else
            {
                Log.Error("Error al buscar la balanzada {0}: {1}. OffSet: {2}", idBalanzada, balanzada.Mensaje.Descripcion, offSetPlc);
            }
            return false;
        }
    }
}
