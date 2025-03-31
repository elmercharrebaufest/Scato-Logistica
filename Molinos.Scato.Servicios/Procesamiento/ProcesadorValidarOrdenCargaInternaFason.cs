using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Validations.Interfaces;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarOrdenCargaInternaFason : ProcesadorComando<ValidarOrdenCargaInternaFason>
    {
        protected readonly IServicioComandos servicioComandos;
        private readonly IValidatorEntity<OrdenCargaInternaFasonDto> validacionCrearOrdenInternaFason;
        protected readonly IServicioRepositorio servicio;
        private readonly IServicioOperaciones servicioOperaciones;
        private ResultadoOrdenFason resultado;

        public ProcesadorValidarOrdenCargaInternaFason(IServicioComandos servicioComandos, IValidatorEntity<OrdenCargaInternaFasonDto> validacionCrearOrdenInternaFason, IServicioRepositorio servicio, IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOperaciones servicioOperaciones)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.validacionCrearOrdenInternaFason = validacionCrearOrdenInternaFason;
            this.servicio = servicio;
            this.resultado = new ResultadoOrdenFason();
            this.servicioOperaciones = servicioOperaciones;
        }


        /// <summary>
        /// Ejecuta la validación de la orden de carga interna fason.
        /// </summary>
        /// <param name="comando">Comando que contiene los datos de la orden a validar.</param>
        /// <returns>Resultado de la validación de la orden.</returns>
        /// <remarks>
        /// Este método realiza las siguientes validaciones:
        /// 1. Obtiene las órdenes de carga asociadas a la patente del chasis.
        /// 2. Selecciona la orden que coincide con el ID proporcionado en el comando.
        /// 3. Convierte el MaterialId de la orden a un entero.
        /// 4. Valida la orden frente a las reglas de negocio definidas.
        /// 5. Si hay errores en la validación, retorna el resultado con los errores.
        /// 6. Si no hay errores, procede a validar la orden y retorna el resultado final.
        /// </remarks>
        public override Resultado Ejecutar(ValidarOrdenCargaInternaFason comando)
        {
            try
            {
                var materialId = int.Parse(comando.Orden.MaterialId);
                var orden = ObtenerOrdenOperaciones(comando.Orden.PatenteChasis, materialId);
                orden.MaterialId = materialId.ToString();
                // Ya en esta instancia el materialId es el Id del material seleccionado no el codigo Sap que llega de Operaciones
                resultado = ValidarOrdenFront(orden, materialId, comando.CentroId, comando.Usuario, comando.AplicaFastPass);

                if (resultado.HayErrores)
                {
                    return resultado;
                }

                return Validar(resultado.Dto, comando.CentroId);
            }
            catch (Exception ex)
            {
                resultado.Error("1", ex.Message);
            }
            return resultado;
        }

        /// <summary>
        /// Valida y procesa una orden de carga interna fason.
        /// </summary>
        /// <param name="ordenOperaciones">Datos de la orden de carga.</param>
        /// <param name="materialId">ID del material.</param>
        /// <param name="centroId">ID del centro.</param>
        /// <param name="usuario">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la validación de la orden de carga interna fason.</returns>
        /// <remarks>
        /// Este método realiza las siguientes validaciones:
        /// 1. Obtiene el workflow correspondiente según el tipo de flete fason.
        /// 2. Convierte los CUITs y CUILs de la orden a formato de scato con '-'.
        /// 3. Obtiene los datos de clientes, chofer y proveedor intermediario de flete.
        /// 4. Genera y ejecuta la consulta de escalables.
        /// 5. Lista los materiales por workflow y verifica si el material es derivado granario.
        /// 6. Llena los datos de la orden de carga interna fason.
        /// 7. Maneja posibles errores durante el proceso.
        /// </remarks>
        private ResultadoOrdenFason ValidarOrdenFront(OrdenDeCargaDto ordenOperaciones, int? materialId, int centroId, string usuario, bool aplicaFastPass)
        {
            try
            {

                var ordenCargaInterna = new OrdenCargaInternaFasonDto();
                string workflowDescripcion = ordenOperaciones.FleteMOA
                    ? Constantes.WorkFlow.workflowFason
                    : Constantes.WorkFlow.workflowFasonSinFlete;

                var workflow = servicio.ObtenerWorkflowPorCodigo(workflowDescripcion);
                var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
                tiposComerciales = workflowDescripcion.Contains("SinFlete")
                    ? tiposComerciales.Where(c => c.Descripcion.Contains("SALIDA")).ToList()
                    : tiposComerciales.Where(c => c.Descripcion.Contains("EGRESO")).ToList();

                var materialesPermitidos = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId).Select(m => m.MaterialId).ToList();

                if (materialId.HasValue && !materialesPermitidos.Contains(materialId.Value))
                {
                    resultado.Error("2", Textos.MaterialNoPermitido);
                    return resultado;
                }

                var cuils = new Dictionary<string, string>
                {
                    { "Destino", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones?.CUITDestino) },
                    { "Cliente", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones?.CUITCliente) },
                    { "Transporte", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones.CUITTransporte) },
                    { "Chofer", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones.CUILChofer) },
                    { "Remitente", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones?.RemitenteComercial) },
                    { "IntermediarioFlete", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones?.CUITIntermediarioFlete) },
                    { "Destinatario", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones?.CUITDestinatario) },
                    { "PagadorFlete", FormatterHelper.ConvertirCuilConGuionesSinException(ordenOperaciones?.PagadorFlete) }
                };

                var clientes = new Dictionary<string, ClienteDto>
                {
                    { "PagadorFlete", servicio.ObtenerClientePorCuit(cuils["PagadorFlete"]) },
                    { "Destino", servicio.ObtenerClientePorCuit(cuils["Destino"]) },
                    { "Cliente", servicio.ObtenerClientePorCuit(cuils["Cliente"]) },
                    { "Remitente", servicio.ObtenerClientePorCuit(cuils["Remitente"]) },
                    { "Destinatario", servicio.ObtenerClientePorCuit(cuils["Destinatario"]) }
                };

                var chofer = servicio.ObtenerChoferPorCuit(cuils["Chofer"]);
                var intermediarioFlete = servicio.ObtenerProveedorPorCuit(cuils["IntermediarioFlete"], new TiposProveedor { PR = true });
                var transportista = servicio.ObtenerProveedorPorCuit(cuils["Transporte"], new TiposProveedor { PR = true });

                ordenCargaInterna.TipoComercialId = tiposComerciales.FirstOrDefault()?.Id ?? 0;
                ordenCargaInterna.TipoComercialDesc = tiposComerciales.FirstOrDefault()?.Descripcion;

                var resultadoEscalables = servicioComandos.Ejecutar(GenerarConsultaEscalables(ordenOperaciones.PatenteChasis, ordenOperaciones.PatenteAcoplado, usuario)) as ResultadoEscalables;
                if (resultadoEscalables.HayErrores)
                {
                    resultado.Error("1", resultadoEscalables.Errores.FirstOrDefault().Value);
                    return resultado;
                }
                else if (!resultadoEscalables.Categoria.HasValue)
                {
                    resultado.Error("1", Textos.CategoriaEscalable_Nula);
                    return resultado;
                }

                var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);
                var esDerivado = materiales.Where(x => x.EsDerivadoGranario).Select(x => x.MaterialId).ToList();
                var material = materiales.FirstOrDefault(x => x.MaterialId == materialId);

                if (esDerivado.Contains(material?.MaterialId ?? 0))
                {
                    ordenCargaInterna.DerivadoGranarioHabilitado = true;
                    ordenCargaInterna.Cliente = clientes["Destino"].Descripcion;
                    ordenCargaInterna.ClienteId = clientes["Destino"].Id;
                    ordenCargaInterna.PlantaDGDestino = int.TryParse(ordenOperaciones.PlantaCodigo, out var plantId) ? plantId : (int?)null;
                    ordenCargaInterna.TipoYOrdenDestino = $"{ordenOperaciones.DomicilioTipo}-{ordenOperaciones.DomicilioOrden}";
                    ordenCargaInterna.TipoDomicilioDestino = int.TryParse(ordenOperaciones.DomicilioTipo, out var domicilioTipo) ? domicilioTipo : (int?)null;
                    ordenCargaInterna.OrdenDomicilioDestino = ordenOperaciones.DomicilioOrden;
                    ordenCargaInterna.PagadorFlete = clientes["PagadorFlete"].Descripcion;
                    ordenCargaInterna.PagadorFleteId = clientes["PagadorFlete"].Id;
                }
                else
                {
                    ordenCargaInterna.DerivadoGranarioHabilitado = false;
                    ordenCargaInterna.Cliente = clientes["Cliente"].Descripcion;
                    ordenCargaInterna.ClienteId = clientes["Cliente"].Id;
                    ordenCargaInterna.PlantaDGDestino = null;
                    ordenCargaInterna.TipoYOrdenDestino = null;
                    ordenCargaInterna.TipoDomicilioDestino = null;
                    ordenCargaInterna.OrdenDomicilioDestino = null;
                    ordenCargaInterna.PagadorFlete = null;
                    ordenCargaInterna.PagadorFleteId = null;
                }


                ordenCargaInterna.Id = 0;
                ordenCargaInterna.NumeroOrden = servicio.ObtenerNuevoNumeroDeOrdenFason();
                ordenCargaInterna.FechaEmision = DateTime.Now;
                ordenCargaInterna.PatenteCamion = ordenOperaciones.PatenteChasis;
                ordenCargaInterna.PatenteAcoplado = ordenOperaciones.PatenteAcoplado;
                ordenCargaInterna.Transportista = ordenOperaciones.RazonSocialTransporte;
                ordenCargaInterna.TransportistaId = transportista.Id;
                ordenCargaInterna.EsTransportista = false;
                ordenCargaInterna.EsExtranjero = false;
                ordenCargaInterna.MaterialId = material?.MaterialId ?? materialId ?? 0;
                ordenCargaInterna.MaterialDesc = material.MaterialDesc;
                ordenCargaInterna.ClienteCuit = clientes["Cliente"].Cuit ?? string.Empty;
                ordenCargaInterna.Chofer = chofer ?? null;
                ordenCargaInterna.KmARecorrer = ordenOperaciones.KmARecorrer;
                ordenCargaInterna.TipoVehiculo = resultadoEscalables.Categoria.Value;
                ordenCargaInterna.Demorado = false;
                ordenCargaInterna.Rechazado = false;
                if (clientes.TryGetValue("Remitente", out var remitente) && remitente != null)
                {
                    ordenCargaInterna.Remitente = remitente.Descripcion;
                    ordenCargaInterna.RemitenteId = remitente.Id;
                    ordenCargaInterna.RemitenteCodigoSap = remitente.CodigoSap;
                }
                else
                {
                    ordenCargaInterna.Remitente = string.Empty;
                    ordenCargaInterna.RemitenteId = 0;
                    ordenCargaInterna.RemitenteCodigoSap = string.Empty;
                }
                if (clientes.TryGetValue("Destinatario", out var destinatario) && destinatario != null)
                {
                    ordenCargaInterna.Destinatario = destinatario.Descripcion;
                    ordenCargaInterna.DestinatarioId = destinatario.Id;
                }
                else
                {
                    ordenCargaInterna.Destinatario = string.Empty;
                    ordenCargaInterna.DestinatarioId = 0;
                }               
                if (intermediarioFlete != null)
                {
                    ordenCargaInterna.IntermediarioFleteId = intermediarioFlete.Id;
                    ordenCargaInterna.IntermediarioFlete = intermediarioFlete.Descripcion;
                }
                ordenCargaInterna.NumeroOrdenExterno = ordenOperaciones.Id.ToString();
                ordenCargaInterna.DestinoMercaderia = ordenOperaciones.DestinoMercaderia;
                ordenCargaInterna.Observaciones = ordenOperaciones.Observacion;

                resultado.Dto = ordenCargaInterna;
                resultado.ControlRecorrido = GenerarControlRecorrido(0, usuario, aplicaFastPass);

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Error("1", $"{Textos.ErrorLlenarCaomposEntidad}  {ex.Message}");
            }

            return resultado;
        }

        private string ObtenerCuit(int clienteId, string clienteCuit)
        {
            if (!string.IsNullOrEmpty(clienteCuit))
            {
                return clienteCuit.Replace("-", string.Empty);
            }

            var cliente = servicio.ObtenerCliente(clienteId);
            if (cliente == null)
            {
                resultado.Errores.Add("2", Textos.ClienteNoExiste);
            }

            return cliente.Cuit.Replace("-", string.Empty);
        }

        private void ManejarErrores(IDictionary<string, string> errores)
        {
            foreach (var error in errores)
            {
                resultado.Error("1", error.Value);
            }
        }

        private Comando GenerarConsultaEscalables(string patente, string acoplado, string usuario)
        {
            if (ValidarDummyActivo())
            {
                return new ConsultarEscalablesDummy { Patente = patente, Acoplado = acoplado, Acoplado2 = string.Empty, Usuario = usuario };
            }
            else
            {
                return new ConsultarEscalables { Patente = patente, Acoplado = acoplado, Acoplado2 = string.Empty, Usuario = usuario };
            }

        }

        private bool ValidarDummyActivo()
        {
            var configuracionGeneral = servicio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason, Constantes.ConfiguracionGeneral.CNRT.CNRTDummy, null);
            return configuracionGeneral is null ? false : bool.Parse(configuracionGeneral.Valor);
        }

        /// <summary>
        /// Valida la orden de carga interna fason.
        /// Se realizan las siguientes validaciones:
        /// - Validación de datos de la orden.
        /// - Validación de transportista y chofer.
        /// - Validación de patentes y acoplados.
        /// - Validación de planta y domicilio.
        /// - Validación de CTG y CPE.
        /// </summary>
        /// <param name="orden">La orden de carga interna de fason a validar.</param>
        /// <param name="idCentro">El identificador del centro.</param>
        /// <returns>Un resultado de la validación de la orden de fason.</returns>
        public ResultadoOrdenFason Validar(OrdenCargaInternaFasonDto orden, int idCentro)
        {
            try
            {
                if (orden == null || idCentro == 0)
                {
                    this.resultado.Error("1", Textos.DatosOrdenInvalidos);
                    return this.resultado;
                };

                // Validación para verificar si KmARecorrer o localidad están vacíos
                if (orden.DerivadoGranarioHabilitado && string.IsNullOrEmpty(orden.KmARecorrer) || (!int.TryParse(orden.KmARecorrer, out int km) || km <= 0))
                {
                    this.resultado.Error("2", "KmARecorrer no pueden estar vacío o estar en cero.");
                    return this.resultado; 
                }

                var esClienteProvisorio = orden.ClienteId == 0 ? false : servicio.ObtenerCliente(orden.ClienteId).EsClienteProvisorio;

                if (esClienteProvisorio && (!orden.RemitenteId.HasValue || orden.RemitenteId == 0))
                {
                    this.resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.Remitente));
                    return this.resultado;
                }

                // Validar la orden utilizando FluentValidation
                var resultado = ValidarOrden(orden, idCentro);

                if (resultado.HayErrores)
                {
                    return resultado;
                }

                if (orden.DerivadoGranarioHabilitado && !(orden.Demorado || orden.Rechazado))
                {

                    if (!orden.PlantaDGDestino.HasValue)
                    {
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
                        return resultado;
                    }

                    if (string.IsNullOrEmpty(orden.TipoYOrdenDestino))
                    {
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
                        return resultado;
                    }
                          
                    if (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0)
                    {
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
                        return resultado;
                    }

                    if (string.IsNullOrEmpty(orden.Destinatario))
                    {
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_Destinatario));
                        return resultado;
                    }

                    var resultadoAltaDummy = EjecutarAutorizarCpeDGDummy(orden, idCentro);
                    if (resultadoAltaDummy.HayErrores)
                    {
                        resultado.Error("1", resultadoAltaDummy.Errores.Values.First());
                        return resultado;
                    }

                    var resultadoAnulacionDummy = EjecutarAnularCPEDGDummy(idCentro, resultadoAltaDummy);
                    if (resultadoAnulacionDummy.HayErrores)
                    {
                        resultado.Error("1", resultadoAnulacionDummy.Errores.Values.First());
                        return resultado;
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                this.resultado.Error("1", ex.Message);
                return this.resultado;
            }
        }

        private void ConfirmarCTGVencidos(int centroId)
        {
            servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });
        }

        private void ajustarPatentesYAcoplado(ref OrdenCargaInternaFasonDto orden)
        {
            if (orden.PatenteCamion != null)
            {
                orden.PatenteCamion = orden.PatenteCamion.ToUpper();
            }
            if (orden.PatenteAcoplado != null)
            {
                orden.PatenteAcoplado = orden.PatenteAcoplado.ToUpper();
            }
        }

        /// <summary>
        /// Valida una orden de carga interna fason.
        /// </summary>
        /// <param name="orden">La orden de carga interna fason a validar.</param>
        /// <param name="idCentro">El identificador del centro.</param>
        /// <returns>Un objeto ResultadoOrdenFason que contiene el resultado de la validación.</returns>
        /// <remarks>
        /// Este método realiza las siguientes validaciones:
        /// 1. Valida el modelo utilizando FluentValidation.
        /// 2. Si la validación falla, agrega los errores al resultado.
        /// 3. Si la validación es exitosa, realiza validaciones adicionales que requieren acceso a la base de datos.
        /// </remarks>
        private ResultadoOrdenFason ValidarOrden(OrdenCargaInternaFasonDto orden, int idCentro)
        {
            // Validar el modelo utilizando FluentValidation
            var validationResult = validacionCrearOrdenInternaFason.Validate(orden);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    // Agregar los errores al resultado 
                    resultado.Error(error.PropertyName, error.ErrorMessage);
                }

                return resultado;
            }

            // Validaciones adicionales que requieren acceso a la base de datos
            return ValidacionesAdicionales(orden, idCentro);
        }

        /// <summary>
        /// Realiza validaciones adicionales sobre la orden de carga interna Fason.
        /// </summary>
        /// <param name="orden">Objeto de tipo OrdenCargaInternaFasonDto que contiene los datos de la orden.</param>
        /// <param name="idCentro">Identificador del centro donde se realiza la validación.</param>
        /// <returns>Un objeto de tipo ResultadoOrdenFason con el resultado de las validaciones.</returns>
        /// <remarks>
        /// Las validaciones realizadas son:
        /// 1. Ajustar patentes y acoplado.
        /// 2. Verificar si el Id de Operaciones está siendo utilizado en un recorrido activo.
        /// 3. Verificar si la patente está activa en otro Workflow.
        /// 4. Setear el chofer.
        /// 5. Setear el transportista.
        /// 6. Validar si el chofer está en otro recorrido activo.
        /// 7. Obtener y setear el material derivado granario.
        /// </remarks>
        private ResultadoOrdenFason ValidacionesAdicionales(OrdenCargaInternaFasonDto orden, int idCentro)
        {
            ajustarPatentesYAcoplado(ref orden);

            // Validar si el Id de Operaciones está siendo utilizado en un recorrido activo
            if (servicio.ExisteOrdenCargaFason(orden.NumeroOrdenExterno))
            {
                resultado.Error("2", string.Format(Textos.IdOperacionesYaUtilizado, orden.NumeroOrdenExterno));
                return resultado;
            }

            // Validar si la patente está activa en otro Workflow
            if (servicio.ObtenerRecorridoActivoPorPatente(orden.PatenteCamion) != null)
            {
                resultado.Error("1", Textos.PatenteEnOtroWorkflow);
                return resultado;
            }

            // Setear el chofer
            var respuestaSetear = setearChofer(orden.Chofer);
            if (respuestaSetear == false)
            {
                return resultado;
            }

            // Setear el transportista
            var transportistaId = orden.TransportistaId;
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, orden.EsTransportista);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                return resultado;
            }

            // Validar si el chofer está en otro recorrido activo
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            if (otroRecorridoDelChofer != null)
            {
                resultado.Error("", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                return resultado;
            }

            var material = servicio.ObtenerMaterial(orden.MaterialId);
            orden.DerivadoGranarioHabilitado = material.EsDerivadoGranario;

            return resultado;
        }

        private bool SetearTransportista(ref int transportistaId, int tipoComercialId, bool esTransportista, bool esTransportistaTramo2 = false)
        {
            var tipoComercial = servicio.ObtenerTipoComercial(tipoComercialId);
            if (tipoComercial.TransportistaEsProveedor && (transportistaId == 0))
            {
                Log.Debug("El transportista es obligatorio para el tipo comercial");
                if (esTransportistaTramo2)
                    resultado.Error("TransportistaTramo2", string.Format(Textos.Error_Requerido, Textos.Transportista_Segundo_Tramo));
                else
                    resultado.Error("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
                return false;
            }

            if (!esTransportista)
            {
                var proveedor = servicio.ObtenerProveedor(transportistaId);
                if (proveedor == null)
                {
                    if (!esTransportistaTramo2)
                        resultado.Error("Transportista", string.Format(Textos.Error_ProveedorInvalido));
                    else
                        resultado.Error("TransportistaTramo2", string.Format(Textos.Error_ProveedorInvalido));
                    transportistaId = 0;
                    return false;
                }

                var transportista = servicio.ObtenerTransportistaPorCuit(proveedor.Cuil);
                if (transportista != null) //Transportista Existente
                {
                    transportistaId = transportista.Id;
                }
                else //Creo el nuevo transportista
                {
                    try
                    {
                        var resultadoTransportista = servicioComandos.Ejecutar(new CrearTransportista
                        {
                            Dto = new TransportistaDto
                            {
                                Cuit = proveedor.Cuil,
                                Domicilio = proveedor.Domicilio,
                                LocalidadId = proveedor.LocalidadId,
                                ProvinciaId = proveedor.ProvinciaId,
                                RazonSocial = proveedor.RazonSocial
                            }
                        });
                        if (resultadoTransportista.HayErrores)
                        {
                            resultadoTransportista.Errores
                                .ToList()
                                .ForEach(kvp => resultado.Error(kvp.Key, kvp.Value));

                            return false;
                        }
                        transportistaId = (resultadoTransportista as ResultadoCrear).Id;
                    }
                    catch
                    {
                        resultado.Error("Transportista", string.Format(Textos.Error_ProveedorInvalido));
                    }
                }
            }

            return true;
        }

        private bool setearChofer(ChoferDto choferDto)
        {
            Log.Info("SetearChofer para el chofer con el CUIL: " + choferDto.Cuil);
            var chofer = servicio.BuscarChoferes(new ChoferFiltro { Cuil = choferDto.Cuil })?.FirstOrDefault();
            if (chofer != null) //Chofer existente
            {
                Log.Info("SetearChofer se actualizará el chofer con CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new ModificarChofer() { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    Log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        resultado.Error("Chofer." + f.Key, f.Value);
                        Log.Debug(f.Value);
                    });

                    return false;
                }
            }
            else //ChoferNuevo
            {
                Log.Info("SetearChofer se dará de alta el chofer con el CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new CrearChofer { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    Log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        resultado.Error("Chofer." + f.Key, f.Value);
                        Log.Debug(f.Value);
                    });
                    return false;
                }
                choferDto.Id = (resultadoChofer as ResultadoCrear).Id;
            }
            return true;
        }

        private ResultadoCartaPorteElectronicaDummy EjecutarAutorizarCpeDGDummy(OrdenCargaInternaFasonDto orden, int idCentro)
        {

            var dominios = new List<string> { orden.PatenteCamion };
            if (!string.IsNullOrEmpty(orden.PatenteAcoplado))
            {
                dominios.Add(orden.PatenteAcoplado);
            }

            ConfirmarCTGVencidos(idCentro);

            return servicioComandos.Ejecutar(new AutorizarCpeDGDummy
            {
                TipoVehiculo = orden.TipoVehiculo,
                CentroId = idCentro,
                MaterialId = orden.MaterialId,
                DestinoId = orden.ClienteId,
                DestinatarioId = orden.DestinatarioId,
                DestinoPlanta = orden.PlantaDGDestino ?? 0,
                DestinoDomicilioTipo = orden.TipoDomicilioDestino ?? 0,
                DestinoDomicilioOrden = orden.OrdenDomicilioDestino ?? 0,
                TransportistaId = orden.TransportistaId,
                Dominios = dominios.ToArray(),
                KmRecorrer = !string.IsNullOrEmpty(orden.KmARecorrer) ? int.Parse(orden.KmARecorrer) : 0,
                ChoferCuit = orden.Chofer.Cuil,
                PagadorFleteId = orden.PagadorFleteId ?? 0,
                CorredorId = orden.CorredorId,
                RemitenteId = orden.RemitenteId,
                ComisionistaId = orden.ComisionistaId,
                IntermediarioFleteId = orden.IntermediarioFleteId,
                AplicaDestinatario = true,
                Observaciones = orden.Observaciones
            }) as ResultadoCartaPorteElectronicaDummy;
        }

        private Resultado EjecutarAnularCPEDGDummy(int idCentro, ResultadoCartaPorteElectronicaDummy resultadoAltaDummy)
        {
            var resul = servicioComandos.Ejecutar(new AnularCPEDGDummy
            {
                CentroId = idCentro,
                NroOrden = (int)resultadoAltaDummy.NroOrden,
                Sucursal = resultadoAltaDummy.Sucursal,
                TipoCPE = (short)resultadoAltaDummy.TipoCPE,
            });

            return resul;
        }

        private OrdenDeCargaDto ObtenerOrdenOperaciones(string patente, int materialId)
        {
            // Obtener código SAP del material que llega de carga de cupo
            var materialCodigoSap = Repositorio.ObtenerProyeccion<Material, string>(x => x.Id == materialId, x => x.CodigoSAP);
            
            // Obtener todas las órdenes para la patente
            var ordenesOperaciones = servicioOperaciones.ObtenerOrdenesDeCarga(patente);

            // Filtrar por el código SAP del material que viene de operaciones
            var ordenesFiltradas = ordenesOperaciones.Where(x => x.CodigoProducto == materialCodigoSap).ToList();        

            if (!ordenesFiltradas.Any())
                throw new OrdenCargaInternaException(nameof(OrdenDeCargaDto.Id), Textos.OrdenesFasonNoEncontradas);

            // Verificar si hay múltiples clientes para el material
            var clientesDistintos = ordenesFiltradas.Select(o => o.CUITCliente).Distinct().Count();
            if (clientesDistintos > 1)
            {
                throw new OrdenCargaInternaException("ValidacionFason",Textos.Multiples_Clientes_Mismo_Material);
            }

            //Ordenar por la mas antigua por el Id de la orden
            var ordenOperacionesMasAntigua = ordenesOperaciones.OrderBy(x => x.Id).FirstOrDefault();
            if (ordenOperacionesMasAntigua == null)
                throw new OrdenCargaInternaException(nameof(OrdenDeCargaDto.Id), Textos.OrdenesFasonNoEncontradas);

            return ordenOperacionesMasAntigua;
        }

        private ControlRecorridoDto GenerarControlRecorrido(int puestoId, string usuario, bool esFastPass)
        {
            return new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenCargaInternaFason,
                ActividadXaml = "IngresarOrdenCargaInternaFason",
                PuestoDeTrabajoId = puestoId,
                NombreUsuario = usuario,
                Comentario = esFastPass ? Textos.AvanzoFastPass : string.Empty,
            };
        }
    }
}
