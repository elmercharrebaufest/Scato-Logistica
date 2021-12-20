using System;
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
    public class ProcesadorActualizarCargaOpuesta : ProcesadorComando<ActualizarCargaOpuesta>
    {
        public ProcesadorActualizarCargaOpuesta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarCargaOpuesta comando)
        {
            var resultado = new ResultadoActualizarCargaOpuesta();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                ModificarEntidad(comando, resultado);
                Repositorio.GuardarCambios();

            }

            return resultado;
        }

        private void ModificarEntidad(ActualizarCargaOpuesta comando, ResultadoActualizarCargaOpuesta resultado)
        {
            var carga = Repositorio.Obtener<Carga>(x => x.Id == comando.Carga_Id && comando.NumeroBalanza == comando.NumeroBalanza);
            if (carga == null)
            {
                resultado.Errores.Add("NoExisteCarga", "No existe la carga.");
                return;
            }
            var cargaOpuesta = Repositorio.Obtener<Carga>(x => x.Id == comando.CargaOpuesta_Id && comando.NumeroBalanza == comando.NumeroBalanza);
            if (cargaOpuesta == null)
            {
                resultado.Errores.Add("NoExisteCarga", "No existe la carga opuesta.");
                return;
            }
            carga.CargaOpuesta_Id = comando.CargaOpuesta_Id;
            carga.CargaOpuesta_NumeroBalanza = comando.NumeroBalanza;
            cargaOpuesta.CargaOpuesta_Id = comando.Carga_Id;
            cargaOpuesta.CargaOpuesta_NumeroBalanza = comando.NumeroBalanza;
            carga.EnviadoASap = !Repositorio.Existe<Balanzada>(x => x.CargaInicial_Id == carga.Id && x.NumeroBalanza == carga.NumeroBalanza && !x.EnviadoASap);
        }

        protected void Validar(ActualizarCargaOpuesta comando, ResultadoActualizarCargaOpuesta resultado)
        {

        }
    }
}