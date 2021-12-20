using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMaterial : ProcesadorComando<CrearMaterial>
    {
        public ProcesadorCrearMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearMaterial comando)
        {
            var resultado = new ResultadoCrear();
            Material material = null;
            try
            {
                if (comando.Dto.CodigoSAP != null)
                {
                    comando.Dto.CodigoSAP = comando.Dto.CodigoSAP.TrimStart(new[] {'0'});
                }
                material = CrearEntidadMaterial(comando);
                ValidarMaterial(comando, resultado); 
                if (!resultado.HayErrores)
                {
                    Repositorio.Agregar(material);

                    var materialPorCentro = CrearEntidadMaterialPorCentro(comando.MaterialPorCentroDto, material);
                    ValidarMaterialPorCentro(comando.MaterialPorCentroDto, resultado);
                    if (!resultado.HayErrores)
                    {
                        Repositorio.Agregar(materialPorCentro);
                    }
                    var logueaEntidad = comando.GetType().GetCustomAttributes(true).Any(s => s.GetType() == typeof(LoguearEntidad));
                    if (logueaEntidad)
                    {
                        try
                        {
                            var logAbm = new LogABM
                            {
                                Pantalla = comando.GetType().Name,
                                Usuario = comando.Usuario,
                                Fecha = DateTime.Now,
                                Evento = EventoABM.Alta,
                                Entidad = comando.ToXml()
                            };
                            Repositorio.Agregar(logAbm);
                            Repositorio.GuardarCambios();
                        }
                        catch (Exception e)
                        {
                            Log.Warn(e, "Ocurrio un error al crear el log AMB Crear");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al crear el material {0}", comando.Dto.Descripcion);
                resultado.Error("", Textos.Observacion_ErrorEnLaCarga);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
                resultado.Id = material.Id;
            }
            return resultado;
        }

        protected Material CrearEntidadMaterial(CrearMaterial comando)
        {
            var materialEditado = Conversor.Convertir<MaterialDto, Material>(comando.Dto);
            if (comando.Dto.VariedadId.HasValue)
            {
                materialEditado.Variedad = Repositorio.Obtener<Variedad>(comando.Dto.VariedadId.Value);
            }
            materialEditado.AlmacenOrigen = Repositorio.Obtener<Almacen>(comando.Dto.AlmacenOrigenId);
            materialEditado.Activo = true;

            return materialEditado;
        }

        protected void ValidarMaterial(CrearMaterial comando, Resultado resultado)
        {
            if (Repositorio.Existe<Material>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.Material_DescripcionExistente);
            }
            if (comando.Dto.CodigoSAP != null && Repositorio.Existe<Material>(e => e.CodigoSAP == comando.Dto.CodigoSAP && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoSAP", Textos.Material_CodigoSAPExistente);
            }
            if (comando.Dto.EsUva && !comando.Dto.VariedadId.HasValue)
            {
                resultado.Error("VariedadId", Textos.Error_Requerido);
            }
        }

        protected MaterialPorCentro CrearEntidadMaterialPorCentro(MaterialPorCentroDto dto, Material material)
        {
            var materialPorCentroEditado = Conversor.Convertir<MaterialPorCentroDto, MaterialPorCentro>(dto);
            materialPorCentroEditado.Material = material;
            materialPorCentroEditado.Centro = Repositorio.Obtener<Centro>(dto.CentroId);
            materialPorCentroEditado.AlmacenPredeterminado = Repositorio.Obtener<Almacen>(dto.AlmacenPredId);
            materialPorCentroEditado.AnalisisInterno = dto.AnalisisInterno;
            materialPorCentroEditado.Camara = Repositorio.Obtener<Camara>(dto.CamaraId);
            materialPorCentroEditado.PorcentajeMuestraAuditoria = dto.PorcentajeMuestraAuditoria;
            materialPorCentroEditado.NoValidaCG = dto.NoValidaCG;
            materialPorCentroEditado.IgnoraContingencia = dto.IgnoraContingencia;
            return materialPorCentroEditado;
        }

        protected void ValidarMaterialPorCentro(MaterialPorCentroDto dto, Resultado resultado)
        {
            if (Repositorio.Existe<MaterialPorCentro>(e => e.Material.Id == dto.MaterialId
                && e.Centro.Id == dto.CentroId && (e.Id != dto.Id)))
            {
                resultado.Error("Material", Textos.MaterialPorCentro_Existente);
            }

            if (!Repositorio.Existe<Camara>(e => e.Id == dto.CamaraId))
            {
                resultado.Error("CamaraId", String.Format(Textos.Error_Requerido, Textos.Camara));
            }
        }
    }
}
