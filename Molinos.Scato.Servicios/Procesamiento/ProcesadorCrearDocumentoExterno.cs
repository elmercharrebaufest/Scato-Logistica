using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDocumentoExterno : ProcesadorCrear<CrearDocumentoExterno, DocumentoExterno>
    {
        protected readonly IServicioRepositorio servicio;

        public ProcesadorCrearDocumentoExterno(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioRepositorio servicio) : base(repositorio, conversor, log)
        {
            this.servicio = servicio;
        }

        protected override DocumentoExterno CrearEntidad(CrearDocumentoExterno comando)
        {
            return new DocumentoExterno
            {
                NumeroDeDocumento = comando.Dto.NumeroDeDocumento.PadLeft(12,'0'),
                TipoDocumentoIngreso = Dominio.Enums.TipoDocumentoIngreso.CartaPorte,
                ArchivoRutaDestino = comando.Dto.ArchivoRutaDestino,
                ArchivoExtension = comando.Dto.ArchivoExtension,
                Fecha = DateTime.Now
            };
        }

        protected override void Validar(CrearDocumentoExterno comando, Resultado resultado)
        {
            var num = comando.Dto.NumeroDeDocumento.PadLeft(12, '0');
            if (
                Repositorio.Existe<DocumentoExterno>(
                    x =>
                    x.Id != comando.Dto.Id && x.NumeroDeDocumento == num))
            {
                resultado.Error("", string.Format(Textos.Error_CpExistente, num));
            }
        }
    }
}