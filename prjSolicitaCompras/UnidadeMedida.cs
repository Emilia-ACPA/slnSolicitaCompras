using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("UnidadeMedida")]
    public class UnidadeMedida
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Descricao { get; set; } = string.Empty;

        public UnidadeMedida() { }
    }
}
