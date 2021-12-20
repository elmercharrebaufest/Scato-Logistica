using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEntregador : ProcesadorCrear<CrearEntregador, Entregador>
    {
        public ProcesadorCrearEntregador(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Entregador CrearEntidad(CrearEntregador comando)
        {
            return new Entregador
            {
                Analisis = comando.Dto.Analisis,
                DescripcionCorta = comando.Dto.DescripcionCorta,
                CodigoSAPCondicionFiscal = comando.Dto.CodigoSAPCondicionFiscal,
                Cuil = comando.Dto.Cuil,
                Domicilio = comando.Dto.Domicilio,
                EnvioAutomaticoMail = comando.Dto.EnvioAutomaticoMail,
                Pais = Repositorio.Obtener<Pais>(x => x.Id == (comando.Dto.PaisId ?? 0)),
                Provincia = Repositorio.Obtener<Provincia>(x => x.Id == (comando.Dto.ProvinciaId ?? 0)),
                Localidad = Repositorio.Obtener<Localidad>(x => x.Id == (comando.Dto.LocalidadId ?? 0)),
                Mail = comando.Dto.Mail,
                Pesada = comando.Dto.Pesada,
                RazonSocial = comando.Dto.RazonSocial,
                TipoEntregador = comando.Dto.TipoEntregador,
                ToleranciaEnKg = comando.Dto.ToleranciaEnKg ,
                ToleranciaEnPorcentaje = comando.Dto.ToleranciaEnPorcentaje,
                Tratamiento = comando.Dto.Tratamiento,
                Activo = comando.Dto.Activo
            };
        }

        protected override void Validar(CrearEntregador comando, Resultado resultado)
        {
            if (
                Repositorio.Existe<Entregador>(
                    x =>
                    x.Id != comando.Dto.Id && (x.DescripcionCorta == comando.Dto.DescripcionCorta)))
            {
                resultado.Error("DescripcionCorta", Textos.Entregador_DescripcionCortaExistente);
            }

            if (
                Repositorio.Existe<Entregador>(
                    x =>
                    x.Id != comando.Dto.Id && (x.RazonSocial == comando.Dto.RazonSocial)))
            {
                resultado.Error("RazonSocial", Textos.Entregador_RazonSocial);
            }

            if (
                Repositorio.Existe<Entregador>(
                    x =>
                    x.Id != comando.Dto.Id && x.Cuil == comando.Dto.Cuil))
            {
                resultado.Error("Cuil", Textos.Entregador_Cuil);
            }
        }
    }
}