using SQLite;

namespace prjSolicitaCompras;

public partial class UnidadesMedida : ContentPage
{
    private readonly SQLiteConnection _con;
    private readonly UnidadeMedida _unidadeMedida;
    public UnidadesMedida(SQLite.SQLiteConnection con, UnidadeMedida unidadeMedida)
	{
		InitializeComponent();
        _con = con;
        _unidadeMedida = unidadeMedida;

        CarregarUnidadeMedida(unidadeMedida);
    }

    //Carrega os dados de unidadeMedida diretamente do banco de dados para a tela de edi��o.
    private void CarregarUnidadeMedida(UnidadeMedida unidadeMedida)
    {
        try
        {
            if (unidadeMedida.Id != 0)
            {
                edCodigo.Text = unidadeMedida.Id.ToString();
                edDescricao.Text = unidadeMedida.Descricao;
            }
        }
        catch (SQLiteException e)
        {
            DisplayAlert("Erro", "Erro ao carregar os dados: " + e.Message, "OK");
        }
    }

    //Salva os dados da unidadeMedida no banco de dados.
    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        //Verifica se todos os campos est�o preenchidos.
        if (edDescricao.Text == "")
        {
            await DisplayAlert("Erro", "Favor preencher o campo 'Descri��o'' e tente novamente.", "OK");
            return;
        }

        //Novo Usu�rio, salva e volta para a tela de lista de usu�rios.
        if (_unidadeMedida.Id == 0)
        {
            _unidadeMedida.Descricao = edDescricao.Text;
            _con.Insert(_unidadeMedida);
            await DisplayAlert("Sucesso", "Unidade de Medida cadastrada com sucesso.", "OK");

            await Navigation.PopAsync();

        }
        //Usu�rio j� existente, salva e permanece na tela de edi��o.
        else
        {
            _unidadeMedida.Descricao = edDescricao.Text;
            _con.Update(_unidadeMedida);
            await DisplayAlert("Sucesso", "Unidade de Medida atualizada com sucesso.", "OK");
        }
    }

    //Cancela a edi��o e voltar para a tela de lista de usu�rios.
    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirma��o", "Deseja mesmo cancelar? Todas as altera��es feitas ser�o perdidas?",
            "Sim", "N�o"
        );

        if (confirmacao)
        {
            CarregarUnidadeMedida(_unidadeMedida);
        }
    }

    //Exclui o registro de unidadeMedida do banco de dados.
    private async void BtnExcluir_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirma��o", "Tem certeza de que deseja excluir esta Unidade de Medida?",
            "Sim", "N�o"
        );

        var registroExistente = _unidadeMedida.Id;
        if (registroExistente != null)
        {
            if (confirmacao)
            {
                _con.Delete(_unidadeMedida);
                await DisplayAlert("Sucesso", "Unidade de Medida exclu�da com sucesso.", "OK");

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