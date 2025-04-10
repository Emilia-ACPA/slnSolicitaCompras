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
    [SQLite.Table("Solicitacao")]
    public class Solicitacao
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Required]
        public int Solicitante { get; set; }
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

        public Solicitacao() { }

        public void CarregarItensSolicitacao(SQLiteConnection con)
        {
            ItensSolicitacao = con.Table<ItemSolicitacao>().Where(i => i.IdSolicitacao == Id).ToList();
            //foreach (var item in ItensSolicitacao)
            //{
            //    var unidadeMedida = con.Table<UnidadeMedida>().FirstOrDefault(u => u.Id == item.IdUnidadeMedida);
            //    if (unidadeMedida != null)
            //    {
            //        item.UnidadeMedida = unidadeMedida.Descricao;
            //    }
            //}
        }


        //CHAMADA PARA CARREGAMENTO DE ITENS a ser utilizada a qualquer manipulação de instância de Solicitacao no projeto
        //var solicitacao = connection.Find<Solicitacao>(idSolicitacao);
        //if (solicitacao != null)
        //{
        //    solicitacao.CarregarItens(connection);

        //    // Exemplo: Iterar sobre os itens carregados
        //    foreach (var item in solicitacao.Itens)
        //    {
        //        Console.WriteLine($"Item: {item.DescricaoItem}, Quantidade: {item.Quantidade}");
        //    }
    }




}
