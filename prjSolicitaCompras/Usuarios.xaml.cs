using SQLite;

namespace prjSolicitaCompras;

public partial class Usuarios : ContentPage
{
    private readonly SQLiteConnection _con;
    private readonly Usuario _usuario;

    public Usuarios(SQLiteConnection con, Usuario usuario)
	{
        InitializeComponent();
        _con = con;
        _usuario = usuario;

        CarregarUsuario(usuario);
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        if (edNome.Text == "" || edSenha.Text == "")
        {
            await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
            return;
        }

        if (_usuario.Id == 0)
        {
            _usuario.NomeUsuario = edNome.Text;
            _usuario.Senha = edSenha.Text;
            _con.Insert(_usuario);
            await DisplayAlert("Sucesso", "Usu�rio cadastrado com sucesso.", "OK");
        }
        else
        {
            _usuario.NomeUsuario = edNome.Text;
            _usuario.Senha = edSenha.Text;
            _con.Update(_usuario);
            await DisplayAlert("Sucesso", "Usu�rio atualizado com sucesso.", "OK");
        }
    }

    private async void CarregarUsuario(Usuario usuario)
    {
        try
        {
            if (usuario.Id != 0)
            {
                edCodigo.Text = usuario.Id.ToString();
                edNome.Text = usuario.NomeUsuario;
                edSenha.Text = usuario.Senha;
            }
        }
        catch (SQLiteException e)
        {
            DisplayAlert("Erro", "Erro ao carregar os dados: " + e.Message, "OK");
        }
    }
    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirma��o", "Deseja mesmo cancelar? Todas as altera��es feitas ser�o perdidas?",
            "Sim", "N�o"
        );

        if (confirmacao)
        {
            CarregarUsuario(_usuario);
        }
    }

    private async void BtnExcluir_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirma��o", "Tem certeza de que deseja excluir este Usu�rio?",
            "Sim", "N�o"
        );

        var registroExistente = _usuario.Id;
        if (registroExistente != null)
        {
            if (confirmacao)
            {
                _con.Delete(_usuario);
                await DisplayAlert("Sucesso", "Usu�rio exclu�do com sucesso.", "OK");
                await Navigation.PopAsync();
            }
        }

    }

    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}