using SQLite;

namespace prjSolicitaCompras;

public partial class ListaUsuarios : ContentPage
{
    private readonly SQLiteConnection _con;
    
    //No construtor, inicializa a tela de lista de usu�rios e carrega os dados do banco de dados, armazeando em uma propriedade privada da classe.
    public ListaUsuarios(SQLiteConnection con)
	{
		InitializeComponent();
        _con = con;
        AtualizarListaUsuarios();
    }
    //Recarrega objeto Usu�rio, atualizando dados com altera��es sofridas.
    private void AtualizarListaUsuarios()
    {
        UsuariosListView.ItemsSource = _con.Table<Usuario>().ToList();
    }
    //Ao clicar no usu�rio, selecionando-o, abre a tela de edi��o do mesmo.
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var usuarioSelecionado = (Usuario)label.BindingContext;
        var usuarioPage = new Usuarios(_con, usuarioSelecionado);
        
        await Navigation.PushAsync(usuarioPage);
    }
    //Atualiza a lista de usu�rios com atualiza��es recentes no cadastro, toda vez que a tela for ativada(ou visualizada).
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

    //Imprime a lista de usu�rios, mas ainda n�o implementado.s
    private void BtnImprimir_Clicked(object sender, EventArgs e)
    {
        //Todo
    }

    //Volta para a tela anterior, que � a tela principal.
    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}