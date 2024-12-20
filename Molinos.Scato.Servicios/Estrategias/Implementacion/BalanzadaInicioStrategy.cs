using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Estrategias
{
    public class BalanzadaInicioStrategy : IBalanzadaStrategy
    {
        private readonly IServicioCarga _servicioCarga;

        public BalanzadaInicioStrategy(IServicioCarga servicioCarga)
        {
            _servicioCarga = servicioCarga;
        }

        public string Nombre => "inicio";

        public bool RegistrarBalanzada(Dictionary<string, string> datos)
        {
            var balanzada = _servicioCarga.ConvertirDatosABalanazadaRecibida(datos);
            var resultado = _servicioCarga.CrearCargaPendiente(balanzada);
            if (!resultado.HayErrores)
            {
                _servicioCarga.CrearCargaInicio(balanzada);
            }
            _servicioCarga.ActualizarUltimaValidacion(balanzada);
            return true;
        }
    }
}