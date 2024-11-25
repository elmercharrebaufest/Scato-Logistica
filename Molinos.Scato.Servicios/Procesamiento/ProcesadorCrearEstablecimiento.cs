using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;

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
                Anulado = comando.Dto.Anulado,
                EPA = comando.Dto.EsSojaEPA,
                EsStandard2 = comando.Dto.EsStandard2,
                EsProvisorio = comando.Dto.EsProvisorio,
                Observaciones = comando.Dto.Observaciones,
                Comercial = Repositorio.Obtener<Comercial>(x=>x.Id == comando.Dto.ComercialId),
                CorredoresAsociados = comando.Dto.CorredoresAsociados==null?null:comando.Dto.CorredoresAsociados.Select(proveedorDto => Repositorio.ObtenerUnchanged<Proveedor>(proveedorDto.Id)).ToList(),
                EsEUDR = comando.Dto.EsEUDR,
            };
            return ent;
        }

        protected override void Validar(CrearEstablecimiento comando, Resultado resultado)
        {
            if (!string.IsNullOrEmpty(comando.Dto.CodigoRENSPA) && !Regex.IsMatch(comando.Dto.CodigoRENSPA, @"^\d{2}\.\d{3}\.\d\.\d{5}/\d{2}$"))
            {
                resultado.Error("CodigoRENSPA", Textos.Establecimiento_Error_FormatoCodigoRENSPA);
            }

            if (Repositorio.Existe<Establecimiento>(x => x.Proveedor.Id == comando.Dto.ProveedorId && x.NombreDeEstablecimiento == comando.Dto.NombreDeEstablecimiento))
            {
                resultado.Error("NombreDeEstablecimiento", string.Format(Textos.Error_Existente,Textos.Establecimiento_Nombre));
            }
        }
    }
}