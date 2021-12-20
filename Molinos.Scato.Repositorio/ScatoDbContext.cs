using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio
{ 
    public class ScatoDbContext : DbContext
    {
        public ScatoDbContext():base("ScatoDb")
        {
            Database.SetInitializer<ScatoDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // No usamos nombres de tablas en plural. Igualmente la pluralización fucniona solo con nombres en ingles.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //Se mapean todas las entidades bajo el namespace Molinos.Scato.Dominio.Entidades      
            MapearAssemblyDe<TipoDocumentoIdentidad>(modelBuilder, x => x.Namespace == typeof(TipoDocumentoIdentidad).Namespace, 
                excluir: null);

            // mapeo many-to-many unidireccional con pustos de carga descarga
            modelBuilder.Entity<Recorrido>()
                .HasMany(r => r.PuestosDeCargaDescargas)
                .WithMany(); // sin propiedad d enavecacion para el otro lado

            modelBuilder.Entity<PlanoDeCarga>()
               .HasMany(r => r.AgentesControlPrivado)
               .WithMany();

            modelBuilder.Entity<PlanoDeCargaHistorico>()
               .HasMany(r => r.AgentesControlPrivado)
               .WithMany();

            modelBuilder.Entity<MuestraEnvioACamara>()
                .HasMany(r => r.CaracteristicasDeCalidad)
                .WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("MuestraEnvioACamara_Id");
                    m.MapRightKey("CaracteristicaDeCalidad_Id");
                    m.ToTable("CaracteristicaDeCalidadMuestraEnvioACamara");
                });
            modelBuilder.Entity<CaracteristicasAnalizadas>()
                .HasRequired(l => l.Recorrido) //  Lot will always have 1 Vehicle
                .WithOptional(v => v.CaracteristicasAnalizadas);

            modelBuilder.Entity<LineUp>()
                .Property(p => p.Orden)
                .HasPrecision(18, 4);
        }

        private void MapearAssemblyDe<TEntidad>(DbModelBuilder modelBuilder, Predicate<Type> incluir, Predicate<Type> excluir)
        {
            var tiposEntidades  = typeof (TEntidad).Assembly.GetTypes()
                .Where(x => incluir(x));
            if (excluir != null)
            {
                tiposEntidades = tiposEntidades.Where(x => !excluir(x));
            }

            var metodo = modelBuilder.GetType().GetMethod("Entity");
            foreach (var tipoEntidad in tiposEntidades)
            {
                metodo.MakeGenericMethod(tipoEntidad).Invoke(modelBuilder, null);
            }
        }
    }
}