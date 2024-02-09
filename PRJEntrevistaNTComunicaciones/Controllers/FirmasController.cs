using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRJEntrevistaNTComunicaciones.CapaNegocio;
using PRJEntrevistaNTComunicaciones.Models;
using System.Configuration;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PRJEntrevistaNTComunicaciones.Controllers
{

    public class FirmasController : Controller
    {

        private readonly Firmas_CN firmascn;
        public FirmasController(Firmas_CN _firmascn)
        {
            firmascn = _firmascn;
        }
    

        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public JsonResult ListadoFirmasDigitales()
        {
            List<firmaDigital> olita=firmascn.ListadoFirmasDigitales();
            return Json(new {data= olita });
        }



        [HttpGet]
        public JsonResult ListadoTiposFirmas()
        {
            List<Tipofirma> olistatipo=firmascn.ListarTipofirma();
            return Json(new { data= olistatipo });
        }




        //Registrar en generar 

        [HttpPost]
        [Consumes("multipart/form-data")]
            public JsonResult GuardarFirmasGeneral([FromForm] IFormFile archivoImagen, [FromForm] IFormFile archivoArchivo, [FromForm] string obj, [FromServices] IConfiguration configuration)

        {
            string mensaje = string.Empty;
            bool operacionExitosa = true;
            bool guardarImagenExito = true;
            bool guardarArchivoExito = true;

            // Convertir obj a firmaDigital
            firmaDigital ofirmaDigital = JsonConvert.DeserializeObject<firmaDigital>(obj);

            if (ofirmaDigital.IdFirma == 0)
            {
                int idFirmaGenerado = firmascn.registrar(ofirmaDigital, out mensaje);
                if (idFirmaGenerado != 0)
                {
                    ofirmaDigital.IdFirma = idFirmaGenerado;
                }
                else
                {
                    operacionExitosa = false;
                }
            }
            else
            {
                 operacionExitosa = new Firmas_CN(configuration).editar(ofirmaDigital, out mensaje);
            }

            if (operacionExitosa)
            {
                if (archivoImagen != null)
                {
                    string rutaGuardarImagen = configuration.GetSection("AppSettings:ServidorFoto").Value;
                    string extensionImagen = Path.GetExtension(archivoImagen.FileName);
                    string nombreImagen = $"{ofirmaDigital.IdFirma}{extensionImagen}";

                    try
                    {
                        using (var fileStream = new FileStream(Path.Combine(rutaGuardarImagen, nombreImagen), FileMode.Create))
                        {
                            archivoImagen.CopyTo(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        guardarImagenExito = false;
                    }

                    if (guardarImagenExito)
                    {
                        ofirmaDigital.extensionruta = rutaGuardarImagen;
                        ofirmaDigital.RutaRubrica = nombreImagen;
                        bool rptaImagen = firmascn.guardardatosimagen(ofirmaDigital, out mensaje);
                    }
                    else
                    {
                        mensaje = "Se guardó la firma, pero hubo problemas con la imagen";
                    }
                }

                if (archivoArchivo != null)
                {
                    string rutaGuardarArchivo = configuration.GetSection("AppSettings:ServidorArchivo").Value;
                    string extensionArchivo = Path.GetExtension(archivoArchivo.FileName);
                    string nombreArchivo = $"{ofirmaDigital.IdFirma}{extensionArchivo}";

                    try
                    {
                        using (var fileStream = new FileStream(Path.Combine(rutaGuardarArchivo, nombreArchivo), FileMode.Create))
                        {
                            archivoArchivo.CopyTo(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        guardarArchivoExito = false;
                    }

                    if (guardarArchivoExito)
                    {
                        ofirmaDigital.extensioncertificado = rutaGuardarArchivo;
                        ofirmaDigital.CertificadoDigital = nombreArchivo;
                        bool rptaArchivo = firmascn.guardardatosArchivos(ofirmaDigital, out mensaje);
                    }
                    else
                    {
                        mensaje = "Se guardó la firma, pero hubo problemas con el archivo";
                    }
                }
            }

            return Json(new { operacionExitosa = operacionExitosa, idGenerado = ofirmaDigital.IdFirma, mensaje = mensaje });
        }

        //Eliminar
        [HttpPost]
        public JsonResult EliminarFirmas(int id)
        {
            try
            {
                bool respuesta = firmascn.Eliminar(id);
                return Json(new { resultado = respuesta });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }


        //Imagenfirma

        [HttpPost]
        public JsonResult ImagenFirmas(int id, [FromServices] IConfiguration configuration)
        {
            bool conversion;
            firmaDigital ofirmaDigital = firmascn.ListadoFirmasDigitales().FirstOrDefault(p => p.IdFirma == id);
            string textobase64 = firmascn.convertirbase64(Path.Combine(ofirmaDigital.extensionruta, ofirmaDigital.RutaRubrica), out conversion);

            return Json(new
            {
                conversion = conversion,
                textobase64 = textobase64,
                extension = Path.GetExtension(ofirmaDigital.RutaRubrica)
            });
        }


        //archivo firma

        [HttpPost]
        public JsonResult ArchivoFirmas(int id, [FromServices] IConfiguration configuration)
        {
            bool conversion;
            firmaDigital ofirmaDigital = firmascn.ListadoFirmasDigitales().FirstOrDefault(p => p.IdFirma == id);
            string textobase64 = firmascn.convertirbase64(Path.Combine(ofirmaDigital.extensioncertificado, ofirmaDigital.CertificadoDigital), out conversion);

            return Json(new
            {
                conversion = conversion,
                textobase64 = textobase64,
                extension = Path.GetExtension(ofirmaDigital.CertificadoDigital)
            });
        }


        #region

        /*

        [HttpPost]
        public JsonResult ImagenFirmas(int id)
        {
            bool conversion;
            // Obtén IConfiguration del constructor del controlador
            IConfiguration configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            // Pasa IConfiguration al constructor de Firmas_CN
            Firmas_CN firmasCN = new Firmas_CN(configuration);
            firmaDigital ofirmaDigital = firmasCN.ListadoFirmasDigitales().FirstOrDefault(p => p.IdFirma == id);


            //  firmaDigital ofirmaDigital = new Firmas_CN().ListadoFirmasDigitales().FirstOrDefault(p => p.IdFirma == id);
            string textobase64 = firmascn.convertirbase64(Path.Combine(ofirmaDigital.extensionruta, ofirmaDigital.RutaRubrica), out conversion);

            return Json(new
            {
                conversion = conversion,
                textobase64 = textobase64,
                extension = Path.GetExtension(ofirmaDigital.RutaRubrica)
            });
        }



        [HttpPost]
        public JsonResult ArchivoFirmas(int id)
        {
            bool conversion;

            // Obtén IConfiguration del constructor del controlador
            IConfiguration configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            // Pasa IConfiguration al constructor de Firmas_CN
            Firmas_CN firmasCN = new Firmas_CN(configuration);

            firmaDigital ofirmaDigital = firmasCN.ListadoFirmasDigitales().FirstOrDefault(p => p.IdFirma == id);

            string textobase64 = firmasCN.convertirbase64(Path.Combine(ofirmaDigital.extensioncertificado, ofirmaDigital.CertificadoDigital), out conversion);

            return Json(new
            {
                conversion = conversion,
                textobase64 = textobase64,
                extension = Path.GetExtension(ofirmaDigital.CertificadoDigital)
            });
        }


        */


        #endregion

    }
}
