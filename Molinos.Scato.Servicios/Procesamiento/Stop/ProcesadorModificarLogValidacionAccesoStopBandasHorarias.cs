using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.Procesamiento;
using Ninject.Extensions.Logging;

public class ProcesadorModificarLogValidacionAccesoStopBandasHorarias
    : ProcesadorModificar<ModificarLogValidacionAccesoStopBandasHorarias>
{
    public ProcesadorModificarLogValidacionAccesoStopBandasHorarias(
        IRepositorio repositorio,
        IConversor conversor,
        ILogger log)
        : base(repositorio, conversor, log)
    {
    }

    protected override void Validar(ModificarLogValidacionAccesoStopBandasHorarias comando, Resultado resultado) 
    {
        var existeLog = Repositorio.Existe<LogValidacionAccesoStopBandasHorarias>(x => x.CTG == comando.Dto.CTG);

        if (!existeLog)
        {            
            resultado.Error("Error", "No existe el registro para modificar");            
        }
    }

    protected override void ModificarEntidad(ModificarLogValidacionAccesoStopBandasHorarias comando)
    {

        Log.Info("ModificarEntidad, CTG del Camion: " + comando.Dto.CTG);
        var entidad = Repositorio.Obtener<LogValidacionAccesoStopBandasHorarias>(x => x.CTG == comando.Dto.CTG);               

        // cambios
        entidad.Permitido = comando.Dto.Permitido;
        entidad.Semaforo = comando.Dto.Semaforo;
        entidad.Estado = comando.Dto.Estado;
        entidad.Mensaje = comando.Dto.Mensaje;
        entidad.BandaHorariaFecha = comando.Dto.BandaHorariaFecha;
        entidad.BandaHorariaHoraDesde = comando.Dto.BandaHorariaHoraDesde;
        entidad.BandaHorariaHoraHasta = comando.Dto.BandaHorariaHoraHasta;
        entidad.Reintentos = comando.Dto.Reintentos;      
    }
}