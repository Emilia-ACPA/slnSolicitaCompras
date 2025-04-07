using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("Usuario")]
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
    }
}
