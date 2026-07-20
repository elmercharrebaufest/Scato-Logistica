using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Validaciones;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarPatenteActiva : ProcesadorComando<ValidarPatenteActiva>
    {
        public ProcesadorValidarPatenteActiva(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ValidarPatenteActiva comando)
        {
            var resultado = new ResultadoValidarPatenteActiva();
            var sw = Stopwatch.StartNew();

            try
            {
                var patentesLeidas = comando.Patentes != null ? comando.Patentes.Where(p => !string.IsNullOrEmpty(p)).ToList() : new List<string>();
                if (!patentesLeidas.Any())
                {
                    resultado.Error(nameof(ValidarPatenteActiva.Patentes), "Es requerido una patente como mínimo.");
                    return resultado;
                }

                if (!EsSustitucionActiva())
                {
                    resultado.Error(nameof(ValidarPatenteActiva), "Configuración de sustitución de patente desactivada.");
                    return resultado;
                }

                var patentesActivas = Repositorio.Listar<Recorrido, string>(x => x.Patente, x => !x.Terminado)
                    .Where(p => p != null)
                    .ToList();
                var exactas = patentesActivas.Where(a => patentesLeidas.Contains(a)).ToList();
                if (exactas.Count > 1)
                {
                    resultado.Error(nameof(ValidarPatenteActiva.Patentes), "Se encontraron varias patentes activas que podrían coincidir con las patentes leídas.");
                    return resultado;
                }

                if (exactas.Count == 1)
                {
                    resultado.PatenteActiva = exactas[0];
                    resultado.Diferencia = 0;
                    return resultado;
                }

                var maxSustituciones = ObtenerMaxSustituciones();
                resultado = EjecutarSustitucion(patentesLeidas, patentesActivas, maxSustituciones);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al validar patente activa.");
                resultado.Error("500", "Error al validar patente activa.");
            }
            finally
            {
                sw.Stop();
                resultado.DuracionMs = (int)sw.ElapsedMilliseconds;
            }

            return resultado;
        }

        private ResultadoValidarPatenteActiva EjecutarSustitucion(IEnumerable<string> patentesLeidas, IEnumerable<string> patentesActivas, int maxSustituciones)
        {
            var resultado = new ResultadoValidarPatenteActiva();

            var candidatos = patentesActivas
                .Select(p => new
                {
                    Patente = p,
                    Score = patentesLeidas.Min(l => CalcularDiferencia(l, p))
                })
                .Where(x => x.Score > 0 && x.Score <= maxSustituciones)
                .OrderBy(x => x.Score)
                .ToList();

            if (!candidatos.Any())
            {
                resultado.Error(nameof(ValidarPatenteActiva.Patentes), "No se encontró ninguna patente activa que coincida con las patentes leídas.");
                return resultado;
            }

            if (candidatos.Count > 1)
            {
                resultado.Error(nameof(ValidarPatenteActiva.Patentes), "Se encontraron varias patentes activas que podrían coincidir con las patentes leídas.");
                return resultado;
            }

            resultado.PatenteActiva = candidatos[0].Patente;
            resultado.Diferencia = candidatos[0].Score;
            return resultado;
        }

        private bool EsSustitucionActiva()
        {
            var config = Repositorio.ObtenerPrimero<ConfiguracionGeneral>(
                x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.Cardless
                  && x.Nombre == Constantes.ConfiguracionGeneral.Cardless.SustitucionActiva);

            return config != null && bool.TryParse(config.Valor, out var valor) && valor;
        }

        private int ObtenerMaxSustituciones()
        {
            var config = Repositorio.ObtenerPrimero<ConfiguracionGeneral>(
                x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.Cardless
                  && x.Nombre == Constantes.ConfiguracionGeneral.Cardless.MaxSustituciones);

            if (config == null || !int.TryParse(config.Valor, out var valor) || valor < 0)
                return 0;

            return valor;
        }

        public int CalcularDiferencia(string a, string b)
        {
            var masLarga = a.Length >= b.Length ? a : b;
            var masCorta = a.Length >= b.Length ? b : a;
            var diferencias = 0;

            for (int i = 0; i < masLarga.Length; i++)
            {
                if (i < masCorta.Length)
                {
                    if (masLarga[i] != masCorta[i])
                        diferencias++;
                }
                else
                {
                    diferencias++;
                }
            }

            return diferencias;
        }
    }
}
