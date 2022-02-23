using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarPDFCpe : ProcesadorComando<ConsultarPDFCpe>
    {
        public ProcesadorConsultarPDFCpe(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ConsultarPDFCpe comando)
        {
            var resultado = new ResultadoConsultarPDFCpe();

            try
            {
                var cartaPorteElectronica = Repositorio.Listar<CartaPorteElectronica>(x => x.NroCTG == comando.NroCtg).FirstOrDefault();
                if (cartaPorteElectronica != null)
                {
                    if (cartaPorteElectronica.Pdf != null)
                    {
                        resultado.Pdf = cartaPorteElectronica.Pdf;
                    }
                    else
                    {
                        resultado.Errores.Add("3", "Error no se pudo obtener la imagen de la CP desde SCATO");
                    }
                } 
                else
                {
                    resultado.Errores.Add("4", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("5", Textos.Error_Generico);
                Log.Error(e, $"Error al obtener pdf de ctg {comando.NroCtg} en ProcesadorConsultarPDFCpe");
            }

            return resultado;
        }
    }
}
