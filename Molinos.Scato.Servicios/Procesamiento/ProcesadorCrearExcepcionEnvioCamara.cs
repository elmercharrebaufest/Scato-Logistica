using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearExcepcionEnvioCamara : ProcesadorCrear<CrearExcepcionEnvioCamara, ExcepcionEnvioCamara>
    {
        public ProcesadorCrearExcepcionEnvioCamara(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ExcepcionEnvioCamara CrearEntidad(CrearExcepcionEnvioCamara comando)
        {
            return new ExcepcionEnvioCamara
            {
                CaracteristicaMaterial = Repositorio.Obtener<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaId),
                Entregador = Repositorio.Obtener<Entregador>(x => x.Id == comando.Dto.EntregadorId),
                Material = Repositorio.Obtener<Material>(x => x.Id == comando.Dto.MaterialId),
                Proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorId),
                TipoComercial = Repositorio.Obtener<TipoComercial>(x => x.Id == comando.Dto.TipoComercialId),
            };
        }

        protected override void Validar(CrearExcepcionEnvioCamara comando, Resultado resultado)
        {
            if (comando.Dto.ProveedorId != 0 && !Repositorio.Existe<Proveedor>(x => x.Id == comando.Dto.ProveedorId))
            {
                resultado.Error("ProveedorDesc", Textos.Error_Invalido);
            }
            if (comando.Dto.EntregadorId != 0 && !Repositorio.Existe<Entregador>(x => x.Id == comando.Dto.EntregadorId))
            {
                resultado.Error("EntregadorDesc", Textos.Error_Invalido);
            }
            if (comando.Dto.CaracteristicaId == 0 || !Repositorio.Existe<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaId))
            {
                resultado.Error("CaracteristicaId", string.Format(Textos.Error_Requerido,Textos.Caracteristica));
            }

            if (
                Repositorio.Existe<ExcepcionEnvioCamara>(
                    x => x.Material.Id == comando.Dto.MaterialId && x.TipoComercial.Id == comando.Dto.TipoComercialId
                    && x.Proveedor.Id == comando.Dto.ProveedorId && x.Entregador.Id == comando.Dto.EntregadorId))
            {
                resultado.Error("CaracteristicasDeCalidadId", Textos.ExcepcionEnvioCamara_Existente);
            }
        }
    }
}
