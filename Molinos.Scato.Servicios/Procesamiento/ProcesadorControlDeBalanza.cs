using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorControlDeBalanza : ProcesadorComando<ControlDeBalanzaComando>
    {
        public ProcesadorControlDeBalanza(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(ControlDeBalanzaComando comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se está ejecutando ProcesadorControlDeBalanza con Numero de Patente: {0}", comando.Dto.Patente);
                var controlDeBalanza = Repositorio.Obtener<ControlDeBalanza>(comando.Dto.ControlDeBalanzaId);
                var recorrido = Repositorio.Obtener<Recorrido>(f => f.InstanciaWorkflow == comando.Dto.InstanciaWorkflow);
                if (controlDeBalanza == null)   //Control de Balanza Nuevo
                {
                    controlDeBalanza = new ControlDeBalanza
                        {
                            Observaciones = comando.Dto.Observaciones,
                            Recorrido = recorrido,
                            TipoPesada = comando.Dto.TipoPesada,
                            ControlesDeBalanzasPesadas =
                                comando.Dto.ControlesDeBalanzasPesadas.Select(s => new ControlDeBalanzaPesada
                                    {
                                        Balanza = Repositorio.Obtener<Balanza>(f => f.Id == s.BalanzaId),
                                        Fecha = s.Fecha,
                                        Peso = s.Peso,
                                    }).ToList()
                        };
                    Repositorio.Agregar(controlDeBalanza);
                }
                else //Control de Balanza Existente
                {
                    controlDeBalanza.Observaciones = comando.Dto.Observaciones;
                    foreach (var controlPesadaDto in comando.Dto.ControlesDeBalanzasPesadas)
                    {
                        var dto = controlPesadaDto;
                        var pesada = Repositorio.Obtener<ControlDeBalanzaPesada>(f => f.Balanza.Id == dto.BalanzaId && f.ControlDeBalanza.Id == controlDeBalanza.Id);
                        if (pesada != null) //Pesada Existente
                        {
                            pesada.Fecha = controlPesadaDto.Fecha;
                            pesada.Peso = controlPesadaDto.Peso;
                        }
                        else //Pesada Inexistente
                        {
                            controlDeBalanza.ControlesDeBalanzasPesadas.Add(new ControlDeBalanzaPesada
                                {
                                    Balanza = Repositorio.Obtener<Balanza>(f => f.Id == dto.BalanzaId),
                                    Fecha = controlPesadaDto.Fecha,
                                    Peso = controlPesadaDto.Peso,
                                    ControlDeBalanza = controlDeBalanza
                                });
                        }
                    }
                }
                if (comando.Finalizar) //Finalizar Control de Balanza
                {
                    recorrido.ControlBalanza = false;
                }
                Repositorio.GuardarCambios();
                resultado.Id = controlDeBalanza.Id;
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorControlDeBalanza con Numero de Patente: {0}", comando.Dto.Patente);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
