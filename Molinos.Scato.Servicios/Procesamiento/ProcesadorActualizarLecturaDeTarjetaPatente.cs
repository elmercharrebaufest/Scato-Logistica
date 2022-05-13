using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarLecturaDeTarjetaPatente : ProcesadorComando<ActualizarLecturaDeTarjetaPatente>
    {
        public ProcesadorActualizarLecturaDeTarjetaPatente(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarLecturaDeTarjetaPatente comando)
        {
            var primeraLectura = Repositorio.ObtenerMayor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == comando.PuestoDeTrabajoId && x.Lectura == comando.Lectura, x => x.Id);
            var puestoDeTrabajo = Repositorio.ObtenerPrimero<PuestoDeTrabajo>(p => p.Id == comando.PuestoDeTrabajoId);
            var tolerancia = puestoDeTrabajo.Centro.ToleranciaPatenteLeida ?? 0;
            var diferencia = CalcularDiferencia(comando.Patente ?? string.Empty, comando.PatenteLeida ?? string.Empty);
            bool? existeOtroCamionEnPlanta = null;

            if (tolerancia > 0 && diferencia > 0 && diferencia <= tolerancia)
            {
                existeOtroCamionEnPlanta = Repositorio.Existe<Recorrido>(x => x.Patente == comando.PatenteLeida && !x.Terminado);
            }

            var reconocimientoExitoso = (diferencia == 0) || (tolerancia > 0 && diferencia <= tolerancia && !(existeOtroCamionEnPlanta != null && existeOtroCamionEnPlanta.Value));

            if (primeraLectura != null)
            {
                primeraLectura.PatenteLeida = comando.PatenteLeida;
                primeraLectura.OcrActivo = true;
                primeraLectura.ReconocimientoExitoso = reconocimientoExitoso;
            }
            
            LogLecturaDeTarjeta log = new LogLecturaDeTarjeta
            {
                Fecha = DateTime.Now,
                Patente = comando.Patente,
                PatenteLeida = comando.PatenteLeida,
                PuestoDeTrabajo = puestoDeTrabajo,
                Tolerancia = tolerancia,
                CantidadDeDiferencias = diferencia,
                ExisteOtroCamionEnPlanta = existeOtroCamionEnPlanta,
                ReconocimientoExitoso = reconocimientoExitoso,
                Tarjeta = comando.Lectura
            };
            Repositorio.Agregar(log);
            Repositorio.GuardarCambios();
            return new ResultadoValidarPatente { ReconocimientoExitoso  = reconocimientoExitoso };
        }

        public int CalcularDiferencia(string words, string words2)
        {
            int equals = 0;
            var masLarga = words.Length >= words2.Length ? words : words2;
            var masCorta = words.Length >= words2.Length ? words2 : words;

            //Recorro las palabras de la primera frase
            for (int i = 0; i < masLarga.Length; i++)
            {
                //Si estoy entre los indices de las palabras de la segunda frase (Para evitar buscar en indices que no existan)
                if (i < masCorta.Length)
                {
                    //Si en la posición actual las palabras de las dos frases son diferentes...
                    if (masCorta[i] != masLarga[i])
                    {
                        equals++;
                    }
                }
                else
                {
                    equals++;
                }
            }
            return equals;
        }
    }
}