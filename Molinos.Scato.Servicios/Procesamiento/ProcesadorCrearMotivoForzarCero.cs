using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMotivoForzarCero : ProcesadorCrear<CrearMotivoForzarCero,MotivoForzarCero>
    {
        public ProcesadorCrearMotivoForzarCero(IRepositorio repositorio, IConversor conversor, ILogger log): base(repositorio, conversor, log)
        {
        }

        protected override MotivoForzarCero CrearEntidad(CrearMotivoForzarCero comando)
        {
            var balanza = Repositorio.Obtener<Balanza>(x => x.Id == comando.BalanzaId);
            var motivo = new MotivoForzarCero
            {
                Balanza = balanza,
                Fecha = comando.Fecha,
                Motivo = comando.Motivo,
                InstanceId = comando.InstanceId,
                Usuario = comando.Usuario
            };
            return motivo;
        }

        protected override void Validar(CrearMotivoForzarCero comando, Resultado resultado)
        {

        }
    }
}
