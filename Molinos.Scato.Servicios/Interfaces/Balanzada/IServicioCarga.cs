using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.Estrategias;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios
{
    public interface IServicioCarga
    {

        BalanzadaRecibidaDTO ConvertirDatosABalanazadaRecibida(Dictionary<string, string> datos);
        ResultadoCrear CrearCargaPendiente(BalanzadaRecibidaDTO balanzada);
        bool ExisteRegistroBalanzaPuerto(BalanzadaRecibidaDTO balanzada);
        ResultadoCrear CrearCargaInicio(BalanzadaRecibidaDTO balanzada);
        ResultadoCrear CrearBalanzada(BalanzadaRecibidaDTO balanzada);
        ResultadoCrear CrearCargaFin(BalanzadaRecibidaDTO balanzada);
        ResultadoCrear CrearRegistroBalanzaPuerto(BalanzadaRecibidaDTO balanzada);
        void ActualizarUltimaValidacion(BalanzadaRecibidaDTO balanzada);
        void RestaurarBalanzadasPerdidas(string numeroBalanza, int desde, int hasta);

    }
}