using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearFormatoDeImpresion : ProcesadorCrear<CrearFormatoDeImpresion, FormatoDeImpresion>
    {
        public ProcesadorCrearFormatoDeImpresion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override FormatoDeImpresion CrearEntidad(CrearFormatoDeImpresion comando)
        {
            var entidad = Conversor.Convertir<FormatoDeImpresionDto, FormatoDeImpresion>(comando.Dto);
            entidad.FormatoDePapel = Repositorio.Obtener<FormatoDePapel>(comando.Dto.FormatoDePapelId);
            
            entidad.FormatosDeCampo.Clear();
            foreach (var formato in comando.Dto.FormatosDeCampo)
            {
                if (!formato._destroy)
                {
                    var formatoCampo = Conversor.Convertir<FormatoDeCampoDto, FormatoDeCampo>(formato);
                    formatoCampo.Letra = Repositorio.Obtener<Letra>(x => x.Id == formato.LetraId);
                    formatoCampo.Campo = Repositorio.Obtener<Campo>(x => x.Id == formato.CampoId);
                    formatoCampo.FormatoDeImpresion = entidad;
                    entidad.FormatosDeCampo.Add(formatoCampo);
                }
            }



            return entidad;
        }

        protected override void Validar(CrearFormatoDeImpresion comando, Resultado resultado)
        {
        }
    }
}
