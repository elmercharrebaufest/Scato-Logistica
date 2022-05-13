using System;
using System.Collections.ObjectModel;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarCargaDeCupo : ProcesadorComando<ActualizarCargaDeCupo>
    {
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorActualizarCargaDeCupo(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(ActualizarCargaDeCupo comando)
        {
            var resultado = new Resultado();
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
            Log.Debug($"Recorrido a actualizar Carga de cupo {recorrido.Id}");
            if (recorrido.Centro.RequiereCupo && recorrido.Vehiculo != null)
            {
                Log.Debug($"Centro Requiere Cupo y existe vehiculo");

                var cargaDecupo = Repositorio.Listar<CargaDeCupo>(x => x.Numero == comando.Numero && comando.Numero != "" && comando.Numero != null && x.Recorrido == null && x.Centro.Id == recorrido.Centro.Id).LastOrDefault();
                if (cargaDecupo != null && !string.IsNullOrEmpty(cargaDecupo.Cupo))
                {
                    Log.Debug($"cargaDecupo Id {cargaDecupo.Id}");
                    cargaDecupo.Recorrido = recorrido;
                    if (cargaDecupo.Material == null)
                    {
                        cargaDecupo.Material = recorrido.Material;
                    }

                    recorrido.SacoTurnoConCircular = cargaDecupo.SacoTurnoConCircular;
                    recorrido.LlegoEnHorario = cargaDecupo.LlegoEnHorario;

                    if (!string.IsNullOrEmpty(recorrido.Vehiculo.CartaPorte.Cupo))
                    {
                        Log.Debug($"Vehiculo con Cupo {recorrido.Vehiculo.CartaPorte.Cupo}");
                        var cargaDecupoPorCartaDePorte = Repositorio.Listar<CargaDeCupo>(x => x.Cupo == recorrido.Vehiculo.CartaPorte.Cupo && x.Recorrido == null/* && x.Id != cargaDecupo.Id && x.Material != null && x.Material.EsGrano*/ && x.SinCupo == false && x.Centro.Id == recorrido.Centro.Id).LastOrDefault();
                        if (cargaDecupoPorCartaDePorte != null)
                        {
                            Log.Debug($"Eliminando cargaDecupoPorCartaDePorte patente {cargaDecupoPorCartaDePorte.Patente} cupo {cargaDecupoPorCartaDePorte.Cupo} ");
                            if (cargaDecupo.SinCupo)
                            {
                                cargaDecupo.Cupo = cargaDecupoPorCartaDePorte.Cupo;
                                cargaDecupo.RespuestaSap = cargaDecupoPorCartaDePorte.RespuestaSap;
                                cargaDecupo.FechaSap = cargaDecupoPorCartaDePorte.FechaSap;
                                cargaDecupo.SinCupo = false;
                                cargaDecupo.Especial = cargaDecupoPorCartaDePorte.Especial;
                                cargaDecupo.Material = cargaDecupoPorCartaDePorte.Material;
                            }

                            Repositorio.Remover(cargaDecupoPorCartaDePorte);
                        }
                        else if (cargaDecupo.SinCupo)
                        {
                            Log.Debug($"Sin cargaDecupoPorCartaDePorte");
                            cargaDecupo.Especial = recorrido.Establecimiento != null || (recorrido.Vehiculo.CartaPorte.TrigoEspecial.HasValue && recorrido.Vehiculo.CartaPorte.TrigoEspecial.Value);
                            cargaDecupo.Material = recorrido.Material;
                        }
                    }

                }
                else if (!string.IsNullOrEmpty(recorrido.Vehiculo.CartaPorte.Cupo) && recorrido.Vehiculo.CartaPorte.Cupo != configuracion.AppSettings["CupoDefault"])
                {
                    Log.Debug($"recorrido sin carga de Cupo y vehiculo con cupo no generico {recorrido.Vehiculo.CartaPorte.Cupo}");

                    var cargaDecupoPorCartaDePorte = Repositorio.Listar<CargaDeCupo>(x => x.Cupo == recorrido.Vehiculo.CartaPorte.Cupo && x.Recorrido == null && x.Centro.Id == recorrido.Centro.Id).LastOrDefault();
                    if (cargaDecupoPorCartaDePorte != null)
                    {
                        cargaDecupoPorCartaDePorte.Recorrido = recorrido;
                        cargaDecupoPorCartaDePorte.Numero = comando.Numero;
                        cargaDecupoPorCartaDePorte.Material = recorrido.Material;
                    }
                    if (cargaDecupo != null)
                    {
                        //Si existe una carga de cupo pendiente y el camión ya fue asignado, la quitamos
                        cargaDecupoPorCartaDePorte.EstuvoPendiente = true;
                        Repositorio.Remover(cargaDecupo);
                    }
                }
                else if (!string.IsNullOrEmpty(recorrido.Vehiculo.CartaPorte.Cupo) && recorrido.Vehiculo.CartaPorte.Cupo == configuracion.AppSettings["CupoDefault"])
                {
                    Log.Debug($"recorrido sin carga de Cupo y vehiculo con cupo generico {recorrido.Vehiculo.CartaPorte.Cupo}");
                    //Si una carga de cupo genérica no existe por tarjeta, es porque hay que crearla
                    var cupoDefault = new CargaDeCupo
                    {
                        Numero = comando.Numero,
                        Cupo = configuracion.AppSettings["CupoDefault"],
                        SinCupo = true,
                        Recorrido = recorrido,
                        Material = recorrido.Material,
                        Centro = recorrido.Centro,
                        Fecha = DateTime.Now,
                        FechaSap = DateTime.Now,
                        Especial = recorrido.Establecimiento != null || (recorrido.Vehiculo.CartaPorte.TrigoEspecial.HasValue && recorrido.Vehiculo.CartaPorte.TrigoEspecial.Value),
                        CPE = recorrido.Vehiculo.CartaPorte.Cpe.HasValue && recorrido.Vehiculo.CartaPorte.Cpe.Value,
                        Patente = recorrido.Patente
                    };

                    if (cargaDecupo != null)
                    {
                        Log.Debug($"Se elimina la carga de cupo: {cargaDecupo.Id}");
                        //Si existe una carga de cupo pendiente y el camión ya fue asignado, la quitamos
                        cupoDefault.EstuvoPendiente = true;
                        Repositorio.Remover(cargaDecupo);
                    }
                    Repositorio.Agregar(cupoDefault);
                }
                Repositorio.GuardarCambios();
            }
            else if (recorrido.Centro.RequiereCupo && !recorrido.Material.EsGrano)
            {
                Log.Debug($"Centro rquiere Cupo y Corresponde a no grano");
                var cargaDecupo = Repositorio.Listar<CargaDeCupo>(x => x.Numero == comando.Numero && comando.Numero != "" && comando.Numero != null && x.Recorrido == null && x.Centro.Id == recorrido.Centro.Id).LastOrDefault();
                if (cargaDecupo != null)
                {
                    cargaDecupo.Recorrido = recorrido;
                    if (cargaDecupo.Material == null)
                    {
                        cargaDecupo.Material = recorrido.Material;
                    }
                    Repositorio.GuardarCambios();
                }
            }

            return resultado;
        }
    }


}