using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTransportistaOrdenCargaInternaFason : ProcesadorComando<CrearTransportistaOrdenCargaInternaFason>
    {
        public ProcesadorCrearTransportistaOrdenCargaInternaFason(IRepositorio repositorio, IConversor conversor,
                                                                  ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearTransportistaOrdenCargaInternaFason comando)
        {
            var resultado = new Resultado();
            var OrdenCargaInternaFason = Repositorio.Obtener<OrdenCargaInternaFason>(comando.OrdenCargaInternaFasonId);

            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                try
                {
                    var transportista = new Transportista
                        {
                            Cuit = comando.Dto.Cuit,
                            RazonSocial = comando.Dto.RazonSocial,
                            Domicilio = comando.Dto.Domicilio,
                            Localidad = Repositorio.Obtener<Localidad>(x => x.Id == comando.Dto.LocalidadId),
                            Provincia = Repositorio.Obtener<Provincia>(x => x.Id == comando.Dto.ProvinciaId),
                        };

                    if (Repositorio.Existe<Transportista>(x => x.Cuit == comando.Dto.Cuit))
                    {
                        transportista = Repositorio.Obtener<Transportista>(x => x.Cuit == comando.Dto.Cuit);
                    }
                    OrdenCargaInternaFason.Transportista = transportista;
                    Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId).Transportista =
                        transportista;


                    Repositorio.GuardarCambios();
                }
                catch (Exception e)
                {
                    
                    resultado.Errores.Add("", e.Message);
                }

            }
            return resultado;
        }

        private void Validar(CrearTransportistaOrdenCargaInternaFason comando, Resultado resultado)
        {
            if (!Repositorio.Existe<OrdenCargaInternaFason>(x => x.Id == comando.OrdenCargaInternaFasonId))
            {
                resultado.Error("", Textos.Error_Generico);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.LocalidadId))
            {
                resultado.Error("LocalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.LocalidadId && x.Provincia.Id == comando.Dto.ProvinciaId))
            {
                resultado.Error("LocalidadId", Textos.Error_LocalidadInvalida);
            }
        }
    }
}