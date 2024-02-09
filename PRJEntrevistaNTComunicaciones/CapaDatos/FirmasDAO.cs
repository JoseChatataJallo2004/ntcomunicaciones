using System.Data.SqlClient;
using System.Data;
using System.Text;
using PRJEntrevistaNTComunicaciones.Models;

namespace PRJEntrevistaNTComunicaciones.CapaDatos
{
    public class FirmasDAO
    {
        private readonly string cad_conex;
        public FirmasDAO(IConfiguration config)
        {
            cad_conex = config.GetConnectionString("JoseChatata");
        }

        //Listado de los Tipos de Documento
        public List<Tipofirma> ListarTipofirma()
        {
            List<Tipofirma> lista = new List<Tipofirma>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(cad_conex))
                {
                    string query = "select * from TipoFirma";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Tipofirma()
                            {
                                idtipofirma = Convert.ToInt32(dr["idtipofirma"]),
                                descripcion = dr["descripcion"].ToString(),
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Tipofirma>();
            }
            return lista;
        }



        //Listado de las Firmas
        public List<firmaDigital> ListadoFirmasDigitales()
        {
            List<firmaDigital> lista = new List<firmaDigital>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(cad_conex))
                {
                    SqlCommand cmd = new SqlCommand("ObtenerListadoFirmasDigitales", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new firmaDigital()
                            {
                                IdFirma = Convert.ToInt32(dr["id_firma"]),
                                RazonSocial = dr["razon_social"].ToString(),
                                otipofirma = new Tipofirma() { idtipofirma = Convert.ToInt32(dr["idtipofirma"]), descripcion = dr["tipo_firma"].ToString() },
                                RepresentanteLegal = dr["representante_legal"].ToString(),
                                EmpresaAcreditadora = dr["empresa_acreditadora"].ToString(),
                                FechaEmision = DateTime.Parse(dr["fecha_emision"].ToString()),
                                FechaVencimiento = DateTime.Parse(dr["fecha_vencimiento"].ToString())
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<firmaDigital>();
            }
            return lista;
        }



        //Registrar Firmas General
        public int registrar(firmaDigital obj,out string mensaje)
        {
            int idautogenerado = 0;

            mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(cad_conex))
                {
                    SqlCommand cmd = new SqlCommand("sp_registrar", oconexion);
                    cmd.Parameters.AddWithValue("@id_tipo_firma", obj.otipofirma.idtipofirma);
                    cmd.Parameters.AddWithValue("@razon_social", obj.RazonSocial);
                    cmd.Parameters.AddWithValue("@representante_legal", obj.RepresentanteLegal);
                    cmd.Parameters.AddWithValue("@empresa_acreditadora", obj.EmpresaAcreditadora);
                    cmd.Parameters.AddWithValue("@fecha_emision", obj.FechaEmision);
                    cmd.Parameters.AddWithValue("@fecha_vencimiento", obj.FechaVencimiento);
                    cmd.Parameters.Add("@resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    idautogenerado = Convert.ToInt32(cmd.Parameters["@resultado"].Value);
                    mensaje = cmd.Parameters["@mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {

                idautogenerado = 0;
                mensaje = ex.Message;
            }
            return idautogenerado;
        }


        //Editar Firmas General

        public bool editar(firmaDigital obj, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(cad_conex))
                {
                    SqlCommand cmd = new SqlCommand("sp_editar", oconexion);
                    cmd.Parameters.AddWithValue("@idfirma", obj.IdFirma);
                    cmd.Parameters.AddWithValue("@id_tipo_firma", obj.otipofirma.idtipofirma);
                    cmd.Parameters.AddWithValue("@razon_social", obj.RazonSocial);
                    cmd.Parameters.AddWithValue("@representante_legal", obj.RepresentanteLegal);
                    cmd.Parameters.AddWithValue("@empresa_acreditadora", obj.EmpresaAcreditadora);

                    cmd.Parameters.AddWithValue("@fecha_emision", obj.FechaEmision);
                    cmd.Parameters.AddWithValue("@fecha_vencimiento", obj.FechaVencimiento);


                    cmd.Parameters.Add("@resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    resultado = Convert.ToBoolean(cmd.Parameters["@resultado"].Value);
                    mensaje = cmd.Parameters["@mensaje"].Value.ToString();


                }
            }
            catch (Exception ex)
            {

                resultado = false;
                mensaje = ex.Message;
            }
            return resultado;
        }

        //Eliminar Firmas General
        public bool Eliminar(int id)
        {
            bool respuesta = true;
            using (SqlConnection oconexion = new SqlConnection(cad_conex))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(" update firmaDigital set eliminado =0 where id_firma= @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    cmd.ExecuteNonQuery();
                    respuesta = true;
                }
                catch (Exception ex)
                {
                    respuesta = false;
                }
            }
            return respuesta;

        }


        //Programacion de IMG
        public bool guardardatosimagen(firmaDigital obj, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(cad_conex))
                {
                    string jquery = "update firmaDigital set extensionruta =@extensionruta,ruta_rubrica=@ruta_rubrica where id_firma=@id_firma";
                    SqlCommand cmd = new SqlCommand(jquery, oconexion);
                    cmd.Parameters.AddWithValue("@extensionruta", obj.extensionruta);
                    cmd.Parameters.AddWithValue("@ruta_rubrica", obj.RutaRubrica);
                    cmd.Parameters.AddWithValue("@id_firma", obj.IdFirma);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }
                    else
                    {
                        mensaje = "No se pudo actualizar la imagen";
                    }

                }
            }
            catch (Exception ex)
            {

                resultado = false;
                mensaje = ex.Message;
            }
            return resultado;
        }

        //Para Archivos
        public bool guardardatosArchivos(firmaDigital obj, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(cad_conex))
                {
                    string jquery = "update firmaDigital set extensioncertificado =@extensioncertificado,certificado_digital=@certificado_digital   where id_firma=@id_firma";
                    SqlCommand cmd = new SqlCommand(jquery, oconexion);
                    cmd.Parameters.AddWithValue("@extensioncertificado", obj.extensioncertificado);
                    cmd.Parameters.AddWithValue("@certificado_digital", obj.CertificadoDigital);
                    cmd.Parameters.AddWithValue("@id_firma", obj.IdFirma);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }
                    else
                    {
                        mensaje = "No se pudo actualizar  el archivo";
                    }

                }
            }
            catch (Exception ex)
            {

                resultado = false;
                mensaje = ex.Message;
            }
            return resultado;
        }

    }
}
