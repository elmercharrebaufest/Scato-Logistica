using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Estrategias
{
    public class BalanzadaContext : IBalanzadaContext
    {
        private readonly IEnumerable<IBalanzadaStrategy> _strategies;

        public BalanzadaContext(IEnumerable<IBalanzadaStrategy> strategies)
        {
            this._strategies = strategies;
        }

        public IBalanzadaStrategy GetStrategy(string tipoBalanzada)
        {
            return _strategies.FirstOrDefault(x =>
                x.Nombre.Equals(tipoBalanzada, StringComparison.InvariantCultureIgnoreCase))
                ?? _strategies.FirstOrDefault(x =>
                    x.Nombre.Equals("error", StringComparison.InvariantCultureIgnoreCase));
        }

    }
}