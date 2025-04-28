using SQLite;

namespace prjSolicitaCompras;

public partial class ListaUsuarios : ContentPage
{
    private readonly SQLiteConnection _con;
    
    //No construtor, inicializa a tela de lista de usuários e carrega os dados do banco de dados, armazeando em uma propriedade privada da classe.
    public ListaUsuarios(SQLiteConnection con)
	{
		InitializeComponent();
        _con = con;
        AtualizarListaUsuarios();
    }
    //Recarrega objeto Usuário, atualizando dados com alterações sofridas.
    private void AtualizarListaUsuarios()
    {
        UsuariosListView.ItemsSource = _con.Table<Usuario>().ToList();
    }
    //Ao clicar no usuário, selecionando-o, abre a tela de edição do mesmo.
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var usuarioSelecionado = (Usuario)label.BindingContext;
        var usuarioPage = new Usuarios(_con, usuarioSelecionado);
        
        await Navigation.PushAsync(usuarioPage);
    }
    //Atualiza a lista de usuários com atualizações recentes no cadastro, toda vez que a tela for ativada(ou visualizada).
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarListaUsuarios();
    }

    private async void BtnNovo_Clicked(object sender, EventArgs e)
    {
//        Usuarios usuarioPage = new Usuarios(_con, null);

        await Navigation.PushAsync(new Usuarios(_con, new Usuario()));
    }

    //Imprime a lista de usuários, mas ainda não implementado.s
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