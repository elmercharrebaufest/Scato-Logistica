using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CantidadCamionesHoraConsulta : IConsultaEscalar<GraficoCamionesHoraDto>
    {
        private readonly DateTime fechaHistorico;
        private readonly DateTime fechaActual;
        private readonly int centroId;
        private readonly int materialId;

        private GraficoCamionesHoraDto modelo;

        public CantidadCamionesHoraConsulta(GraficoCamionesHoraDto model, int centroId)
        {
            modelo = model;
            fechaHistorico = model.FechaVieja;
            fechaActual = DateTime.Now;
            this.centroId = centroId;
            materialId = model.MaterialId;
        }

        public GraficoCamionesHoraDto Ejecutar(DbContext contexto)
        {
            var consulta = @"select count(*) as Valor, Clave from (
                                
								select DATEPART(hour, pesotarafecha) as Clave
                        from Recorrido WITH (NOLOCK)
                        where centro_id = @centroId  and rechazado = 0 and pesotarafecha is not null and
                            0 = (select TipoDeWorkflow from Workflow WITH (NOLOCK) where id = Recorrido.Workflow_Id) and 
                            (@materialId = 0 or Material_Id = @materialId) and cast(pesotarafecha as date) = cast(@fecha as date)

                        ) as horas
                    group by Clave";
            var query = contexto.Database.SqlQuery<ClaveValorDto>(consulta,
                new SqlParameter("@fecha", fechaHistorico.Date),
                new SqlParameter("@materialId", materialId),
                new SqlParameter("@centroId", centroId));

            var historico = query.ToDictionary(r => r.Clave, r => r.Valor);
            
            int[] salida = new int[24];

            for (int i = 0; i < salida.Length; i++)
            {
                if (historico.ContainsKey(i))
                {
                    salida[i] = historico[i];
                }
            }
            modelo.CamionesHistorico = salida;

            query = contexto.Database.SqlQuery<ClaveValorDto>(consulta, new SqlParameter("@fecha", fechaActual.Date),
                new SqlParameter("@materialId", materialId),
                new SqlParameter("@centroId", centroId));
           
            var actual = query.ToDictionary(r => r.Clave, r => r.Valor);

            var salidaActual = new int[fechaActual.Hour + 1];

            for (int i = 0; i < salidaActual.Length; i++)
            {
                if (actual.ContainsKey(i))
                {
                    salidaActual[i] = actual[i];
                }
            }
            modelo.CamionesActuales = salidaActual;
            return modelo;
        }
    }
}
