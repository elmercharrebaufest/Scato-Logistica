using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarFormatoDeImpresion : ProcesadorModificar<ModificarFormatoDeImpresion>
    {
        public ProcesadorModificarFormatoDeImpresion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarFormatoDeImpresion comando)
        {
            var formatoDeImpresion = Repositorio.Obtener<FormatoDeImpresion>(comando.Dto.Id);

            formatoDeImpresion.Columnas = comando.Dto.Columnas;
            formatoDeImpresion.Descripcion = comando.Dto.Descripcion;
            formatoDeImpresion.Filas = comando.Dto.Filas;
            formatoDeImpresion.MargenIzquierdo = comando.Dto.MargenIzquierdo;
            formatoDeImpresion.MargenSuperior = comando.Dto.MargenSuperior;
            formatoDeImpresion.Posicion = comando.Dto.Posicion;

           if (formatoDeImpresion.FormatoDePapel.Id != comando.Dto.FormatoDePapelId)
            {
                formatoDeImpresion.FormatoDePapel = Repositorio.Obtener<FormatoDePapel>(comando.Dto.FormatoDePapelId);
            }
            foreach (var formato in comando.Dto.FormatosDeCampo)
            {
                if (!formato._destroy && !formato.EsNuevo)
                {
                    var campoAModificar = formatoDeImpresion.FormatosDeCampo.FirstOrDefault(x => x.Id == formato.Id);
                    if (campoAModificar != null)
                    {
                        campoAModificar.Alineacion = formato.Alineacion;
                        campoAModificar.Campo = Repositorio.Obtener<Campo>(x => x.Id == formato.CampoId);
                        campoAModificar.Columna = formato.Columna;
                        campoAModificar.Cursiva = formato.Cursiva;
                        campoAModificar.Fila = formato.Fila;
                        campoAModificar.Negrita = formato.Negrita;
                        campoAModificar.Subrayado = formato.Subrayado;
                        campoAModificar.Tamaño = formato.Tamaño;
                        campoAModificar.EsColumna = formato.EsColumna;
                        campoAModificar.TipoDeCampo = formato.TipoDeCampo;
                        campoAModificar.Letra = Repositorio.Obtener<Letra>(x => x.Id == formato.LetraId);
                        campoAModificar.Texto = formato.Texto;
                    }
                }
                else if (!formato._destroy && formato.EsNuevo)
                {
                    var formatoCampo = Conversor.Convertir<FormatoDeCampoDto, FormatoDeCampo>(formato);
                    formatoCampo.Letra = Repositorio.Obtener<Letra>(x => x.Id == formato.LetraId);
                    formatoCampo.Campo = Repositorio.Obtener<Campo>(x => x.Id == formato.CampoId);
                    formatoCampo.FormatoDeImpresion = formatoDeImpresion;
                    formatoDeImpresion.FormatosDeCampo.Add(formatoCampo);
                }
            }
        }

        protected override void Validar(ModificarFormatoDeImpresion comando, Resultado resultado)
        {

        }
    }
}
