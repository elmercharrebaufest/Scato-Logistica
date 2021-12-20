using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearInhabilitacionChofer : ProcesadorCrear<CrearInhabilitacionChofer, InhabilitacionChofer>
    {
        public ProcesadorCrearInhabilitacionChofer(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override InhabilitacionChofer CrearEntidad(CrearInhabilitacionChofer comando)
        {
            comando.Dto.FechaHasta = comando.Dto.FechaHasta.Add(new TimeSpan(0, 23, 59, 59));
            var chofer = new InhabilitacionChofer
            {
                Chofer = Repositorio.Obtener<Chofer>(x => x.NumeroDeDocumento == comando.Dto.NumeroDeDocumento && x.TipoDocumentoIdentidad.Id == comando.Dto.TipoDocumentoIdentidadId),
                FechaDesde = comando.Dto.FechaDesde,
                FechaHasta = comando.Dto.FechaHasta,
                Motivo = comando.Dto.Motivo,
                Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId),
                NombreUsuarioResponsable = comando.Dto.NombreUsuarioResponsable,
            };

            chofer.Adjuntos = new List<Adjunto>();
            if(comando.Dto.Adjuntos != null)
            {
                foreach (var archivo in comando.Dto.Adjuntos)
                {
                    chofer.Adjuntos.Add(Conversor.Convertir<AdjuntoDto, Adjunto>(archivo));
                }
            }

            return chofer;
        }

        protected override void Validar(CrearInhabilitacionChofer comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Chofer>(x => x.NumeroDeDocumento == comando.Dto.NumeroDeDocumento && x.TipoDocumentoIdentidad.Id == comando.Dto.TipoDocumentoIdentidadId))
            {
                resultado.Error("NumeroDeDocumento", Textos.InhabilitacionChofer_ChoferNoExiste);
            }
            if (!Repositorio.Existe<Centro>(x => x.Id == comando.Dto.CentroId))
            {
                resultado.Error("CentroId", Textos.InhabilitacionChofer_CentroNoExiste);
            }
        }
    }
}
