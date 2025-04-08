using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("Solicitacao")]
    public class Solicitacao
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Solicitante { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public int NivelUrgencia { get; set; }
        [Ignore]
        public string NomeSolicitante { get; set; } = string.Empty;
        [Ignore]
        public string StrNivelUrgencia { get; set; } = string.Empty;


        public Solicitacao() { }

    }
}
