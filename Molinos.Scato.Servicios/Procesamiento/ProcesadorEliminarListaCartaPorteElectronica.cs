using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarListaCartaPorteElectronica : ProcesadorComando<EliminarListaCartaPorteElectronica>
    {
        public ProcesadorEliminarListaCartaPorteElectronica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
           
        }

        public override Resultado Ejecutar(EliminarListaCartaPorteElectronica comando)
        {
            var resultado = new Resultado();
            const string nombrePantalla = Constantes.ConfiguracionGeneral.Pantalla.LimpiarCacheCartaPorte;

            try
            {
                int diasInicio = ObtenerConfiguracionInt(
                    nombrePantalla,
                    Constantes.ConfiguracionGeneral.CartaPorteElectronica.DiasInicioDeBusquedaDeRecorrido
                );

                int registrosEliminados = comando.SonCartasIngresadas
                    ? EliminarCartasIngresadas(diasInicio)
                    : EliminarCartasNoIngresadas(nombrePantalla, diasInicio);

                Log.Info($"Se eliminaron {registrosEliminados} registros de Carta Porte Electrónica " +
                         $"{(comando.SonCartasIngresadas ? "ingresados" : "no ingresados")}.");

                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al eliminar lista de Carta Porte Electrónica");
                resultado.Errores.Add(
                    nameof(EliminarListaCartaPorteElectronica),
                    "Error al eliminar la lista de Carta Porte Electrónica."
                );
                return resultado;
            }
        }

        private int ObtenerConfiguracionInt(string pantalla, string nombre)
        {
            var valor = Repositorio.Obtener<ConfiguracionGeneral>(
                x => x.Pantalla == pantalla && x.Nombre == nombre
            )?.Valor;

            if (!int.TryParse(valor, out int resultado))
                throw new InvalidOperationException(
                    $"Configuración inválida: {pantalla} - {nombre}"
                );

            return resultado;
        }

        private int EliminarCartasIngresadas(int diasInicio)
        {
            return Repositorio.EliminaCartaPorteElectronicaDocumentoIngresados(diasInicio);
        }

        private int EliminarCartasNoIngresadas(string nombrePantalla, int diasInicio)
        {
            int diasLimite = ObtenerConfiguracionInt(
                nombrePantalla,
                Constantes.ConfiguracionGeneral.CartaPorteElectronica.DiasLimiteDeBusqueda
            );

            return Repositorio.EliminaCartaPorteElectronicaDocumentosNoIngresados(
                diasLimite,
                diasInicio
            );
        }

    }
}