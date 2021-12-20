using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using NUnit.Framework;

namespace Molinos.Scato.Test.Helpers
{
    [TestFixture]
    public class ExtensionesReflectionTest
    {
        private IngresosPorCompraDeGranosTransmisionASap ingresoCompraGranos;
        private EgresosNoProductivosTransmisionASap egresosNoProd;
        private IngresosEgresosFazonesTransmisionASap ingEgrFazon;
        private SalidaDeOrigenEnRedespachosTransmisionASap salidaOrignRedes;
        private ICollection<RecepcionYRedespacho> coleccion;
        private RecepcionYRedespacho RyD;

        [SetUp]
        public void SetUp()
        {          
            RyD = new RecepcionYRedespacho
            {
                Id = 5,
                Caracteristica = "carac1",
                Desckilos = "13",
                Descporc = "10",
                EntradaOSalida = "Entrada",
                NumCarPor = "0005",
                Resultado = "OK",
                Secuencia = "001",
                TipoMuest = "Normal"
            };
            coleccion = new Collection<RecepcionYRedespacho>{RyD};
            ingresoCompraGranos = new IngresosPorCompraDeGranosTransmisionASap
            {
                Almacen = "ALM1",
                Camara = "Camara1",
                Patente = "AAA111",
                RecepcionesYDespachosII = coleccion
            };

            egresosNoProd = new EgresosNoProductivosTransmisionASap
                {
                    Almacen = "almacn1",
                    Cantidad = (decimal) 23.11,
                    Cliente = "client1",
                    Peso = 100,
                    PesoTotal = 120
                };
            ingEgrFazon = new IngresosEgresosFazonesTransmisionASap
                {
                    Almacen = "almacn2",
                    Km = null,
                    Patente = "ABC333",
                    Provincia = "Bs As",
                    Transportista = "Jose"
                };
            salidaOrignRedes = new SalidaDeOrigenEnRedespachosTransmisionASap
                {
                    AlmEmisor = "almacn3",
                    CUITTransp = "30-11452777-6",
                    Kilometros = (decimal) 99.64,
                    CentroEmisor = "Molinos",
                    Patente1 = "ABR111"
                };
        }

        [Test]
        public void ObtenerPropiedadesIngresoCompraGranos()
        {
            
            var resultado = new Dictionary<string, string>
                {
                    {"Almacen","ALM1"},
                    {"Camara","Camara1"},
                    {"Patente","AAA111"},
                    {"RecepcionesYDespachosII-5-Desckilos","13"},
                    {"RecepcionesYDespachosII-5-Descporc","10"},
                    {"RecepcionesYDespachosII-5-Caracteristica","carac1"},
                    {"RecepcionesYDespachosII-5-EntradaOSalida","Entrada"},
                    {"RecepcionesYDespachosII-5-Resultado","OK"},
                };
            var target = ingresoCompraGranos.ObtenerPropiedades();

            foreach (var llave in resultado.Keys)
            {
                Assert.True(target.ContainsKey(llave));
                Assert.That(target[llave], Is.EqualTo(resultado[llave]));
            }
            Assert.That(target, Is.Not.Null);
            Assert.That(target, Is.Not.Empty);
        }

        [Test]
        public void ObtenerPropiedadesEgresosNoProd()
        {

            var resultado = new Dictionary<string, string>
                {
                    { "Almacen", "almacn1" },
                    { "Cantidad", "23,11" },
                    { "Cliente", "client1" },
                    { "Peso", "100" },
                    { "PesoTotal", "120" },
                };
            var target = egresosNoProd.ObtenerPropiedades();

            foreach (var llave in resultado.Keys)
            {
                Assert.True(target.ContainsKey(llave));
                Assert.That(target[llave], Is.EqualTo(resultado[llave]));
            }
            Assert.That(target, Is.Not.Null);
            Assert.That(target, Is.Not.Empty);
        }
        [Test]
        public void ObtenerPropiedadesIngEgrFazon()
        {

            var resultado = new Dictionary<string, string>
                {
                    { "Almacen", "almacn2" },
                    { "Km", "" },
                    { "Patente", "ABC333" },
                    { "Provincia", "Bs As" },
                    { "Transportista", "Jose" },
                };
            var target = ingEgrFazon.ObtenerPropiedades();

            foreach (var llave in resultado.Keys)
            {
                Assert.True(target.ContainsKey(llave));
                Assert.That(target[llave], Is.EqualTo(resultado[llave]));
            }
            Assert.That(target, Is.Not.Null);
            Assert.That(target, Is.Not.Empty);
        }

