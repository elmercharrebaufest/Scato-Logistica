using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorSincronizarMateriales : ProcesadorSincronizar<SincronizarMateriales>
    {
        public ProcesadorSincronizarMateriales(IRepositorio repositorio, IConversor conversor, ZSDWS_SCATO servicioSap, ILogger log)
            : base(repositorio, conversor, servicioSap, log)
        {
        }

        protected override int ProcesarSincronizacion(SincronizarMateriales comando, DateTime fechaEjecucion, out Resultado resultado)
        {
            resultado = new ResultadoSincronizarMateriales();
            var centros = Repositorio.Listar<Centro>();
            var materialesSAP = Enumerable.Empty<ZSDES5211>();
            var materialesDescargados = 0;
            foreach (var centro in centros)
            {
                var idCentro = centro.Id;
                var esNuevoCentro = !Repositorio.Existe<MaterialPorCentro>(m => m.Centro.Id == idCentro);
                // Esto es porque si es un centro nuevo debe traer todos los materiales para el centro
                var fecha = (esNuevoCentro ? MinimaFechaEjecucion : fechaEjecucion).ToString("yyyy-MM-dd");
                var datosSap = ServicioSap.DatosMateriales(
                    new DatosMaterialesRequest(new DatosMateriales
                    {
                        Centro = centro.CodigoSAP,
                        Fecha = fecha,
                        IdSAP = string.Empty
                    }));
                Log.Debug("Materiales descargados para el centro {0}: {1}", 
                    centro.CodigoSAP, datosSap.DatosMaterialesResponse.Materiales.Length);
                materialesDescargados += datosSap.DatosMaterialesResponse.Materiales.Length;
                materialesSAP = materialesSAP.Union(datosSap.DatosMaterialesResponse.Materiales);
            }
            Log.Debug("Total materiales por centro descargados: {0}", materialesDescargados);
            var materialesPorCodigo = materialesSAP.GroupBy(m => m.ID_SAP, (cod, mat) => mat.ToList()).ToList();
            Log.Debug("Total materiales descargados (por código): {0}", materialesPorCodigo.Count);
            Log.Debug("Comenzando sincronización...");
            var cantidad = 0;
            foreach (var materialPorCodigo in materialesPorCodigo)
            {
                var materialSap = materialPorCodigo.First();
                materialSap.ID_SAP = materialSap.ID_SAP.TrimStart(new[] {'0'});
                var material = Repositorio.Obtener<Material>(x => x.CodigoSAP == materialSap.ID_SAP);
                if (material == null)
                {
                    Log.Debug("Sincronización - Insertando nuevo material: {0} - {1}.", materialSap.ID_SAP, materialSap.DESCRIPCION);
                    material = new Material();
                    ActualizarMaterial(material, materialSap);
                    material = Repositorio.Agregar(material);
                }
                else
                {
                    Log.Debug("Sincronización - Actualizando material: {0}.", materialSap.DESCRIPCION);
                    ActualizarMaterial(material, materialSap);
                }
                cantidad++;
                if (comando.RetornarResultado)
                {
                    ((ResultadoSincronizarMateriales)resultado).AgregarResultado(Conversor.Convertir<Material, MaterialDto>(material));
                }
                foreach (var materialCentro in materialPorCodigo)
                {
                    var codigoCentro = materialCentro.WERKS;
                    var existeMaterialPorCentro = Repositorio.Existe<MaterialPorCentro>(x => x.Material.CodigoSAP == materialSap.ID_SAP && x.Centro.CodigoSAP == codigoCentro);
                    if (!existeMaterialPorCentro)
                    {
                        Log.Debug("Sincronización - Insertando nuevo material por centro: {0} - {1}.", codigoCentro, materialSap.ID_SAP);
                        var centro = centros.First(c => c.CodigoSAP == codigoCentro);
                        var materialPorCentro = new MaterialPorCentro { Material = material, Centro = centro };
                        Repositorio.Agregar(materialPorCentro);
                    }
                }
                if (cantidad%1000 == 0)
                {
                    Log.Debug("Procesados {0}/{1}. Guardando cambios...", cantidad, materialesDescargados);
                    Repositorio.GuardarCambios();
                }
            }

            ((ResultadoSincronizarMateriales)resultado).Cantidad = cantidad;

            return cantidad;
        }

        protected override string NombreInterface()
        {
            return "Materiales";
        }

        private static void ActualizarMaterial(Material material, ZSDES5211 materialSap)
        {
            material.CodigoSAP = materialSap.ID_SAP;
            material.UnidadDeMedidad = materialSap.MEINS;
            material.Descripcion = materialSap.DESCRIPCION;
            material.Activo = materialSap.ACTIVO == "X";
            material.PesoTeoricoSap = materialSap.NTGEW;
        }
    }
}
