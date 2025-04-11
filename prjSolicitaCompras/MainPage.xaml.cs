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

            dbPath = "C:\\DSV\\Compras\\slnSolicitaCompras\\Compras.db3"; //System.IO.Path.Combine(FileSystem.AppDataDirectory, "Compras.db3");
            con = new SQLiteConnection(dbPath);
            con.Execute("PRAGMA foreign_keys = ON;");
            con.CreateTable<Usuario>();
            con.CreateTable<Item>();
            con.CreateTable<UnidadeMedida>();
            con.CreateTable<Solicitacao>();
        }

        private async void BtnNovaSolicitacao_Clicked(object sender, EventArgs e)
        {
            try {
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
    }

}
