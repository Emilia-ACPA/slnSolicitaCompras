using SQLite;

namespace prjSolicitaCompras;

public partial class ListaUnidadesMedida : ContentPage
{
    private readonly SQLiteConnection _con;

    public ListaUnidadesMedida(SQLiteConnection con)
    {
        InitializeComponent();
        _con = con;
        AtualizarListaRegistros();
    }

    //Ao clicar no registro, selecionando-o, abre a tela de edi��o do mesmo.
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var RegistroSelecionado = (UnidadeMedida)label.BindingContext;
        var RegistroPage = new UnidadesMedida(_con, RegistroSelecionado);

        await Navigation.PushAsync(RegistroPage);
    }

    //Atualiza a lista de registros com atualiza��es recentes no cadastro, toda vez que a tela for ativada(ou visualizada).
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarListaRegistros();
    }

    //Recarrega objeto de persist�ncia, atualizando dados com altera��es sofridas.
    private void AtualizarListaRegistros()
    {
        RegistrosListView.ItemsSource = _con.Table<UnidadeMedida>().ToList();
    }

    //Cria um novo registro, abrindo a tela de cadastro.
    private async void BtnNovo_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UnidadesMedida(_con, new UnidadeMedida()));
    }

    //Imprime a lista de registros, mas ainda n�o implementado.
    private void BtnImprimir_Clicked(object sender, EventArgs e)
    {
        //Todo
    }

    //Volta para a tela anterior, que � a tela principal.
    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Sincroniza o ScrollView dos dados com o ScrollView do cabe�alho
    private void CabecalhoScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        DadosScrollView.ScrollToAsync(e.ScrollX, 0, false);
    }

    // Sincroniza o ScrollView do cabe�alho com o ScrollView dos dados
    private void DadosScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        CabecalhoScrollView.ScrollToAsync(e.ScrollX, 0, false);
    }
}
