using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Estrategias
{
    public class BalanzadaErrorStrategy : IBalanzadaStrategy
    {
        private readonly IServicioCarga _servicioCarga;

        public BalanzadaErrorStrategy(IServicioCarga servicioCarga)
        {
            _servicioCarga = servicioCarga;
        }

        public string Nombre => "error";

        public bool RegistrarBalanzada(Dictionary<string, string> datos)
        {
            var balanzada = _servicioCarga.ConvertirDatosABalanazadaRecibida(datos);
            var resultado = _servicioCarga.CrearCargaPendiente(balanzada);
            if (!resultado.HayErrores)
            {
                _servicioCarga.CrearRegistroBalanzaPuerto(balanzada);
            }
            _servicioCarga.ActualizarUltimaValidacion(balanzada);
            return true;
        }
    }
}