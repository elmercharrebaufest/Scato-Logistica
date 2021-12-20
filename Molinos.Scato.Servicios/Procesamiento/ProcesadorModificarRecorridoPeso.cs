using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoPeso : ProcesadorComando<ModificarRecorridoPeso>
    {
        public ProcesadorModificarRecorridoPeso(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoPeso comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorModificarRecorridoPeso con RecorridoId = {0}, Peso = {1} y TipoPesada = {2}", comando.InstanceId, comando.Peso, comando.TipoPesada);
               var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                var balanza = comando.BalanzaId.HasValue ? Repositorio.Obtener<Balanza>(x => x.Id == comando.BalanzaId) : null;
                if (comando.TipoPesada == TipoPesada.Bruto)
                {
                    recorridoAEditar.PesoBruto = comando.Peso;
                    recorridoAEditar.PesoBrutoFecha = DateTime.Now;
                    recorridoAEditar.BalanzaBruto = balanza;
                    recorridoAEditar.PesoBrutoUsuario = comando.Usuario;
                    recorridoAEditar.PesoBrutoModalidad = balanza != null ? balanza.Modalidad : (Modalidad?)null;
                }
                else
                {
                    recorridoAEditar.PesoTara = comando.Peso;
                    recorridoAEditar.PesoTaraFecha = DateTime.Now;
                    recorridoAEditar.BalanzaTara = balanza;
                    recorridoAEditar.PesoTaraUsuario = comando.Usuario;
                    recorridoAEditar.PesoTaraModalidad = balanza != null ? balanza.Modalidad : (Modalidad?)null;

                }
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ha ocurrido un error en ProcesadorModificarRecorridoPeso con RecorridoId = {0}, Peso = {1} y TipoPesada = {2}", comando.InstanceId, comando.Peso, comando.TipoPesada);
                resultado.Error("", Textos.Error_Generico);
            }
            return resultado;
        }
    }
}
