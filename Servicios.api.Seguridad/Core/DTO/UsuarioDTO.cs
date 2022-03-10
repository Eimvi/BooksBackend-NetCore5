using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Seguridad.Core.DTO
{
    public class UsuarioDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Token { get; set; }
    }
}
