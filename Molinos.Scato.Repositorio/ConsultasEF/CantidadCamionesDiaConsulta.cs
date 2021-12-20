using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CantidadCamionesDiaConsulta : IConsultaEscalar<GraficoCamionesDiaDto>
    {
        private readonly int centroId;
        private readonly int materialId;

        private GraficoCamionesDiaDto modelo;

        public CantidadCamionesDiaConsulta(GraficoCamionesDiaDto model, int centroId)
        {
            modelo = model;
            this.centroId = centroId;
            materialId = model.MaterialId;
        }

        public GraficoCamionesDiaDto Ejecutar(DbContext contexto)
        {
            var consulta = @"select count(*) as Valor, Clave from (
										select DATEDIFF(DAY, pesotarafecha,GETDATE()) as Clave from Recorrido WITH (NOLOCK)
                                where centro_id = @centroId and rechazado = 0 and
                                    0 = (select TipoDeWorkflow from Workflow WITH (NOLOCK) where id = Recorrido.Workflow_Id) and material_id = @materialId and pesotarafecha is not null and
                                    DATEDIFF(DAY, cast(pesotarafecha as date),GETDATE()) <= 31
                        ) as horas
							  group by Clave";
            var query = contexto.Database.SqlQuery<ClaveValorDto>(consulta,
                new SqlParameter("@materialId", materialId),
                new SqlParameter("@centroId", centroId));

           var actual = query.ToDictionary(r => r.Clave, r => r.Valor);

            var salida = new int[31];
            int contador = 0;

            for (int i = salida.Length - 1; i >= 0; i--)
            {
                if (actual.ContainsKey(i))
                {
                    salida[contador] = actual[i];
                }
                contador++;
            }
            modelo.CamionesDia = salida;
            return modelo;
        }
    }
}
