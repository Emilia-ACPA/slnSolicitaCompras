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
        //Verifica se todos os campos estão preenchidos.
        if (edNome.Text == "" || edSenha.Text == "")
        {
            await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
            return;
        }

        //Novo Usuário, salva e volta para a tela de lista de usuários.
        if (_usuario.Id == 0)
        {
            _usuario.NomeUsuario = edNome.Text;
            _usuario.Senha = edSenha.Text;
            _con.Insert(_usuario);
            await DisplayAlert("Sucesso", "Usuário cadastrado com sucesso.", "OK");

            await Navigation.PopAsync();

        }
        //Usuário já existente, salva e permanece na tela de edição.
        else
        {
            _usuario.NomeUsuario = edNome.Text;
            _usuario.Senha = edSenha.Text;
            _con.Update(_usuario);
            await DisplayAlert("Sucesso", "Usuário atualizado com sucesso.", "OK");
        }
    }

    //Carrega os dados do usuário selecionado na tela de edição.
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
    //Carrega os dados originais antes das alterações do usuário na tela de edição.
    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirmação", "Deseja mesmo cancelar? Todas as alterações feitas serão perdidas?",
            "Sim", "Não"
        );

        if (confirmacao)
        {
            CarregarUsuario(_usuario);
        }
    }

    //Exclui o usuário selecionado, após confirmação do usuário e volta para a tela de lista de usuários.
    private async void BtnExcluir_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirmação", "Tem certeza de que deseja excluir este Usuário?",
            "Sim", "Não"
        );

        var registroExistente = _usuario.Id;
        if (registroExistente != null)
        {
            if (confirmacao)
            {
                _con.Delete(_usuario);
                await DisplayAlert("Sucesso", "Usuário excluído com sucesso.", "OK");

                await Navigation.PopAsync();
            }
        }
    }

    //Volta para a tela de lista de usuários.
    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}