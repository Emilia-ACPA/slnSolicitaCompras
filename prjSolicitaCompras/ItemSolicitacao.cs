using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("ItemSolicitacao")]
    public class ItemSolicitacao
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Required]
        [ForeignKey("IdSolicitacao")]
        public int IdSolicitacao { get; set; }
        [ForeignKey("Item")]
        public int IdItem { get; set; }
        [ForeignKey("UnidadeMedida")]
        public int IdUnidadeMedida { get; set; }
        public decimal Quantidade { get; set; } = decimal.Zero;
        public decimal ValorUnitario { get; set; } = decimal.Zero;
        public decimal ValorTotal { get; set; } = decimal.Zero; //=> Quantidade * ValorUnitario;

        [Ignore]
        public string DescricaoItem { get; set; } = string.Empty;
        [Ignore]
        public string UnidadeMedida { get; set; } = string.Empty;

        public ItemSolicitacao() { }
    }
}
