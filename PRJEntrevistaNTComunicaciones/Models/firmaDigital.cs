namespace PRJEntrevistaNTComunicaciones.Models
{
    public class firmaDigital
    {
        public int IdFirma { get; set; }
        public Tipofirma otipofirma { get; set; }
        public string RazonSocial { get; set; }
        public string RepresentanteLegal { get; set; }
        public string EmpresaAcreditadora { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string extensionruta { get; set; }   
        public string RutaRubrica { get; set; }
        public string extensioncertificado { get; set; }
        public string CertificadoDigital { get; set; }
        public DateTime FechaDeRegistro { get; set; }
        public bool Eliminado { get; set; }

       

        public string base64 { get; set; }
        public string extension { get; set; }
    }
}
