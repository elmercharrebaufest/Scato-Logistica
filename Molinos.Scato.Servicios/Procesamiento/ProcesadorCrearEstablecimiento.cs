using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEstablecimiento : ProcesadorCrear<CrearEstablecimiento, Establecimiento>
    {
        public ProcesadorCrearEstablecimiento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Establecimiento CrearEntidad(CrearEstablecimiento comando)
        {
            var ent = new Establecimiento
            {
                Proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorId),
                NombreDeEstablecimiento = comando.Dto.NombreDeEstablecimiento,
                CodigoDeEstablecimiento = comando.Dto.CodigoDeEstablecimiento,
                Provincia = Repositorio.Obtener<Provincia>(x => x.Id == comando.Dto.ProvinciaId),
                Localidad = Repositorio.Obtener<Localidad>(x => x.Id == comando.Dto.LocalidadId),
                Domicilio = comando.Dto.Domicilio,
                CodigoPostal = comando.Dto.CodigoPostal,
                Anulado = comando.Dto.Anulado
            };
            return ent;
        }

        protected override void Validar(CrearEstablecimiento comando, Resultado resultado)
        {
            if (Repositorio.Existe<Establecimiento>(x => x.Proveedor.Id == comando.Dto.ProveedorId && x.NombreDeEstablecimiento == comando.Dto.NombreDeEstablecimiento))
            {
                resultado.Error("NombreDeEstablecimiento", string.Format(Textos.Error_Existente,Textos.Establecimiento_Nombre));
            }
        }
    }
}