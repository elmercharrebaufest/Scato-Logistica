using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;



namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCargadoPlanoDeCarga : ProcesadorModificar<ModificarCargadoPlanoDeCarga>
    {
        public ProcesadorModificarCargadoPlanoDeCarga(IRepositorio repositorio, IConversor conversor, ILogger log)
        : base(repositorio, conversor, log)
        {
        }



        protected override void ModificarEntidad(ModificarCargadoPlanoDeCarga comando)
        {
            var planoDeCarga = Repositorio.Obtener<PlanoDeCarga>(comando.Id);



            planoDeCarga.Cargado = true;
        }



        protected override void Validar(ModificarCargadoPlanoDeCarga comando, Resultado resultado)
        {



        }
    }
}