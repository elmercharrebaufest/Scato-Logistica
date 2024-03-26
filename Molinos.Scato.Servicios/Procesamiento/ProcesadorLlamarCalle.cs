using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLlamarCalle : ProcesadorComando<LlamarCalle>
    {
        public ProcesadorLlamarCalle(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LlamarCalle comando)
        {
            var resultado = new Resultado();
            try
            {
                Validar(comando, resultado);
                if (resultado.HayErrores)
                    return resultado;

                var calle = Repositorio.Obtener<Calle>(comando.Dto.Id);
                calle.Bloqueada = comando.Dto.Bloqueada;
                calle.FechaLLamada = comando.Dto.FechaLLamada;

                if (comando.Dto.CalleCaladoId > 0)
                    calle.CalleCalado = Repositorio.Obtener<Calle>(comando.Dto.CalleCaladoId);

                if (!comando.Llamada)
                    calle.CalleCalado = null;

                Repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                Log.Error($"Error en el llamado de la calle {comando.Dto.Id} - {ex.Message}");
                resultado.Error("", ex.Message);
            }

            return resultado;
        }

        private void Validar(LlamarCalle comando, Resultado resultado)
        {
            var ConfiguracionGranoActivo = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            var ConfiguracionNoGranoActivo = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
            if ((ConfiguracionGranoActivo.Valor.Equals("True")
                && Repositorio.Existe<AutomatismoGrano>(a => a.Activo == true && (a.CallePreBalanzaId == comando.Dto.Id || a.CallePreHidraulicaId == comando.Dto.Id)))
                || (ConfiguracionNoGranoActivo.Valor.Equals("True")
                && Repositorio.Existe<AutomatismoNoGrano>(a => a.Activo == true && (a.CallePlanta.Id == comando.Dto.Id || a.CallePlayaInterna.Id == comando.Dto.Id))))
            {
                resultado.Error("", Textos.Automatismo_CalleUtilizadaEnAutomatismoActivo);
            }
        }
    }
}