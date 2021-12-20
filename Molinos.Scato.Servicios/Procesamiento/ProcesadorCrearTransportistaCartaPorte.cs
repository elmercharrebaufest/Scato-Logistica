using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTransportistaCartaPorte : ProcesadorComando<CrearTransportistaCartaPorte>
    {
        public ProcesadorCrearTransportistaCartaPorte(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearTransportistaCartaPorte comando)
        {
            var resultado = new Resultado();
            var cartaPorte = Repositorio.Obtener<CartaPorte>(comando.CartaPorteId);

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
                    cartaPorte.Transportista = transportista;
                    Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId).Transportista =
                        transportista;


                    Repositorio.GuardarCambios();
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al crear transportistsa {0}", comando.Dto.Cuit);
                    resultado.Errores.Add("",e.Message);
                }
                
            }
            return resultado;
        }

        private void Validar(CrearTransportistaCartaPorte comando, Resultado resultado)
        {
            if (!Repositorio.Existe<CartaPorte>(x => x.Id == comando.CartaPorteId))
            {
                resultado.Error("", Textos.Error_Generico);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.LocalidadId))
            {
                resultado.Error("LocalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Provincia>(x => x.Id == comando.Dto.ProvinciaId))
            {
                resultado.Error("ProvinciaId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.LocalidadId && x.Provincia.Id == comando.Dto.ProvinciaId))
            {
                resultado.Error("LocalidadId", Textos.Error_LocalidadInvalida);
            }
        }
    }
}
