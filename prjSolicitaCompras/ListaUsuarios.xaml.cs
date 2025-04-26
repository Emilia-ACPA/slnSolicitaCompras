using SQLite;

namespace prjSolicitaCompras;

public partial class ListaUsuarios : ContentPage
{
    private readonly SQLiteConnection _con;
    //private object usuariosListView;
    public ListaUsuarios(SQLiteConnection con)
	{
		InitializeComponent();
        _con = con;
        AtualizarListaUsuarios();
    }

    private void AtualizarListaUsuarios()
    {
        UsuariosListView.ItemsSource = _con.Table<Usuario>().ToList();
    }
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var usuarioSelecionado = (Usuario)label.BindingContext;
        var usuarioPage = new Usuarios(_con, usuarioSelecionado);
        
        await Navigation.PushAsync(usuarioPage);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarListaUsuarios();
    }

}