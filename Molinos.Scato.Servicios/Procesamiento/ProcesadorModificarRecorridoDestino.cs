using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoDestino : ProcesadorComando<ModificarRecorridoDestino>
    {
        public ProcesadorModificarRecorridoDestino(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoDestino comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                if (recorridoAEditar.Almacen == null || recorridoAEditar.Centro.ModificaAlmacenEnPesada)
                {
                    recorridoAEditar.Almacen = Repositorio.Obtener<Almacen>(comando.AlmacenId);
                }

                if (recorridoAEditar.Calle == null && comando.CalleId.HasValue)
                {
                    recorridoAEditar.Calle = Repositorio.Obtener<Calle>(comando.CalleId);
                }

                if (recorridoAEditar.PuestosDeCargaDescargas.Count == 0 && comando.HidraulicaId.HasValue)
                {
                    recorridoAEditar.PuestosDeCargaDescargas.Add(Repositorio.Obtener<PuestosDeCargaDescarga>(comando.HidraulicaId));
                }

                if (recorridoAEditar.BalanzaBruto == null && comando.TipoPesada == Dominio.Enums.TipoPesada.Tara &&
                    comando.ProximaBalanzaId.HasValue)
                {
                    recorridoAEditar.BalanzaBruto = Repositorio.Obtener<Balanza>(comando.ProximaBalanzaId.Value);
                }

                if (recorridoAEditar.BalanzaTara == null && comando.TipoPesada == Dominio.Enums.TipoPesada.Bruto &&
                    comando.ProximaBalanzaId.HasValue)
                {
                    recorridoAEditar.BalanzaTara = Repositorio.Obtener<Balanza>(comando.ProximaBalanzaId.Value);
                }

                if (String.IsNullOrEmpty(recorridoAEditar.PesoBrutoUsuario))
                {
                    recorridoAEditar.PesoBrutoUsuario = comando.UsuarioBruto;
                }

                if (String.IsNullOrEmpty(recorridoAEditar.PesoTaraUsuario))
                {
                    recorridoAEditar.PesoTaraUsuario = comando.UsuarioTara;
                }

                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al modificar el destino en el recorrido {0}", comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
