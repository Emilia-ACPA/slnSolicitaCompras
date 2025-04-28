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

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var RegistroSelecionado = (UnidadeMedida)label.BindingContext;
        var RegistroPage = new UnidadesMedida(_con, RegistroSelecionado);

        await Navigation.PushAsync(RegistroPage);
    }

    //Atualiza a lista de registros com atualizações recentes no cadastro, toda vez que a tela for ativada(ou visualizada).
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarListaRegistros();
    }

    //Recarrega objeto de persistência, atualizando dados com alterações sofridas.
    private void AtualizarListaRegistros()
    {
        RegistrosListView.ItemsSource = _con.Table<UnidadeMedida>().ToList();
    }

    //Cria um novo registro, abrindo a tela de cadastro.
    private async void BtnNovo_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UnidadesMedida(_con, new UnidadeMedida()));
    }

    //Imprime a lista de registros, mas ainda não implementado.
    private void BtnImprimir_Clicked(object sender, EventArgs e)
    {
        //Todo
    }

    //Volta para a tela anterior, que é a tela principal.
    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
