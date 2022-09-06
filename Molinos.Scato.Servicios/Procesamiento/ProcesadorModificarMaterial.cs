using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMaterial : ProcesadorModificar<ModificarMaterial>
    {
        public ProcesadorModificarMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMaterial comando)
        {
            var materialEditado = Repositorio.Obtener<Material>(comando.Dto.Id);

            materialEditado.CodigoSAP = comando.Dto.CodigoSAP == null ? null : comando.Dto.CodigoSAP.TrimStart(new[] {'0'});
            materialEditado.Descripcion = comando.Dto.Descripcion;
            materialEditado.DescripcionCorta = comando.Dto.DescripcionCorta;
            materialEditado.CodigoEspecie = comando.Dto.CodigoEspecie;
            materialEditado.UnidadDeMedidad = comando.Dto.UnidadDeMedidad;
            materialEditado.FactorConversion = comando.Dto.FactorConversion;
            materialEditado.TipoDeGrano = comando.Dto.TipoDeGrano;
            materialEditado.CodigoONCCA = comando.Dto.CodigoONCCA;
            materialEditado.PesoTeoricoSap = comando.Dto.PesoTeoricoSap;
            materialEditado.RequiereNumeroTropa = comando.Dto.RequiereNumeroTropa;
            materialEditado.RequiereAnexoInase = comando.Dto.RequiereAnexoInase;
            materialEditado.UsaBinPallet = comando.Dto.UsaBinPallet;
            materialEditado.Clase = comando.Dto.Clase;
            materialEditado.Peso = comando.Dto.Peso;
            materialEditado.EsUva = comando.Dto.EsUva;
            materialEditado.Commodity = comando.Dto.Commodity;
            materialEditado.AlmacenOrigen = Repositorio.Obtener<Almacen>(Convert.ToInt32(comando.Dto.AlmacenOrigenId));
            materialEditado.EsCosecha = comando.Dto.EsCosecha;
            //materialEditado.Cosecha = comando.Dto.Cosecha;
            materialEditado.VigenciaDesde = comando.Dto.VigenciaDesde;
            materialEditado.VigenciaHasta = comando.Dto.VigenciaHasta;
            materialEditado.Contrato = comando.Dto.Contrato;
            materialEditado.Lote = comando.Dto.Lote;
            materialEditado.NirsCodigoProducto = comando.Dto.NirsCodigoProducto;
            materialEditado.Oleico = comando.Dto.Oleico;
            materialEditado.EsGrano = comando.Dto.EsGrano;
            materialEditado.EsInsumo = comando.Dto.EsInsumo;
            materialEditado.Color = comando.Dto.Color;

            if (comando.Dto.VariedadId.HasValue)
            {
                materialEditado.Variedad = Repositorio.Obtener<Variedad>(comando.Dto.VariedadId.Value);
            }

            if (comando.MaterialPorCentroDto.Id > 0)
            {
                var materialPorCentroEditado = Repositorio.Obtener<MaterialPorCentro>(comando.MaterialPorCentroDto.Id);

                materialPorCentroEditado.AlmacenPredeterminado = Repositorio.Obtener<Almacen>(Convert.ToInt32(comando.MaterialPorCentroDto.AlmacenPredId));
                materialPorCentroEditado.AnalisisInterno = comando.MaterialPorCentroDto.AnalisisInterno;
                materialPorCentroEditado.Camara = Repositorio.Obtener<Camara>(Convert.ToInt32(comando.MaterialPorCentroDto.CamaraId));
                materialPorCentroEditado.CorrespondeDescarga = comando.MaterialPorCentroDto.CorrespondeDescarga;
                materialPorCentroEditado.PorcentajeMuestraAuditoria = comando.MaterialPorCentroDto.PorcentajeMuestraAuditoria;
                materialPorCentroEditado.RequiereTecnologia = comando.MaterialPorCentroDto.RequiereTecnologia;
                materialPorCentroEditado.MaterialDeTerceros = comando.MaterialPorCentroDto.MaterialDeTerceros;
                materialPorCentroEditado.ImprimeReciboMunicipal = comando.MaterialPorCentroDto.ImprimeReciboMunicipal;
                materialPorCentroEditado.NoValidaCG = comando.MaterialPorCentroDto.NoValidaCG;
                materialPorCentroEditado.EpaStockPorCorte = comando.MaterialPorCentroDto.EpaStockPorCorte;
                materialPorCentroEditado.MostrarEnWebMobile = comando.MaterialPorCentroDto.MostrarEnWebMobile;
                materialPorCentroEditado.DescripcionWebMobile = comando.MaterialPorCentroDto.DescripcionWebMobile;
                materialPorCentroEditado.Orden = comando.MaterialPorCentroDto.Orden;
                materialPorCentroEditado.IgnoraContingencia = comando.MaterialPorCentroDto.IgnoraContingencia;
                Log.Debug("Se modificó el material por centro: {0}, valor ImprimeReciboMunicipal de: {1} a {2}, Usuario: {3}", comando.MaterialPorCentroDto.Id, materialPorCentroEditado.ImprimeReciboMunicipal, comando.MaterialPorCentroDto.ImprimeReciboMunicipal, comando.Usuario);

            }
            else
            {
                var materialPorCentroEditado = Conversor.Convertir<MaterialPorCentroDto, MaterialPorCentro>(comando.MaterialPorCentroDto);
                materialPorCentroEditado.Material = Repositorio.Obtener<Material>(comando.MaterialPorCentroDto.MaterialId);
                materialPorCentroEditado.Centro = Repositorio.Obtener<Centro>(comando.MaterialPorCentroDto.CentroId);
                materialPorCentroEditado.AlmacenPredeterminado = Repositorio.Obtener<Almacen>(comando.MaterialPorCentroDto.AlmacenPredId);
                materialPorCentroEditado.AnalisisInterno = comando.MaterialPorCentroDto.AnalisisInterno;
                materialPorCentroEditado.Camara = Repositorio.Obtener<Camara>(Convert.ToInt32(comando.MaterialPorCentroDto.CamaraId));
                materialPorCentroEditado.PorcentajeMuestraAuditoria = comando.MaterialPorCentroDto.PorcentajeMuestraAuditoria;
                materialPorCentroEditado.RequiereTecnologia = comando.MaterialPorCentroDto.RequiereTecnologia;
                materialPorCentroEditado.MaterialDeTerceros = comando.MaterialPorCentroDto.MaterialDeTerceros;
                materialPorCentroEditado.ImprimeReciboMunicipal = comando.MaterialPorCentroDto.ImprimeReciboMunicipal;
                materialPorCentroEditado.NoValidaCG = comando.MaterialPorCentroDto.NoValidaCG;
                materialPorCentroEditado.EpaStockPorCorte = comando.MaterialPorCentroDto.EpaStockPorCorte;
                materialPorCentroEditado.MostrarEnWebMobile = comando.MaterialPorCentroDto.MostrarEnWebMobile;
                materialPorCentroEditado.DescripcionWebMobile = comando.MaterialPorCentroDto.DescripcionWebMobile;
                materialPorCentroEditado.Orden = comando.MaterialPorCentroDto.Orden;
                materialPorCentroEditado.IgnoraContingencia = comando.MaterialPorCentroDto.IgnoraContingencia;
                Repositorio.Agregar(materialPorCentroEditado);

                Log.Debug("Se creó el material por centro: {0}, valor ImprimeReciboMunicipal de: {1} a {2}, Usuario: {3}", comando.MaterialPorCentroDto.Id, materialPorCentroEditado.ImprimeReciboMunicipal, comando.MaterialPorCentroDto.ImprimeReciboMunicipal, comando.Usuario);
            }
        }

        protected override void Validar(ModificarMaterial comando, Resultado resultado)
        {
            if (Repositorio.Existe<MaterialPorCentro>(e => e.Material.Descripcion == comando.Dto.Descripcion && (e.Material.Id != comando.Dto.Id) && e.Centro.Id == comando.MaterialPorCentroDto.CentroId))
            {
                resultado.Error("Descripcion", Textos.Material_DescripcionExistente);
            }
            if (comando.Dto.CodigoSAP != null)
            {
                var codigoSAP = comando.Dto.CodigoSAP.TrimStart(new[] {'0'});
                if (Repositorio.Existe<Material>(e => e.CodigoSAP == codigoSAP && (e.Id != comando.Dto.Id)))
                {
                    resultado.Error("CodigoSAP", Textos.Material_CodigoSAPExistente);
                }
            }
            if (comando.Dto.EsUva && !comando.Dto.VariedadId.HasValue)
            {
                resultado.Error("VariedadId", Textos.Error_Requerido);
            }
            if (comando.MaterialPorCentroDto.PorcentajeMuestraAuditoria.HasValue && (comando.MaterialPorCentroDto.PorcentajeMuestraAuditoria.Value > 100 || comando.MaterialPorCentroDto.PorcentajeMuestraAuditoria.Value < 0))
            {
                resultado.Error("PorcentajeMuestraAuditoria", Textos.Error_RangoPorcentaje);
            }
            if (comando.MaterialPorCentroDto.AnalisisInterno.HasValue && (comando.MaterialPorCentroDto.AnalisisInterno.Value > 100 || comando.MaterialPorCentroDto.AnalisisInterno.Value < 0))
            {
                resultado.Error("AnalisisInterno", Textos.Error_RangoPorcentaje);
            }
        }
    }
}
