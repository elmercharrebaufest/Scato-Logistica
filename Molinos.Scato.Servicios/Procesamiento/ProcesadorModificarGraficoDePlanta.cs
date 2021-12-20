using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarGraficoDePlanta : ProcesadorComando<ModificarGraficoDePlanta>
    {
        public ProcesadorModificarGraficoDePlanta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarGraficoDePlanta comando)
        {
            var resultado = new ResultadoCrear();
            //Esta transacción es necesaria para que la actualización se ejecute incluso cuando falla la transacción padre.
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Log.Info("Iniciando ModificarGraficoDePlanta");
                var grafico = Repositorio.Obtener<GraficoDePlanta>(x => x.NombreActividad == comando.Dto.NombreActividad && x.Centro.Id == comando.Dto.CentroId);

                if (grafico == null)
                {
                    Log.Info("Creando GraficoDePlanta");
                    grafico = Conversor.Convertir<GraficoDePlantaDto, GraficoDePlanta>(comando.Dto);
                    grafico.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
                    Repositorio.Agregar(grafico);
                }
                else
                {
                    Log.Info("Modificando GraficoDePlanta");
                    grafico.Color = comando.Dto.Color;
                    grafico.Rango = comando.Dto.Rango;
                }

                Log.Info("Se van a guardar los cambios en GraficoDePlanta");
                Repositorio.GuardarCambios();

                resultado.Id = grafico.Id;
                Log.Info("Cambios guardados");
                transaction.Complete();
            }
            return resultado;
        }
    }
}
