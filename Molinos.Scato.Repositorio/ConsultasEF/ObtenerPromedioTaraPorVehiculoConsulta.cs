using System.Data.Entity;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerPromedioTaraPorVehiculoConsulta : IConsultaEscalar<PromedioTaraVehiculoDto>
    {
        private readonly int recorridoId;

        private const string SqlBase = @"
            SELECT 
                vv.Patente1 AS PatenteCamion,
                vv.Patente2 AS PatenteAcoplado,
                vv.Patente3 AS PatenteAcoplado2,
                UPPER(ISNULL(ch.Apellido, '')) AS ChoferApellido,
                UPPER(ISNULL(ch.Nombre, '')) AS ChoferNombre,
                ISNULL(ch.NumeroDeDocumento, '') AS ChoferNumeroDocumento,
                ISNULL(m.CodigoSAP, '') AS CodigoSapMaterial
            FROM Recorrido r
            INNER JOIN VehiculoView vv
                ON vv.Recorrido_Id = r.Id
            LEFT JOIN Chofer ch
                ON ch.Id = r.Chofer_Id
            LEFT JOIN Material m
                ON m.Id = r.Material_Id
            WHERE r.Id = @RecorridoId"; 

        private const string SqlPromedio = @"
            SELECT AVG(T.PesoTara) AS PromedioTara
            FROM (
                SELECT TOP (3) r.PesoTara
                FROM VehiculoView vv
                INNER JOIN Recorrido r
                    ON r.Id = vv.Recorrido_Id
                WHERE vv.Patente1 = @PatenteCamion
                  AND ISNULL(vv.Patente2, '') = ISNULL(@PatenteAcoplado, '')
                  AND ISNULL(vv.Patente3, '') = ISNULL(@PatenteAcoplado2, '')
                  AND r.Terminado = 1
                  AND r.Rechazado = 0
                ORDER BY r.Id DESC
            ) AS T ";

        public ObtenerPromedioTaraPorVehiculoConsulta(int recorridoId)
        {
            this.recorridoId = recorridoId;
        }

        public PromedioTaraVehiculoDto Ejecutar(DbContext contexto)
        {
            var baseData = contexto.Database
                .SqlQuery<BasePromedioTaraVehiculoData>(
                    SqlBase,
                    new SqlParameter("@RecorridoId", recorridoId))
                .FirstOrDefault();

            if (baseData == null)
            {
                return new PromedioTaraVehiculoDto { PromedioTara = 0 };
            }

            var codigosSapMateriales = ObtenerCodigosSapMaterialesConfigurados(contexto);
            if (!CorrespondeControlarMaterial(baseData.CodigoSapMaterial, codigosSapMateriales))
            {
                return Convertir(baseData, 0);
            }

            var promedio = contexto.Database
                .SqlQuery<int>(
                    SqlPromedio,
                    new SqlParameter("@PatenteCamion", baseData.PatenteCamion ?? string.Empty),
                    new SqlParameter("@PatenteAcoplado", baseData.PatenteAcoplado ?? string.Empty),
                    new SqlParameter("@PatenteAcoplado2", baseData.PatenteAcoplado2 ?? string.Empty))
                .FirstOrDefault();

            return Convertir(baseData, promedio);
        }

        private static PromedioTaraVehiculoDto Convertir(BasePromedioTaraVehiculoData baseData, int promedioTara)
        {
            return new PromedioTaraVehiculoDto
            {
                PatenteCamion = baseData.PatenteCamion,
                PatenteAcoplado = baseData.PatenteAcoplado,
                PatenteAcoplado2 = baseData.PatenteAcoplado2,
                ChoferApellido = baseData.ChoferApellido,
                ChoferNombre = baseData.ChoferNombre,
                ChoferNumeroDocumento = baseData.ChoferNumeroDocumento,
                PromedioTara = promedioTara
            };
        }

        private static ISet<string> ObtenerCodigosSapMaterialesConfigurados(DbContext contexto)
        {
            var valorConfiguracion = contexto.Set<ConfiguracionGeneral>()
                .Where(config => config.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.DiferenciaPesoTaraWFE
                    && config.Nombre == Constantes.ConfiguracionGeneral.DiferenciaPesoTaraWFE.CodigoSAPMateriales
                    && config.CentroId == null)
                .Select(config => config.Valor)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(valorConfiguracion))
            {
                return new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            }

            return new HashSet<string>(
                valorConfiguracion
                    .Split(',')
                    .Select(codigo => codigo.Trim())
                    .Where(codigo => !string.IsNullOrWhiteSpace(codigo)),
                System.StringComparer.OrdinalIgnoreCase);
        }

        private static bool CorrespondeControlarMaterial(string codigoSapMaterial, ISet<string> codigosSapMateriales)
        {
            return !string.IsNullOrWhiteSpace(codigoSapMaterial)
                && codigosSapMateriales != null
                && codigosSapMateriales.Count > 0
                && codigosSapMateriales.Contains(codigoSapMaterial.Trim());
        }

        private class BasePromedioTaraVehiculoData
        {
            public string PatenteCamion { get; set; }
            public string PatenteAcoplado { get; set; }
            public string PatenteAcoplado2 { get; set; }
            public string ChoferApellido { get; set; }
            public string ChoferNombre { get; set; }
            public string ChoferNumeroDocumento { get; set; }
            public string CodigoSapMaterial { get; set; }
        }
    }
}