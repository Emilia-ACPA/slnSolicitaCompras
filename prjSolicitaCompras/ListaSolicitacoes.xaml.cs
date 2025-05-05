using SQLite;

namespace prjSolicitaCompras;

public partial class ListaSolicitacoes : ContentPage
{
    private readonly SQLiteConnection _con;
    private object solicitacoesListView;

    public ListaSolicitacoes(SQLiteConnection con)
    {
        InitializeComponent();
        _con = con;
    }

    private void CarregaListaSolicitacoes()
    {
        var solicitacoes = _con.Table<Solicitacao>().ToList();

        foreach (var solicitacao in solicitacoes)
        {
            solicitacao.CarregarExternos(_con);
            solicitacao.CarregarItens(_con);
        }

        RegistrosListView.ItemsSource = solicitacoes;
    }

    private async void OnLabelTapped(object sender, EventArgs e)
    {
        var label = (Label)sender;
        var solicitacao = (Solicitacao)label.BindingContext;
        var solicitacaoPage = new NovaSolicitacao(_con, solicitacao);

        await Navigation.PushAsync(solicitacaoPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CarregaListaSolicitacoes();
    }

    // Sincroniza o ScrollView dos dados com o ScrollView do cabeçalho
    private void CabecalhoScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        DadosScrollView.ScrollToAsync(e.ScrollX, 0, false);
    }

    // Sincroniza o ScrollView do cabeçalho com o ScrollView dos dados
    private void DadosScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        CabecalhoScrollView.ScrollToAsync(e.ScrollX, 0, false);
    }

    private async void BtnNovo_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NovaSolicitacao(_con, new Solicitacao()));
    }

    private void BtnImprimir_Clicked(object sender, EventArgs e)
    {
        //Todo
    }

    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}