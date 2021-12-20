using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarBalanzada : ProcesadorModificar<ModificarBalanzada>
    {
        public ProcesadorModificarBalanzada(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarBalanzada comando)
        {
            var balanzaPuerto = Repositorio.Obtener<Balanzada>(e => comando.Dto.Id == e.Id && e.NumeroBalanza == comando.Dto.NumeroBalanza);
            Conversor.Convertir(comando.Dto, balanzaPuerto);
            balanzaPuerto.CargaInicial = Repositorio.Obtener<Carga>(x => x.NumeroBalanza == comando.Dto.NumeroBalanza && x.Id == comando.Dto.CargaInicial_Id);
        }

        protected override void Validar(ModificarBalanzada comando, Resultado resultado)
        {
            if (Repositorio.Obtener<Balanzada>(e => e.Id == comando.Dto.Id && e.NumeroBalanza == comando.Dto.NumeroBalanza) == null)
            {
                resultado.Error("ModificarBalanzada", string.Format(Textos.Error_ActualizarGenerico));
            }
            ValidarFechaBalanzada(comando, resultado);
        }

        private void ValidarFechaBalanzada(ModificarBalanzada comando, Resultado resultado)
        {
            var carga = Repositorio.Obtener<Carga>(x => x.NumeroBalanza == comando.Dto.NumeroBalanza && x.Id == comando.Dto.CargaInicial_Id);
            if (carga.CargaOpuesta != null)
            {
                if (carga.Fecha > comando.Dto.Fecha || comando.Dto.Fecha > carga.CargaOpuesta.Fecha)
                {
                    resultado.Error("ModificarBalanzada", string.Format(Textos.Error_CrearBalanzada_FechaFueraDeRango + " (" + carga.Fecha + " - " + carga.CargaOpuesta.Fecha + ")"));
                }
            }
            else
            {
                if (carga.Fecha > comando.Dto.Fecha)
                {
                    resultado.Error("ModificarBalanzada", string.Format(Textos.Error_CrearBalanzada_FechaMenorInicio) + " (" + carga.Fecha + ")");
                }
            }
        }
        private bool ValidarFecha(string fechaHora, ref DateTime fecha)
        {
            return DateTime.TryParse(fechaHora, out fecha);
        }
    }
}
