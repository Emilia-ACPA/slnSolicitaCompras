using SQLite;

namespace prjSolicitaCompras
{
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorUnitario { get; set; } = decimal.Zero;
        public byte[] Imagem { get; set; } = Array.Empty<byte>();
    }


    //A ser usado para carregar a imagem. Armazenando imagem inicialmente no banco. Posteriormente, armazenagem na nuvem.

    //ATUALMENTE NO BANCO
    //var item = new Item
    //{
    //    Imagem = File.ReadAllBytes("C:/Users/User/Documents/ACPA Infos/Icons/Item1.png")
    //};

    //POSTERIORMENTE NA NUVEM - URL
    //var item = new Item
    //{
    //    UrlImagem = "https://meusite.com/imagens/item1.png"
    //};

    //CONVERSÃO PARA EXIBIR IMAGEM 
    //var imageSource = ImageSource.FromStream(() => new MemoryStream(item.Imagem));

}
