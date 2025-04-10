using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("ItemSolicitacao")]
    public partial class ItemSolicitacao
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Required]
        public int IdSolicitacao { get; set; }
        public int IdItem { get; set; }
        public int IdUnidadeMedida { get; set; }
        public decimal Quantidade { get; set; } = decimal.Zero;
        public decimal ValorUnitario { get; set; } = decimal.Zero;
        public decimal ValorTotal => Quantidade * ValorUnitario;    
        [Ignore]
        public string DescricaoItem { get; set; } = string.Empty;
        [Ignore]
        public string UnidadeMedida { get; set; }
    }
}
