using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarBalanzaEstaEnCero : ProcesadorComando<ModificarBalanzaEstaEnCero>
    {
        public ProcesadorModificarBalanzaEstaEnCero(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarBalanzaEstaEnCero comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorModificarBalanzaEstaEnCero con Balanza Id = {0} y EstaEnCero = {1}", comando.BalanzaId, comando.EstaEnCero);
                var balanzaAEditar = Repositorio.Obtener<Balanza>(comando.BalanzaId);
                balanzaAEditar.EstaEnCero = comando.EstaEnCero;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ha ocurrido un error en ProcesadorModificarBalanzaEstaEnCero con Balanza Id = {0} y EstaEnCero = {1}", comando.BalanzaId, comando.EstaEnCero);
                resultado.Error("", Textos.Pesada_Error);
            }
            return resultado;
        }
    }
}
