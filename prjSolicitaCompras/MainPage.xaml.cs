namespace prjSolicitaCompras
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void BtnNovaSolicitacao_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovaSolicitacao());
        }

        private async void BtnListaSolicitacoes_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaSolicitacoes());
        }
    }

}
