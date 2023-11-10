using System;
using System.Data.SqlClient;

public class DatabaseHelper
{
    public static void ExecuteWithConnection(string connectionString, Action<SqlConnection> action)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                // Abre la conexión
                connection.Open();

                // Ejecuta la acción pasando la conexión como argumento
                action(connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Asegúrate de cerrar la conexión cuando hayas terminado
                connection.Close();
            }
        }
    }
}
