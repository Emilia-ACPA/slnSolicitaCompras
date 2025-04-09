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
            con.CreateTable<Usuario>();
        }

        private async void BtnNovaSolicitacao_Clicked(object sender, EventArgs e)
        {
            con.CreateTable<Solicitacao>();
            await Navigation.PushAsync(new NovaSolicitacao(con, new Solicitacao()));
        }

        private async void BtnListaSolicitacoes_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaSolicitacoes(con));
        }
    }

}
