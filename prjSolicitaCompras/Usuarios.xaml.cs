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
        //Verifica se todos os campos est�o preenchidos.
        if (edNome.Text == "" || edSenha.Text == "")
        {
            await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
            return;
        }

        //Novo Usu�rio, salva e volta para a tela de lista de usu�rios.
        if (_usuario.Id == 0)
        {
            var usuarioExistente = _con.Table<Usuario>().FirstOrDefault(u => u.NomeUsuario == "nome_desejado");
            if (usuarioExistente != null)
            {
                // Registro encontrado
                await DisplayAlert("Erro", "Usu�rio j� existe.", "Ok");
                return;
            }
            _usuario.NomeUsuario = edNome.Text;
            _usuario.Senha = edSenha.Text;
            try
            {
                _con.Insert(_usuario);
                await DisplayAlert("Sucesso", "Usu�rio cadastrado com sucesso.", "OK");

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                // Tratamento de erro espec�fico do SQLite
                if (ex.Message.Contains("UNIQUE constraint failed"))
                {
                    await DisplayAlert("Erro", "J� existe um registro com este nome de usu�rio.", "OK");
                }
                else
                {
                    await DisplayAlert("Erro", "Erro desconhecido ao inserir registro: " + ex.Message, "OK");
                }
            }
        }
        //Usu�rio j� existente, salva e permanece na tela de edi��o.
        else
        {
            _usuario.NomeUsuario = edNome.Text;
            _usuario.Senha = edSenha.Text;
            try
            {
                _con.Update(_usuario);
                await DisplayAlert("Sucesso", "Usu�rio atualizado com sucesso.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro desconhecido ao salvar registro: " + ex.Message, "OK");
            }
        }
    }

    //Carrega os dados do usu�rio selecionado na tela de edi��o.
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
    //Carrega os dados originais antes das altera��es do usu�rio na tela de edi��o.
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

    //Exclui o usu�rio selecionado, ap�s confirma��o do usu�rio e volta para a tela de lista de usu�rios.
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

    //Volta para a tela de lista de usu�rios.
    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}