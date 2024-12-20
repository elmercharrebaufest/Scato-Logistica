using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Estrategias
{
    public class BalanzadaFinStrategy : IBalanzadaStrategy
    {
        private readonly IServicioCarga _servicioCarga;

        public BalanzadaFinStrategy(IServicioCarga servicioCarga)
        {
            _servicioCarga = servicioCarga;
        }

        public string Nombre => "fin";

        public bool RegistrarBalanzada(Dictionary<string, string> datos)
        {
            var balanzada = _servicioCarga.ConvertirDatosABalanazadaRecibida(datos);
            var resultado = _servicioCarga.CrearCargaPendiente(balanzada);
            if (!resultado.HayErrores)
            {
                _servicioCarga.CrearCargaFin(balanzada);
            }
            _servicioCarga.ActualizarUltimaValidacion(balanzada);
            return true;
        }
    }
}