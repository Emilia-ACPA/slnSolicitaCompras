using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SQLite;

namespace prjSolicitaCompras
{
    [SQLite.Table("Solicitacao")]
    public class Solicitacao
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Required]
        [ForeignKey("Usuario")]
        public int IdSolicitante { get; set; }
        [Required]
        public DateTime DataSolicitacao { get; set; }
        [Required]
        public int NivelUrgencia { get; set; }

        [Ignore]
        public string NomeSolicitante { get; set; } = string.Empty;
        [Ignore]
        public string StrNivelUrgencia { get; set; } = string.Empty;

        [Ignore]
        public List<ItemSolicitacao> ItensSolicitacao { get; set; } = new List<ItemSolicitacao>();

        public Solicitacao()
        {
        }

        public void CarregarExternos(SQLiteConnection con)
        {
            //Nome do solicitante
            var solicitanteUsuario = con.Table<Usuario>().FirstOrDefault(u => u.Id == IdSolicitante);
            if (solicitanteUsuario != null)
            {
                NomeSolicitante = solicitanteUsuario.NomeUsuario;
            }

            //Nível de urgência
            var StrNiveisUrgencia = new Dictionary<int, string>
            {
                { 1, "Baixa" },
                { 2, "Médio" },
                { 3, "Alta" }
            };
            if (StrNiveisUrgencia.TryGetValue(NivelUrgencia, out var strNivelUrgencia))
            {
                StrNivelUrgencia = strNivelUrgencia;
            }
        }

        public void CarregarItens(SQLiteConnection con)
        {
            //Carregar Itens
            ItensSolicitacao = con.Table<ItemSolicitacao>().Where(i => i.IdSolicitacao == Id).ToList();
            foreach (var item in ItensSolicitacao)
            {
                // Descrição do item
                var DescricaoItem = con.Table<Item>().FirstOrDefault(i => i.Id == item.IdItem);
                if (DescricaoItem != null)
                {
                    item.DescricaoItem = DescricaoItem.Descricao;
                }

                // Unidade de medida
                var unidadeMedida = con.Table<UnidadeMedida>().FirstOrDefault(u => u.Id == item.IdUnidadeMedida);
                if (unidadeMedida != null)
                {
                    item.UnidadeMedida = unidadeMedida.Descricao;
                }
            }
        }
    }
}
