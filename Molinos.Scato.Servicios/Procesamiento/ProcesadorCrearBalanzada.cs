using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBalanzada : ProcesadorCrear<CrearBalanzada, Balanzada>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorCrearBalanzada(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        protected override Balanzada CrearEntidad(CrearBalanzada comando)
        {
            var offsetBalanza = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoBalanza == comando.Dto.NumeroBalanza).OffSetPlc;
            return new Balanzada
            {
                Id = comando.Dto.Id + offsetBalanza,
                NumeroBalanza = comando.Dto.NumeroBalanza,
                Fecha = comando.Dto.Fecha.Value,
                Tipo = "balanzada",
                PesoBruto = comando.Dto.PesoBruto,
                PesoNeto = comando.Dto.PesoNeto,
                PesoTara = comando.Dto.PesoTara,
                CargaInicial = Repositorio.Obtener<Carga>(x => x.NumeroBalanza == comando.Dto.NumeroBalanza && x.Id == comando.Dto.CargaInicial_Id)
            };
        }

        protected override void Validar(CrearBalanzada comando, Resultado resultado)
        {
            var offsetBalanza = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoBalanza == comando.Dto.NumeroBalanza).OffSetPlc;
            if (Repositorio.Existe<Balanzada>(e => e.Id == comando.Dto.Id + offsetBalanza && e.NumeroBalanza == comando.Dto.NumeroBalanza))
            {
                resultado.Error("CrearBalanzada", string.Format(Textos.Error_ActualizarGenerico));
            }
            ValidarFechaBalanzada(comando, resultado);
            ValidarIdBalanzada(comando, resultado, offsetBalanza);
        }
        private bool ValidarFecha(string fechaHora, ref DateTime fecha)
        {
            return DateTime.TryParse(fechaHora, out fecha);
        }
        private void ValidarFechaBalanzada(CrearBalanzada comando, Resultado resultado)
        {
            var carga = Repositorio.Obtener<Carga>(x => x.NumeroBalanza == comando.Dto.NumeroBalanza && x.Id == comando.Dto.CargaInicial_Id);
            if (carga.CargaOpuesta != null)
            {
                if (carga.Fecha > comando.Dto.Fecha || comando.Dto.Fecha > carga.CargaOpuesta.Fecha)
                {
                    resultado.Error("ModificarBalanzada", string.Format(Textos.Error_CrearBalanzada_FechaFueraDeRango) + " (" + carga.Fecha + " - " + carga.CargaOpuesta.Fecha + ")");
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
        private void ValidarIdBalanzada(CrearBalanzada comando, Resultado resultado, int offSet)
        {
            var carga = Repositorio.Obtener<Carga>(x => x.NumeroBalanza == comando.Dto.NumeroBalanza && x.Id == comando.Dto.CargaInicial_Id);
            if (carga.CargaOpuesta == null)
            {
                if (Repositorio.Existe<RegistroBalanzaPuerto>(x => x.Id == comando.Dto.Id + offSet && x.NumeroBalanza == comando.Dto.NumeroBalanza))
                {
                    resultado.Error("ModificarBalanzada", string.Format(Textos.Error_CrearCarga_IdRepetido));
                }
                if (comando.Dto.Id + offSet <= carga.Id)
                {
                    resultado.Error("ModificarBalanzada", string.Format(Textos.Error_CrearCarga_IdMenorACargaInicio));
                }
            }
        }
    }
}
