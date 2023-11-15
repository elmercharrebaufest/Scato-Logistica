using Microsoft.AspNetCore.Mvc;
using Molinos.Scato.Dominio.Entidades;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace QaTools.Dao
{
    public class AdministracionDao : IAdministracionDao
    {
        private readonly IConfiguration configuration;

        public AdministracionDao(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public JsonResult EliminarRecorridos(IEnumerable<int> recorridos)
        {
            var errores = new List<string>();

            var totalEliminados = 0;

            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        foreach (var recorridoId in recorridos)
                        {

                            try
                            {
                                var query = string.Format(@"DECLARE @ajusteCalidadId INT;
                                DECLARE @vehiculo INT;
                                DECLARE @NoExisteCartaPorte BIT = 0;
                                DECLARE @cartaPorteId INT;

                                DECLARE @IDRecorrido INT = {0};

                                SET @ajusteCalidadId = (SELECT ac.Id
                                FROM Recorrido r
                                INNER JOIN AjusteDeCalidad ac ON r.TipoDocumentoIngreso = ac.TipoDocumentoIngreso AND r.NumeroDocumentoIngreso = ac.NumeroDocumentoIngreso
                                WHERE r.Id = @IDRecorrido);

                                --borro ajuste de calidad				   
                                IF @ajusteCalidadId IS NOT NULL
                                BEGIN
                                DELETE FROM AjusteDeCalidad
                                WHERE Id = @ajusteCalidadId;
                                END;

                                IF EXISTS (
                                SELECT 1
                                FROM CallePorRecorrido cpr
                                INNER JOIN Recorrido r ON cpr.Recorrido_Id = r.id
                                WHERE r.id = @IDRecorrido
                                )
                                --borro calle por recorrido
                                BEGIN
                                delete from CallePorRecorrido where Recorrido_Id = @IDRecorrido
                                END

                                SET @vehiculo = (SELECT Vehiculo_Id from Recorrido where id = @IDRecorrido)

                                IF (@vehiculo IS NOT NULL)

                                BEGIN

                                set @cartaPorteId = (select cp.Id as cpId from Recorrido r 
                                inner join Vehiculo v on r.Vehiculo_Id = v.Id
                                inner join CartaPorte cp on v.CartaPorte_Id = cp.Id
                                where r.Id = @IDRecorrido)
                                if NOT EXISTS(
                                select r.Id from Recorrido r 
                                inner join Vehiculo v on r.Vehiculo_Id = v.Id
                                inner join CartaPorte cp on v.CartaPorte_Id = cp.Id
                                where r.Id != @IDRecorrido and cp.Id = @cartaPorteId
                                )
                                begin
	                                set @NoExisteCartaPorte = 1;
                                end
                                END

                                --borrar recorrido
                                delete from recorrido where id = @IDRecorrido

                                --borrar vehiculo
                                IF @vehiculo IS NOT NULL
                                BEGIN

                                DELETE FROM Vehiculo
                                WHERE Id = @vehiculo;
                                END;

                                --borrar carta porte
                                IF @NoExisteCartaPorte = 1
                                begin

                                delete from CartaPorte where id = @cartaPorteId
                                end", recorridoId);

                                command.Connection = connection;
                                command.CommandText = query;

                                command.ExecuteNonQuery();

                                totalEliminados++;

                            }
                            catch (Exception ex)
                            {

                                errores.Add("no se elimino recorrido: " + recorridoId + " - error:" + ex.Message);
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }

            return new JsonResult(new { mensaje = "se realizo la eliminacion de " + totalEliminados + " recorridos", errores = errores });

        }

        public IEnumerable<int> GetRecorridos(int centroId, int workflowId)
        {
            var recorridosIds = new List<int>();

            string sqlQuery = string.Format("SELECT * FROM Recorrido where Centro_Id = {0} and Workflow_Id = {1}", centroId, workflowId);

            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("id"));
                                recorridosIds.Add(id);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }

            return recorridosIds;
        }
    }
}
