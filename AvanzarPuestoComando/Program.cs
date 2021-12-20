using System;
using System.ServiceModel;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace AvanzarPuestoComando
{
    class Program
    {
        private static IServicioRepositorio srvRepositorio;
        static void Main(string[] args)
        {
            srvRepositorio = new ChannelFactory<IServicioRepositorio>("ServicioRepositorio").CreateChannel();
            Console.Write("Se inicia proceso\n\n");
            var workflows = srvRepositorio.ListarWorkFlowsEnPuestoComando();
            Console.Write("Faltan Avanzar {0} workflows\n\n", workflows.Count);
            var i = 0;
            foreach (var recorridoDto in workflows)
            {
                Console.Write("Iniciando {0} de {1}\n\n",i++,workflows.Count);
                Console.Write("{0}, Definicion {1}, Iniciando\n\n", recorridoDto.InstanciaWorkflow, recorridoDto.WorkflowDefinicionId);
                var servicioFactory = new ServicioActividadFactory<IPuestoComandoService>();
                var servicio = servicioFactory.CrearServicio("", recorridoDto.WorkflowDefinicionId);
                Console.Write("{0}, Definicion {1}, Servicios Creados\n\n", recorridoDto.InstanciaWorkflow, recorridoDto.WorkflowDefinicionId);
                var control = new ControlRecorridoDto
                {
                    Actividad = Textos.ActPuestoComando,
                    ActividadXaml = "PuestoComando",
                    Fecha = DateTime.Now,
                    NombreUsuario = "monteron",
                    PuestoDeTrabajoId = 0,
                    WorkflowInstanceId = recorridoDto.InstanciaWorkflow
                };
                Console.Write("{0}, Definicion {1}, Se va a ejecutar el puesto comando\n\n", recorridoDto.InstanciaWorkflow, recorridoDto.WorkflowDefinicionId);
                try
                {
                    var resultado = servicio.PuestoComando(recorridoDto.InstanciaWorkflow, control);
                    Console.Write("{0}, Definicion {1}, Puesto Comando Ejecutado Correcto\n\n", recorridoDto.InstanciaWorkflow, recorridoDto.WorkflowDefinicionId);
                }
                catch (Exception e)
                {
                    Console.Write("{0}, Definicion {1}, Puesto Comando ERROR!{2}\n\n", recorridoDto.InstanciaWorkflow, recorridoDto.WorkflowDefinicionId,e.Message);
                }
            }
            Console.Write("FIN\n\n");
            Console.Read();
        }

        private static void Main2()
        {
            var servicioFactory = new ServicioActividadFactory<IPuestoComandoService>();
            var servicio = servicioFactory.CrearServicio("", 475);
            var guids = new[]
                {
new Guid("3FCE2D72-05FC-45A0-BE88-E2048BCF978E"),
new Guid("1E07C371-BCFC-4BB4-B74F-F825138F0F5F"),
new Guid("DEA6DCD2-9574-4432-8382-B46D1E62718B"),
new Guid("C7190563-3797-4219-AD8A-FE3FB188F854"),
new Guid("F644C7FD-1CB1-4F45-BB3C-A9C8D3AC4B9B"),
new Guid("D2274282-74C2-45A9-B77A-99037DED2572"),
new Guid("F5596CDC-9A4F-4FEE-9660-9CF37EA85507"),
new Guid("0B76293B-511D-469F-B280-BE11F9E9BA8F"),
new Guid("1F7A3040-7261-4D53-804D-099E77C00D55"),
new Guid("5DD0A167-5D61-4E05-92FD-7C00291D878D"),
new Guid("0182572B-BE9F-4255-B253-3514A263EE68"),
new Guid("10D07828-306B-4901-90E4-9BCAAA64F433"),
new Guid("54C78918-EF9D-4BD4-8F53-C7F456754773"),
new Guid("017793B7-77CE-42AF-A3F4-E3C0E13D2391"),
new Guid("1761D1D3-67DD-485C-8C6D-FF3C76AF3E20"),
new Guid("BC223AB1-BFFA-4E1E-AC5E-50B6F9B3F979"),
new Guid("86BEC4C1-2280-4935-8B74-5DEBBD01A2B5"),
new Guid("5CEC0A20-B2FC-4483-B582-01738CB699EB"),
new Guid("BB5F8DD7-D5D7-492B-93CA-D3D5DFA51804"),
new Guid("0BFA3022-75BC-4215-AF7A-1CF0D533E6A4"),
new Guid("9A1B4575-0817-4947-9141-B05A0ECBD403"),
new Guid("A8357D07-F172-4361-A487-75CA8399461B"),
new Guid("9AA3D943-9CA9-4A95-BB32-1722FC4E894A"),
new Guid("D641E59A-C059-42FF-B8FE-9B98C2A198E9"),
new Guid("41CAEF4C-F51A-45E0-A5DF-295E6CC64EE5"),
new Guid("2895F2BC-2C36-41D7-816F-16B2449B9574"),
new Guid("1288ADA4-5E70-447B-BCE5-1041317F6F86")
                };

            foreach (var guid in guids)
            {
                // var guid = new Guid("10BBC025-2562-4646-9E77-0B18850E8D92");
                var control = new ControlRecorridoDto
                {
                    Actividad = Textos.ActPuestoComando,
                    ActividadXaml = "PuestoComando",
                    Fecha = DateTime.Now,
                    NombreUsuario = "monteron",
                    PuestoDeTrabajoId = 0,
                    WorkflowInstanceId = guid
                };
                var resultado = servicio.PuestoComando(guid, control);
            }



        }
    }
}