        [Test]
        public void ObtenerPropiedadessalidaOrignRedes()
        {

            var resultado = new Dictionary<string, string>
                { 
                    { "AlmEmisor", "almacn3" },
                    { "CUITTransp", "30-11452777-6" },
                    { "Kilometros", "99,64" },
                    { "CentroEmisor", "Molinos" },
                    { "Patente1", "ABR111" },
                };
            var target = salidaOrignRedes.ObtenerPropiedades();

            foreach (var llave in resultado.Keys)
            {
                Assert.True(target.ContainsKey(llave));
                Assert.That(target[llave], Is.EqualTo(resultado[llave]));
            }
            Assert.That(target, Is.Not.Null);
            Assert.That(target, Is.Not.Empty);
        }



        [Test]
        public void SetearPropiedadesingresoCompraGranos()
        {
            var resultado = new Dictionary<string, string>
                {
                    {"Almacen","ALM22"},
                    {"Camara","Camara10"},
                    {"Patente","XXX111"},
                    {"RecepcionesYDespachosII-5-Desckilos","20"},
                    {"RecepcionesYDespachosII-5-Descporc","5"},
                    {"RecepcionesYDespachosII-5-Caracteristica","carac5"},
                    {"RecepcionesYDespachosII-5-EntradaOSalida","Salida"},
                    {"RecepcionesYDespachosII-5-Resultado","Error"},
                };
            var target = ingresoCompraGranos.SetearPropiedades(resultado);
            Assert.True(ingresoCompraGranos.Almacen.Equals("ALM22"));
            Assert.True(ingresoCompraGranos.Camara.Equals("Camara10"));
            Assert.True(ingresoCompraGranos.Patente.Equals("XXX111"));
            Assert.True(ingresoCompraGranos.RecepcionesYDespachosII.First().Desckilos.Equals("20"));
            Assert.True(ingresoCompraGranos.RecepcionesYDespachosII.First().Descporc.Equals("5"));
            Assert.True(ingresoCompraGranos.RecepcionesYDespachosII.First().Caracteristica.Equals("carac5"));
            Assert.True(ingresoCompraGranos.RecepcionesYDespachosII.First().EntradaOSalida.Equals("Salida"));
            Assert.True(ingresoCompraGranos.RecepcionesYDespachosII.First().Resultado.Equals("Error"));
            Assert.That(target.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void SetearPropiedadesegresosNoProd()
        {
            var resultado = new Dictionary<string, string>
                { 
                    { "Almacen", "almacn22" },
                    { "Cantidad", "11,13" },
                    { "Cliente", "client5" },
                    { "Peso", "20" },
                    { "PesoTotal", "300,00" },
                };
            var target = egresosNoProd.SetearPropiedades(resultado);
            Assert.True(egresosNoProd.Almacen.Equals("almacn22"));
            Assert.True(egresosNoProd.Cantidad.Equals((decimal)11.13));
            Assert.True(egresosNoProd.Cliente.Equals("client5"));
            Assert.True(egresosNoProd.Peso.Equals(20));
            Assert.True(egresosNoProd.PesoTotal.Equals((decimal)300.0));
            Assert.That(target.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void SetearPropiedadesingEgrFazon()
        {
            var resultado = new Dictionary<string, string>
                { 
                    { "Almacen", "almacn55" },
                    { "Km", "11,13" },
                    { "Patente", "XXX787" },
                    { "Provincia", "Salta" },
                    { "Transportista", "Marcelo" },
                };
            var target = ingEgrFazon.SetearPropiedades(resultado);
            Assert.True(ingEgrFazon.Almacen.Equals("almacn55"));
            Assert.True(ingEgrFazon.Km.Equals(decimal.Parse("11,13")));
            Assert.True(ingEgrFazon.Patente.Equals("XXX787"));
            Assert.True(ingEgrFazon.Provincia.Equals("Salta"));
            Assert.True(ingEgrFazon.Transportista.Equals("Marcelo"));
            Assert.That(target.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void SetearPropiedadessalidaOrignRedes()
        {
            var resultado = new Dictionary<string, string>
                { 
                    { "AlmEmisor", "almacn55" },
                    { "CUITTransp", "30-22665774-6" },
                    { "Kilometros", "" },
                    { "CentroEmisor", "Arjona" },
                    { "Patente1", "MAY555" },
                };
            var target = salidaOrignRedes.SetearPropiedades(resultado);
            Assert.True(salidaOrignRedes.AlmEmisor.Equals("almacn55"));
            Assert.True(salidaOrignRedes.CUITTransp.Equals("30-22665774-6"));
            Assert.True(salidaOrignRedes.Kilometros.Equals(null));
            Assert.True(salidaOrignRedes.CentroEmisor.Equals("Arjona"));
            Assert.True(salidaOrignRedes.Patente1.Equals("MAY555"));
            Assert.That(target.HayErrores, Is.EqualTo(false));
        }

    }
}