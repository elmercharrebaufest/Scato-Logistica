using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Validations.Interfaces;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarOrdenCargaInternaFas : ProcesadorComando<ValidarOrdenCargaInternaFas>
    {
        protected readonly IServicioComandos servicioComandos;
        private readonly IValidatorEntity<OrdenCargaFasDto> validacionCrearOrdenInternaFas;
        protected readonly IServicioRepositorio servicio;
        private readonly IServicioOperaciones servicioOperaciones;
        private ResultadoOrdenFas resultado;
        private int _clienteId;

        public ProcesadorValidarOrdenCargaInternaFas(IServicioComandos servicioComandos, IValidatorEntity<OrdenCargaFasDto> validacionCrearOrdenInternaFas, IServicioRepositorio servicio, IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOperaciones servicioOperaciones)
                 : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.validacionCrearOrdenInternaFas = validacionCrearOrdenInternaFas;
            this.servicio = servicio;
            this.resultado = new ResultadoOrdenFas();
            this.servicioOperaciones = servicioOperaciones;
        }

        /// <summary>
        /// Ejecuta la validación de la orden de carga fas.
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
        public override Resultado Ejecutar(ValidarOrdenCargaInternaFas comando)
        {
            try
            {
                var materialId = int.Parse(comando.Orden.MaterialId);
                var centro = servicio.ObtenerCentro(comando.CentroId);
                var patente = comando.Orden.PatenteChasis;
                var workflowObj = servicio.ObtenerWorkflowPorCodigo(Constantes.WorkFlow.workflowVentaFas);

                OrdenDeCargaSapDto orden = new OrdenDeCargaSapDto(centro.CodigoSAP, patente, Constantes.WorkFlow.workflowVentaFas);
                var respuestaSap = servicioComandos.Ejecutar(new ConsultarOrdenCargaFas { Orden = orden }) as ResultadoConsultaOrdenCargaFas;

                if (respuestaSap.HayErrores)
                {
                    Log.Error("Error al obtener la orden de carga de SAP");
                    respuestaSap.Errores.ToList().ForEach(x => resultado.Error(x.Key, x.Value));
                }

                var ordenFas = respuestaSap.Orden.Where(x => x.MaterialId == materialId).OrderBy(x => x.Id).FirstOrDefault();
                ValidarOrdenFront(ref ordenFas, materialId, comando.CentroId, comando.Usuario, comando.AplicaFastPass);

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
        /// Valida y procesa una orden de carga fas.
        /// </summary>
        /// <param name="ordenOperaciones">Datos de la orden de carga.</param>
        /// <param name="materialId">ID del material.</param>
        /// <param name="centroId">ID del centro.</param>
        /// <param name="usuario">Usuario que realiza la operación.</param>
        /// <returns>Resultado de la validación de la orden de carga fas.</returns>
        /// <remarks>
        /// Este método realiza las siguientes validaciones:
        /// 1. Obtiene el workflow correspondiente según el tipo de flete fas.
        /// 2. Convierte los CUITs y CUILs de la orden a formato de scato con '-'.
        /// 3. Obtiene los datos de clientes, chofer y proveedor intermediario de flete.
        /// 4. Genera y ejecuta la consulta de escalables.
        /// 5. Lista los materiales por workflow y verifica si el material es derivado granario.
        /// 6. Llena los datos de la orden de carga fas.
        /// 7. Maneja posibles errores durante el proceso.
        /// </remarks>
        private void ValidarOrdenFront(ref OrdenCargaFasDto orden, int? materialId, int centroId, string usuario, bool aplicaFastPass)
        {
            try
            {
                string workflowDescripcion = Constantes.WorkFlow.workflowVentaFas;
                var workflow = servicio.ObtenerWorkflowPorCodigo(workflowDescripcion);

                // Obtener datos de una vez
                var tiposComerciales = servicio.ListarTiposComercialesPorWfCodigo(workflow.Codigo);
                var materiales = servicio.ListarMaterialesPorWorkflow(workflow.Id, centroId);
                var materialesPermitidos = materiales.Select(m => m.MaterialId).ToList();

                // Validación de material
                if (materialId.HasValue && !materialesPermitidos.Contains(materialId.Value))
                {
                    Log.Error(Textos.MaterialNoPermitido);
                    resultado.Error("2", Textos.MaterialNoPermitido);
                    return; // Terminamos la ejecución si el material no es válido
                }

                // Construcción de diccionarios
                var cuils = new Dictionary<string, string>
                {
                    { "Cliente", orden?.ClienteCuit },
                    { "Transporte", orden?.CuitTransporte },
                    { "Chofer", orden?.Chofer?.Cuil },
                    { "Destinatario", orden?.DestinatarioCuit },
                    { "PagadorFlete", orden?.PagadorFleteCuit }
                };

                if (!string.IsNullOrEmpty(orden?.RemitenteCuit))
                    cuils["Remitente"] = orden.RemitenteCuit;

                if (!string.IsNullOrEmpty(orden?.IntermediarioFlete))
                    cuils["IntermediarioFlete"] = orden.IntermediarioFlete;

                var clientes = cuils
                    .Where(kv => !string.IsNullOrEmpty(kv.Value)) // Filtramos los valores vacíos
                    .ToDictionary(kv => kv.Key, kv => servicio.ObtenerClientePorCuit(kv.Value));

                var chofer = servicio.ObtenerChoferPorCuit(cuils["Chofer"]);
                var transportista = servicio.ObtenerProveedorPorCuit(cuils["Transporte"], new TiposProveedor { PR = true });
                var intermediarioFlete = clientes.ContainsKey("IntermediarioFlete")
                    ? servicio.ObtenerProveedorPorCuit(cuils["IntermediarioFlete"], new TiposProveedor { PR = true })
                    : null;

                // Obtener resultado escalables
                var resultadoEscalables = servicioComandos.Ejecutar(GenerarConsultaEscalables(orden.PatenteCamion, orden.PatenteAcoplado, usuario)) as ResultadoEscalables;
                if (resultadoEscalables.HayErrores || !resultadoEscalables.Categoria.HasValue)
                {
                    Log.Error(resultadoEscalables.HayErrores ? resultadoEscalables.Errores.FirstOrDefault().Value : Textos.CategoriaEscalable_Nula);
                    resultado.Error("1", resultadoEscalables.HayErrores ? resultadoEscalables.Errores.FirstOrDefault().Value : Textos.CategoriaEscalable_Nula);
                    return;
                }

                // Procesar material y orden
                var material = materiales.FirstOrDefault(x => x.MaterialId == materialId);
                bool esDerivado = material?.EsDerivadoGranario ?? false;

                orden.TipoComercialId = tiposComerciales.FirstOrDefault()?.Id ?? 0;
                orden.TipoComercialDesc = tiposComerciales.FirstOrDefault()?.Descripcion;
                orden.MaterialId = material?.MaterialId ?? materialId ?? 0;
                orden.MaterialDesc = material?.MaterialDesc;
                orden.EsExtranjero = false;
                orden.VehiculoDemorado = false;
                orden.Rechazado = false;
                orden.TipoVehiculo = resultadoEscalables.Categoria.Value;
                orden.TransportistaId = transportista?.Id ?? 0;

                // Asignación de clientes
                AsignarCliente(orden, clientes, "Cliente");
                AsignarCliente(orden, clientes, "PagadorFlete");
                AsignarCliente(orden, clientes, "Remitente");

                if (intermediarioFlete != null)
                {
                    orden.IntermediarioFlete = intermediarioFlete.Descripcion;
                    orden.IntermediarioFleteId = intermediarioFlete.Id;
                }

                orden.DerivadoGranarioHabilitado = esDerivado;
                if (esDerivado)
                {
                    orden.PlantaDGDestino = orden.PlantaDGDestino;
                    orden.TipoYOrdenDestino = $"{orden.TipoDomicilioDestino}-{orden.OrdenDomicilioDestino}";
                }
                else
                {
                    orden.PlantaDGDestino = null;
                    orden.TipoYOrdenDestino = null;
                }

                resultado.Dto = orden;
                resultado.ControlRecorrido = GenerarControlRecorrido(0, usuario, aplicaFastPass);
            }
            catch (Exception ex)
            {
                Log.Error($"{Textos.ErrorLlenarCaomposEntidad} {ex.Message}");
                resultado.Error("1", $"{Textos.ErrorLlenarCaomposEntidad} {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las plantas DG para un centro y cliente específicos.
        /// </summary>
        /// <param name="centroId">Identificador del centro.</param>
        /// <param name="clienteId">Identificador del cliente.</param>
        /// <param name="plantaId">Identificador de la planta (opcional).</param>
        /// <param name="clienteCuit">CUIT del cliente (opcional).</param>
        /// <returns>Resultado de la operación.</returns>
        /// <remarks>
        /// Validaciones realizadas:
        /// 1. Se obtiene el CUIT del cliente.
        /// 2. Se ejecuta el comando para consultar las plantas Derivados Granarios en Arca.
        /// 3. Se manejan los errores del resultado.
        /// 4. Se verifica si la planta seleccionada es válida.
        /// </remarks>
        public Resultado ObtenerPlantasDg(int centroId, int clienteId, int? plantaId, string clienteCuit = null)
        {
            var cuit = ObtenerCuit(clienteId, clienteCuit);
            var result = servicioComandos.Ejecutar(new ConsultarPlantasDG
            {
                CentroId = centroId,
                Cuit = long.Parse(cuit)
            }) as ResultadoConsultaPlantasDG;

            ManejarErrores(result.Errores);

            if (!result.Plantas.Any(p => p.Equals(plantaId)))
            {
                Log.Error(Textos.PlantaNoValida);
                resultado.Error("2", Textos.PlantaNoValida);
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

        /// <summary>
        /// Obtiene los domicilios Derivados Granarios desde el Arca para un centro y cliente específicos.
        /// </summary>
        /// <param name="centroId">Identificador del centro.</param>
        /// <param name="clienteId">Identificador del cliente.</param>
        /// <param name="tipoDomicilio">Identificador del tipo domicilio.</param>
        /// <param name="ordenDomicilio">Identificador del tipo orden domicilio.</param>
        /// <param name="clienteCuit">CUIT del cliente (opcional).</param>
        /// <returns>Resultado de la operación con los domicilios obtenidos.</returns>
        public Resultado ObtenerDomiciliosDg(int centroId, int clienteId, int? tipoDomicilio, int? ordenDomicilio, string clienteCuit = null)
        {
            var cuit = ObtenerCuit(clienteId, clienteCuit);
            var result = servicioComandos.Ejecutar(new ConsultarDomiciliosDG
            {
                CentroId = centroId,
                Cuit = long.Parse(cuit)
            }) as ResultadoConsultaDomiciliosDG;

            ManejarErrores(result.Errores);

            if (!result.Domicilios.Any(p => p.Tipo == tipoDomicilio && p.Orden == ordenDomicilio))
            {
                Log.Error(Textos.DomicilioNoValido);
                resultado.Error("2", Textos.DomicilioNoValido);
            }

            return resultado;
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
        /// Valida la orden de carga fas.
        /// Se realizan las siguientes validaciones:
        /// - Validación de datos de la orden.
        /// - Validación de transportista y chofer.
        /// - Validación de patentes y acoplados.
        /// - Validación de planta y domicilio.
        /// - Validación de CTG y CPE.
        /// </summary>
        /// <param name="orden">La orden de carga fas a validar.</param>
        /// <param name="idCentro">El identificador del centro.</param>
        /// <returns>Un resultado de la validación de la orden de fas.</returns>
        public ResultadoOrdenFas Validar(OrdenCargaFasDto orden, int idCentro)
        {
            try
            {
                if (orden == null || idCentro == 0)
                {
                    Log.Error(Textos.DatosOrdenInvalidos);
                    this.resultado.Error("1", Textos.DatosOrdenInvalidos);
                    return this.resultado;
                }
                ;

                // Validación para verificar si KmARecorrer está vacío
                if (orden.DerivadoGranarioHabilitado &&
                    (string.IsNullOrEmpty(orden.KmARecorrer) ||
                     !int.TryParse(orden.KmARecorrer, out int km) ||
                     km <= 0))
                {
                    var mensajeError = $"{orden.PatenteCamion}: KmARecorrer no pueden estar vacío o estar en cero.";
                    Log.Error(mensajeError);
                    this.resultado.Error("2", mensajeError);
                    return this.resultado;
                }

                var esClienteProvisorio = orden.ClienteId == 0 ? false : servicio.ObtenerCliente(orden.ClienteId).EsClienteProvisorio;

                if (esClienteProvisorio && (!orden.RemitenteId.HasValue || orden.RemitenteId == 0))
                {
                    Log.Error(string.Format(Textos.Error_Requerido, Textos.Remitente));
                    this.resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.Remitente));
                    return this.resultado;
                }

                //Procesar Domicilio y Destino
                ProcesarTipoYOrdenDestino(ref orden);

                // Validar la orden utilizando FluentValidation
                var resultado = ValidarOrden(orden, idCentro);

                if (resultado.HayErrores)
                {
                    return resultado;
                }

                if (orden.DerivadoGranarioHabilitado && !(orden.VehiculoDemorado || orden.Rechazado))
                {
                    if (!orden.PlantaDGDestino.HasValue)
                    {
                        Log.Error(string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));
                        return resultado;
                    }

                    if (string.IsNullOrEmpty(orden.TipoYOrdenDestino))
                    {
                        Log.Error(string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));
                        return resultado;
                    }

                    if (!orden.PagadorFleteId.HasValue || orden.PagadorFleteId <= 0)
                    {
                        Log.Error(string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_CuitPagadorFlete));
                        return resultado;
                    }

                    if (string.IsNullOrEmpty(orden.DestinatarioDesc))
                    {
                        Log.Error(string.Format(Textos.Error_Requerido, Textos.OrdenCarga_Destinatario));
                        resultado.Error("1", string.Format(Textos.Error_Requerido, Textos.OrdenCarga_Destinatario));
                        return resultado;
                    }

                    var validarPlantas = ObtenerPlantasDg(idCentro, _clienteId, orden.PlantaDGDestino, orden.ClienteCuit) as ResultadoOrdenFas;
                    if (validarPlantas.HayErrores)
                    {
                        return validarPlantas;
                    }

                    var validarDomicilios = ObtenerDomiciliosDg(idCentro, _clienteId, orden.TipoDomicilioDestino, orden.OrdenDomicilioDestino, orden.ClienteCuit) as ResultadoOrdenFas;
                    if (validarDomicilios.HayErrores)
                    {
                        return validarDomicilios;
                    }

                    var resultadoAltaDummy = EjecutarAutorizarCpeDGDummy(orden, idCentro);
                    if (resultadoAltaDummy.HayErrores)
                    {
                        Log.Error(resultadoAltaDummy.Errores.Values.First());
                        resultado.Error("1", resultadoAltaDummy.Errores.Values.First());
                        return resultado;
                    }

                    var resultadoAnulacionDummy = EjecutarAnularCPEDGDummy(idCentro, resultadoAltaDummy);
                    if (resultadoAnulacionDummy.HayErrores)
                    {
                        Log.Error(resultadoAnulacionDummy.Errores.Values.First());
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
            //servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            //{
            //    CentroId = centroId,
            //    TipoPerfil = TipoPerfil.Solicitante,
            //});
        }

        private void ajustarPatentesYAcoplado(ref OrdenCargaFasDto orden)
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
        /// Valida una orden de carga interna fas.
        /// </summary>
        /// <param name="orden">La orden de carga fas a validar.</param>
        /// <param name="idCentro">El identificador del centro.</param>
        /// <returns>Un objeto ResultadoOrdenFas que contiene el resultado de la validación.</returns>
        /// <remarks>
        /// Este método realiza las siguientes validaciones:
        /// 1. Valida el modelo utilizando FluentValidation.
        /// 2. Si la validación falla, agrega los errores al resultado.
        /// 3. Si la validación es exitosa, realiza validaciones adicionales que requieren acceso a la base de datos.
        /// </remarks>
        private ResultadoOrdenFas ValidarOrden(OrdenCargaFasDto orden, int idCentro)
        {
            // Validar el modelo utilizando FluentValidation
            var validationResult = validacionCrearOrdenInternaFas.Validate(orden);

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
        /// Realiza validaciones adicionales sobre la orden de carga fas.
        /// </summary>
        /// <param name="orden">Objeto de tipo OrdenCargaFasDto que contiene los datos de la orden.</param>
        /// <param name="idCentro">Identificador del centro donde se realiza la validación.</param>
        /// <returns>Un objeto de tipo ResultadoOrdenFas con el resultado de las validaciones.</returns>
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
        private ResultadoOrdenFas ValidacionesAdicionales(OrdenCargaFasDto orden, int idCentro)
        {
            ajustarPatentesYAcoplado(ref orden);

            // Validar si el Id de Operaciones está siendo utilizado en un recorrido activo
            if (servicio.ExisteOrdenCargaFas(orden.NumeroOrden))
            {
                string json = JsonConvert.SerializeObject(orden, Formatting.Indented);
                Log.Info("Orden: " + json);
                Log.Error(string.Format("El numero de orden ya fue utilizado", orden.NumeroOrden));
                resultado.Error("2", string.Format("El numero de orden ya fue utilizado", orden.NumeroOrden));
                return resultado;
            }

            // Validar si la patente está activa en otro Workflow
            if (servicio.ObtenerRecorridoActivoPorPatente(orden.PatenteCamion) != null)
            {
                Log.Error(Textos.PatenteEnOtroWorkflow);
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
            var resultadoTransportista = SetearTransportista(ref transportistaId, orden.TipoComercialId, false);
            orden.TransportistaId = transportistaId;
            if (!resultadoTransportista)
            {
                return resultado;
            }

            // Validar si el chofer está en otro recorrido activo
            var otroRecorridoDelChofer = servicio.ObtenerOtroRecorridoDelChofer(orden.Chofer.Id);
            if (otroRecorridoDelChofer != null)
            {
                Log.Error(string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
                resultado.Error("1", string.Format(Textos.Error_ChoferYaEstaEnPlanta, orden.Chofer.NombreCompleto, otroRecorridoDelChofer.NumeroDocumentoIngreso, otroRecorridoDelChofer.Patente));
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
                {
                    Log.Error(string.Format(Textos.Error_Requerido, Textos.Transportista_Segundo_Tramo));
                    resultado.Error("TransportistaTramo2", string.Format(Textos.Error_Requerido, Textos.Transportista_Segundo_Tramo));
                }
                else
                {
                    Log.Error(string.Format(Textos.Error_Requerido, Textos.Transportista));
                    resultado.Error("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
                }
                return false;
            }

            if (!esTransportista)
            {
                var proveedor = servicio.ObtenerProveedor(transportistaId);
                if (proveedor == null)
                {
                    if (!esTransportistaTramo2)
                    {
                        Log.Error(string.Format(Textos.Error_ProveedorInvalido));
                        resultado.Error("Transportista", string.Format(Textos.Error_ProveedorInvalido));
                    }
                    else
                    {
                        Log.Error(string.Format(Textos.Error_ProveedorInvalido));
                        resultado.Error("TransportistaTramo2", string.Format(Textos.Error_ProveedorInvalido));
                    }

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
                        Log.Error(string.Format(Textos.Error_ProveedorInvalido));
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

        private void ProcesarTipoYOrdenDestino(ref OrdenCargaFasDto orden)
        {
            try
            {
                if (orden.TipoYOrdenDestino != null)
                {
                    var domicilio = orden.TipoYOrdenDestino.Split('-');
                    orden.TipoDomicilioDestino = int.Parse(domicilio[0]);
                    orden.OrdenDomicilioDestino = int.Parse(domicilio[1]);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private ResultadoCartaPorteElectronicaDummy EjecutarAutorizarCpeDGDummy(OrdenCargaFasDto orden, int idCentro)
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
                AplicaDestinatario = true
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

        private ControlRecorridoDto GenerarControlRecorrido(int puestoId, string usuario, bool esFastPass)
        {
            return new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenCargaFas,
                ActividadXaml = "IngresarOrdenCargaFas",
                PuestoDeTrabajoId = puestoId,
                NombreUsuario = usuario,
                Comentario = esFastPass ? Textos.AvanzoFastPass : string.Empty,
            };
        }

        private void AsignarCliente(OrdenCargaFasDto orden, Dictionary<string, ClienteDto> clientes, string key)
        {
            if (clientes.TryGetValue(key, out var cliente) && cliente != null)
            {
                orden.GetType().GetProperty($"{key}Desc")?.SetValue(orden, cliente.Descripcion);
                orden.GetType().GetProperty($"{key}Id")?.SetValue(orden, cliente.Id);
            }
            else
            {
                orden.GetType().GetProperty($"{key}Desc")?.SetValue(orden, string.Empty);
                orden.GetType().GetProperty($"{key}Id")?.SetValue(orden, 0);
            }
        }
    }
}