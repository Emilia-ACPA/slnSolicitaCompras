using SQLite;

namespace prjSolicitaCompras
{
    public partial class MainPage : ContentPage
    {
        string dbPath;
        SQLiteConnection con;

        public MainPage()
        {
            InitializeComponent();
            dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Compras.db3");
            con = new SQLiteConnection(dbPath);
            con.Execute("PRAGMA foreign_keys = ON;");
            con.CreateTable<Usuario>();
            con.CreateTable<Item>();
            con.CreateTable<UnidadeMedida>();
            con.CreateTable<Solicitacao>();
            con.CreateTable<ItemSolicitacao>();

            Console.WriteLine($"Caminho do banco de dados: {dbPath}");

        }

        private async void BtnNovaSolicitacao_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new NovaSolicitacao(con, new Solicitacao()));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao carregar os dados" + ex.Message, "OK");
            }
        }

        private async void BtnListaSolicitacoes_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ListaSolicitacoes(con));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao carregar os dados" + ex.Message, "OK");
            }
        }

        private async void BtnUsuarios_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ListaUsuarios(con));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao carregar os dados" + ex.Message, "OK");
            }
        }

        private async void BtnUnidadesMedida_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ListaUnidadesMedida(con));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao carregar os dados" + ex.Message, "OK");
            }
        }

        private async void BtnItens_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ListaItens(con));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro ao carregar os dados" + ex.Message, "OK");
            }
        }

        private void BtnLimparItensBase_Clicked(object sender, EventArgs e)
        {
            List<ItemSolicitacao> ItensSolicitacao = con.Table<ItemSolicitacao>().ToList();
            var ItenssolicitacaoCount = ItensSolicitacao.Count;
            con.Execute("delete from ItensSolicitacao");
        }
    }
}
