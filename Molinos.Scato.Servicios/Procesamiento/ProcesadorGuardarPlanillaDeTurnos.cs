using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorGuardarPlanillaDeTurnos : ProcesadorModificar<GuardarPlanillaDeTurnos>
    {
        public ProcesadorGuardarPlanillaDeTurnos(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(GuardarPlanillaDeTurnos comando)
        {


            var moduloDeCarga = Repositorio.Obtener<ModuloDeCarga>(comando.IdModuloDeCarga);

            ///////////////////////////
            ///// PROCESO PARA EL HISTORICO /////
            if (moduloDeCarga.FechaDeCreacion == null)
                moduloDeCarga.FechaDeCreacion = DateTime.Now;
            else
            {
                ///// OPERACIONES/TABLERISTAS /////
                moduloDeCarga.FechaDeModificacion = DateTime.Now;

                
                var moduloDeCargaPlanillaDeTurnos = new ModuloDeCargaPlanillaDeTurnos
                {
                    ModuloDeCarga = moduloDeCarga,
                    Fecha = comando.Dto.Fecha,
                };

                moduloDeCargaPlanillaDeTurnos.ModuloDeCargaPlanillaDeTurnosTurnos = new List<ModuloDeCargaPlanillaDeTurnosTurnos>();
                    

                    comando.Dto.ModuloDeCargaPlanillaDeTurnosTurnos.ToList()
                    .ForEach(turnos => moduloDeCargaPlanillaDeTurnos.ModuloDeCargaPlanillaDeTurnosTurnos.Add(new ModuloDeCargaPlanillaDeTurnosTurnos
                    {
                        TurnoPuerto = turnos.TurnoPuerto != null ? Repositorio.Obtener<TurnoPuerto>(turnos.TurnoPuerto.Id) : null,
                        Cerrado = turnos.Cerrado,
                        Enviado = turnos.Enviado
                    }));

                    moduloDeCarga.ModuloDeCargaPlanillaDeTurnos.Add(new ModuloDeCargaPlanillaDeTurnos
                    {
                        ModuloDeCarga = moduloDeCarga,
                        Fecha = comando.Dto.Fecha,
                        ModuloDeCargaPlanillaDeTurnosTurnos = moduloDeCargaPlanillaDeTurnos.ModuloDeCargaPlanillaDeTurnosTurnos
                    });
                Repositorio.GuardarCambios();
            }
        }

        protected override void Validar(GuardarPlanillaDeTurnos comando, Resultado resultado)
        {

        }


    }
}