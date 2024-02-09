using PRJEntrevistaNTComunicaciones.CapaDatos;
using PRJEntrevistaNTComunicaciones.Models;
namespace PRJEntrevistaNTComunicaciones.CapaNegocio
{
    public class Firmas_CN
    {

        private FirmasDAO objfirmasdao;


        public Firmas_CN(IConfiguration config)
        {

            objfirmasdao = new FirmasDAO(config);
        }

      

        public List<firmaDigital> ListadoFirmasDigitales()
        {
            return objfirmasdao.ListadoFirmasDigitales();
        }


        public List<Tipofirma> ListarTipofirma()
        {
            return objfirmasdao.ListarTipofirma();
        }





        //base64
        public  string convertirbase64(string ruta, out bool conversion)
        {
            string textobase64 = string.Empty;
            conversion = true;

            try
            {
                byte[] bytes = File.ReadAllBytes(ruta);
                textobase64 = Convert.ToBase64String(bytes);
            }
            catch
            {

                conversion = false;
            }
            return textobase64;
        }






        public int registrar(firmaDigital obj, out string mensaje)
        {
            mensaje = string.Empty;
            if (obj.otipofirma.idtipofirma == 0)
            {
                mensaje = "Debes Seleccionar un Tipo de Firma";
            }else if (string.IsNullOrEmpty(obj.RazonSocial) || string.IsNullOrWhiteSpace(obj.RazonSocial))
            {
                mensaje = "La Razon Social no puede ir vacio";
            }else if(string.IsNullOrEmpty(obj.RepresentanteLegal) || string.IsNullOrWhiteSpace(obj.RepresentanteLegal))
            {
                mensaje = "El Representante Legal no puede ir vacio";
            }
            else if (string.IsNullOrEmpty(obj.EmpresaAcreditadora) || string.IsNullOrWhiteSpace(obj.EmpresaAcreditadora))
            {
                mensaje = "La Empresa Acreditadora no puede ir vacio";
            }
            else if (obj.FechaEmision == DateTime.MinValue) 
            {
                mensaje = "Debe Seleccionar una Fecha de Emision válida";
            }
            else if (obj.FechaVencimiento == DateTime.MinValue)
            {
                mensaje = "Debe Seleccionar una Fecha de Vencimiento válida";
            }

            if (string.IsNullOrEmpty(mensaje))
            {

                return objfirmasdao.registrar(obj, out mensaje);

            }

            else
            {
                return  0;
            }

        }


        public bool editar(firmaDigital obj, out string mensaje)
        {
            mensaje=string.Empty;
            if (obj.otipofirma.idtipofirma == 0)
            {
                mensaje = "Debes Seleccionar un Tipo de Firma";
            }
            else if (string.IsNullOrEmpty(obj.RazonSocial) || string.IsNullOrWhiteSpace(obj.RazonSocial))
            {
                mensaje = "La Razon Social no puede ir vacio";
            }
            else if (string.IsNullOrEmpty(obj.RepresentanteLegal) || string.IsNullOrWhiteSpace(obj.RepresentanteLegal))
            {
                mensaje = "El Representante Legal no puede ir vacio";
            }
            else if (string.IsNullOrEmpty(obj.EmpresaAcreditadora) || string.IsNullOrWhiteSpace(obj.EmpresaAcreditadora))
            {
                mensaje = "La Empresa Acreditadora no puede ir vacio";
            }
            else if (obj.FechaEmision == DateTime.MinValue)
            {
                mensaje = "Debe Seleccionar una Fecha de Emision válida";
            }
            else if (obj.FechaVencimiento == DateTime.MinValue)
            {
                mensaje = "Debe Seleccionar una Fecha de Vencimiento válida";
            }
            if (string.IsNullOrEmpty(mensaje))
            {
                return objfirmasdao.editar(obj, out mensaje);

            }
            else
            {
                return false;
            }


        }

        public bool Eliminar(int id)
        {
            return objfirmasdao.Eliminar(id);
        }


        //Meotodos de la img y archivo
        public bool guardardatosimagen(firmaDigital obj, out string mensaje)
        {
            return objfirmasdao.guardardatosimagen(obj,out  mensaje);
        }


        public bool guardardatosArchivos(firmaDigital obj, out string mensaje)
        {
            return objfirmasdao.guardardatosArchivos(obj, out mensaje);
        }


       

    }
}
