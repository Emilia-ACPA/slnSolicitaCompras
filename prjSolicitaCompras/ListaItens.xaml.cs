using System.Threading.Tasks;
using SQLite;

namespace prjSolicitaCompras;

public partial class ListaItens : ContentPage
{
    private readonly SQLiteConnection _con;
    public ListaItens(SQLiteConnection con)
	{
		InitializeComponent();
        _con = con;

        AtualizarListaRegistros();
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
        ItensListView.ItemsSource = _con.Table<Item>().ToList();
    }

    //Cria um novo registro, abrindo a tela de cadastro.
    private async void BtnNovo_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Itens(_con, new Item()));
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

    //Ao clicar no registro, selecionando-o, abre a tela de edição do mesmo.
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var RegistroSelecionado = (Item)label.BindingContext;
        var RegistroPage = new Itens(_con, RegistroSelecionado);

        await Navigation.PushAsync(RegistroPage);
    }
}