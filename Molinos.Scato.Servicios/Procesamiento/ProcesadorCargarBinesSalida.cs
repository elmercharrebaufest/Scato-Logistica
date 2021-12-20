using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCargarBinesSalida : ProcesadorComando<CargarBinesSalida>
    {
        public ProcesadorCargarBinesSalida(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CargarBinesSalida comando)
        {
            try
            {
                var remito = Repositorio.Obtener<RemitoBodegaUva>(r => r.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflow);
                ActualizarBinesSalida(comando.Bines, remito);
                remito.Observacion = comando.Observacion;
                var recorrido = remito.Recorrido;

                var pesoBinesEntrada = remito.DescargasDeBines.Sum(x => x.CantidadBines * (x.Tipo.Peso ?? 0M));
                var pesoBinesSalida = remito.CargasDeBines.Sum(x => x.CantidadBines * (x.Tipo.Peso ?? 0M));
                var pesoTaraBodega = (recorrido.PesoTara ?? 0) + Convert.ToInt32(pesoBinesEntrada - pesoBinesSalida);
                recorrido.PesoTaraBodega = pesoTaraBodega;
                Repositorio.GuardarCambios();

               return new ResultadoCargarBinesSalida {PesoTaraBodega = pesoTaraBodega};
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudieron actualizar los bines de salida");
                throw;
            }
        }

        private void ActualizarBinesSalida(IEnumerable<CargaDeBinesDto> binesSalida, RemitoBodegaUva remito)
        {
            if (remito.CargasDeBines != null)
            {
                remito.CargasDeBines.Clear();
            }
            else
            {
                remito.CargasDeBines = new List<CargaDeBines>();
            }
            foreach (var binSalida in binesSalida)
            {
                remito.CargasDeBines.Add(new CargaDeBines
                    {
                        CantidadBines = binSalida.CantidadBines,
                        Tipo = Repositorio.Obtener<Material>(binSalida.TipoId),
                        RemitoBodegaUva = remito
                    });
            }
        }
    }
}
