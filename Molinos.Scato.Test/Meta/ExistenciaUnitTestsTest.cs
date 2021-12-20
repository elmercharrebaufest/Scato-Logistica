using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.WebMobile.Controllers;
using NUnit.Framework;

namespace Molinos.Scato.Test.Meta
{
    /// <summary>
    /// Checkea que existan tests de unidad para Controllers, Procesadores de Comandos, Servicios
    /// </summary>
    [TestFixture]
    public class ExistenciaUnitTestsTest
    {
        [Test]
        public void ExisteUnTestParaCadaControllerSoloWarn()
        {
            ExistenTestsPara(
                assemblyDe: typeof(ChoferController),
                condicion: t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract,
                nombreGrupo: "Controllers");
        }
        [Test]
        public void ExisteUnTestParaCadaControllerMobileSoloWarn()
        {
            ExistenTestsPara(
                assemblyDe: typeof(IndexController),
                condicion: t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract,
                nombreGrupo: "Controllers");
        }
        [Test]
        public void ExisteUnTestParaCadaControllerMobile()
        {
            ExistenTestsPara(
                assemblyDe: typeof(IndexController),
                condicion: t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract,
                nombreGrupo: "Controllers",
                fallarSifaltan: true,
                excluidos: new List<string>());
        }
        //[Test]
        //public void ExisteUnTestParaCadaController()
        //{
        //    ExistenTestsPara(
        //        assemblyDe: typeof (ChoferController),
        //        condicion: t =>  typeof (Controller).IsAssignableFrom(t) && !t.IsAbstract,
        //        nombreGrupo: "Controllers", 
        //        fallarSifaltan: true,
        //        excluidos: new List<string>());
        //}

        //[Test]
        //public void ExisteUnTestParaCadaProcesadorDeComandos()
        //{
        //    ExistenTestsPara(
        //        assemblyDe: typeof (IProcesadorComando<>),
        //        condicion: x => !x.IsAbstract 
        //            && x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IProcesadorComando<>)),
        //        nombreGrupo: "Procesadores de Comandos",
        //        fallarSifaltan: true);
        //}

        [Test]
        public void ExisteUnTestParaCadaCodeActivitySoloWarn()
        {
            ExistenTestsPara(
                assemblyDe: typeof(PersistirPropiedadesCustom),
                condicion: x => x.BaseType != null && (x.BaseType == typeof(CodeActivity) || (x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(CodeActivity<>))),
                nombreGrupo: "Actividades");
        }

        //[Test]
        //public void ExisteUnTestParaCadaCodeActivity()
        //{
        //    ExistenTestsPara(
        //        assemblyDe: typeof(PersistirPropiedadesCustom),
        //        condicion: x => x.BaseType != null && (x.BaseType == typeof(CodeActivity) || (x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(CodeActivity<>))),
        //        nombreGrupo: "Actividades", fallarSifaltan:true,
        //        excluidos: new List<string> {"ObtenerFecha"});
        //}
        
        private void ExistenTestsPara(Type assemblyDe, Func<Type, bool> condicion, string nombreGrupo, bool fallarSifaltan = false, IList<string> excluidos = null)
        {
            excluidos = excluidos ?? new List<string>();
            var nombresClasses = assemblyDe.Assembly.GetTypes().Where(condicion).Select(t => t.Name);
            
            var nombresTests = NombresTests();

            var sinTest = nombresClasses.Where(nombreClase => !nombresTests.Contains(nombreClase + "Test")).ToList();
            var sinTestFiltrado = sinTest.Where(nombresClase => !excluidos.Contains(nombresClase)).ToList();

            if (sinTestFiltrado.Count > 0)
            {
                var resultado = ToString(nombreGrupo, sinTestFiltrado);
                if (fallarSifaltan)
                {
                    Assert.Fail(resultado);
                }
                else
                {
                    Assert.Inconclusive(resultado);
                }
            }
            else
            {
                Assert.Pass("Todos los {0} tienen test de unidad.", nombreGrupo);
            }
        }

        private static string ToString(string nombreGrupo, List<string> sinTest)
        {
            var builder = new StringBuilder();
            builder.Append("WARN - ")
                   .Append(nombreGrupo)
                   .AppendFormat(" sin tests ({0}):", sinTest.Count)
                   .AppendLine();
            foreach (var nombreClase in sinTest)
            {
                builder.AppendLine(nombreClase);
            }
            builder.AppendLine();
            return builder.ToString();
        }

        private IList<string> NombresTests()
        {
            return GetType().Assembly.GetTypes()
                            .Where(t => Attribute.GetCustomAttributes(t).Any(att => att.GetType() == typeof(TestFixtureAttribute)))
                            .Select(t => t.Name).ToList();
        }
    }
}
