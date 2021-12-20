using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarOrdenDeDescargaFason : ProcesadorModificar<ModificarOrdenDeDescargaFason>
    {
        public ProcesadorModificarOrdenDeDescargaFason(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarOrdenDeDescargaFason comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var procedencia = Repositorio.Obtener<Localidad>(comando.Orden.ProcedenciaId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var cliente = Repositorio.Obtener<Cliente>(comando.Orden.ClienteId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);

            var ordenDeDescarga = Repositorio.Obtener<OrdenDeDescargaFason>(comando.Orden.Id);

            ordenDeDescarga.Chofer = chofer;
            ordenDeDescarga.Procedencia = procedencia;
            ordenDeDescarga.TipoComercial = tipoComercial;
            ordenDeDescarga.Transportista = transportista;
            ordenDeDescarga.PatenteCamion = comando.Orden.PatenteCamion;
            ordenDeDescarga.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            ordenDeDescarga.Material = material;
            ordenDeDescarga.Cliente = cliente;
            ordenDeDescarga.PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen;
            ordenDeDescarga.PesoNetoOrigen = comando.Orden.PesoNetoOrigen;
            ordenDeDescarga.PesoTaraOrigen = comando.Orden.PesoTaraOrigen;
            ordenDeDescarga.FechaOD = comando.Orden.FechaOD;
            ordenDeDescarga.NumeroRemito = comando.Orden.NumeroRemito;

            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            recorrido.PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen;
            recorrido.PesoTaraOrigen = comando.Orden.PesoTaraOrigen;
        }

        protected override void Validar(ModificarOrdenDeDescargaFason comando, Resultado resultado)
        {
        }
    }
}
