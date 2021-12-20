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
    public class ProcesadorValidarConsistenciaBalanzadas : ProcesadorComando<ValidarConsistenciaBalanzadas>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        public ProcesadorValidarConsistenciaBalanzadas(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(ValidarConsistenciaBalanzadas comando)
        {
            var resultado = new ResultadoValidarConsistenciaBalanzadas();
            try
            {
                Log.Info("Se está ejecutando ValidarConsistenciaBalanzadas con balanza: {0}", comando.Balanza);

                var validacion = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoDispositivo == comando.CodigoDispositivo);
                var ultimaValidacion = comando.Desde != null ? comando.Desde.Value + validacion.OffSetPlc : validacion.UltimaValidacion;
                var hasta = comando.Hasta + validacion.OffSetPlc;
                var listaEntera = Repositorio.Listar<RegistroBalanzaPuerto, int>(x => x.Id,
                                                                                        c => c.Id >= ultimaValidacion && c.Id < hasta
                                                                                        && (c.NumeroBalanza == validacion.CodigoBalanza))
                                                                                                        .OrderBy(x => x).ToList();
                listaEntera.Add(hasta);
                if (listaEntera.Any() && ultimaValidacion < listaEntera.Max())
                {
                    resultado.BalanzadasPerdidas = Enumerable.Range(ultimaValidacion, listaEntera.Max() - ultimaValidacion).Except(listaEntera).ToList();
                    Log.Info("Faltan las siguientes balanzadas: {0}", string.Join(",", resultado.BalanzadasPerdidas));

                    int iterCantValidaciones = 0;
                    foreach (int idBalanzada in resultado.BalanzadasPerdidas)
                    {
                        bool validado = this.ValidarBalanzada(idBalanzada, validacion.OffSetPlc, comando.CodigoDispositivo, comando.Balanza);
                        while (iterCantValidaciones <= validacion.IntentosValidacion && !validado)
                        {
                            System.Threading.Thread.Sleep(4000);
                            validado = this.ValidarBalanzada(idBalanzada, validacion.OffSetPlc, comando.CodigoDispositivo, comando.Balanza);
                            iterCantValidaciones++;
                        }
                        if (!validado)
                        {
                            Log.Info("No se pudo obtener la balanzada: {0};", idBalanzada);
                        }
                        iterCantValidaciones = 0;
                        System.Threading.Thread.Sleep(4000);
                    }
                    if (!comando.Desde.HasValue)
                    {
                        this.ActualizarValidarDesde(resultado.BalanzadasPerdidas.Any() ? resultado.BalanzadasPerdidas.Max() : listaEntera.Max(), comando.CodigoDispositivo);
                    }
                }
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ValidarConsistenciaBalanzadas con balanza: {0}", comando.Balanza);
                resultado.Error("", e.Message);
            }

            return resultado;
        }

        public void ActualizarValidarDesde(int desde, string dispositivo)
        {
            var validacion = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoDispositivo == dispositivo);
            validacion.UltimaValidacion = desde;
            Repositorio.GuardarCambios();
            Log.Info("se actualizó la ultima balanzada validada: {0}. Corresponde a la balanza: {1} ", desde, dispositivo);
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
