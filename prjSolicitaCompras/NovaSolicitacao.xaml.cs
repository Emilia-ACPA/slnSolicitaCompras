using System.Globalization;
using SQLite;

namespace prjSolicitaCompras;

public partial class NovaSolicitacao : ContentPage
{
    private readonly SQLiteConnection _con;
    private readonly Solicitacao _solicitacao;

    public NovaSolicitacao(SQLiteConnection con, Solicitacao solicitacao)
    {
        InitializeComponent();
        _con = con;
        _solicitacao = solicitacao;
        LoadUsuarios();

        if (_solicitacao.Id == 0)
        {
            LimparSolicitacao(_solicitacao);
        }
        else
        {
            LoadSolicitacao(_solicitacao);
        }
    }

    private void DtSolicitacao_TextChanged(object sender, TextChangedEventArgs e)
    {
        string dateFormat = "dd/MM/yyyy";
        if (DateTime.TryParseExact(DtSolicitacao.Text, dateFormat, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
        {
            DtSolicitacao.BackgroundColor = Colors.Transparent;
            DtSolicitacao.Text = parsedDate.ToString(dateFormat);
        }
        else
        {
            DtSolicitacao.BackgroundColor = Colors.Red;
        }
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        _solicitacao.Solicitante = (int)EdUsuario.SelectedIndex + 1;
        _solicitacao.DataSolicitacao = DateTime.ParseExact(DtSolicitacao.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        _solicitacao.NivelUrgencia = (int)NivelUrgenciaPicker.SelectedIndex + 1;

        if (_solicitacao.Solicitante == 0 || _solicitacao.NivelUrgencia == 0 || _solicitacao.DataSolicitacao == DateTime.MinValue)
        {
            await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
            return;
        }

        var registroExistente = _con.Find<Solicitacao>(_solicitacao.Id);
        if (registroExistente != null)
        {
            bool confirmacao = await DisplayAlert(
                "Confirma��o", "Qualquer altera��o feita ser� registrada. Deseja salvar?",
                "Sim", "N�o"
            );

            if (confirmacao) {
                _con.Update(_solicitacao);
                await DisplayAlert("Sucesso", "Solicita��o salva com sucesso.", "OK");
            }
        }
        else
        {
            _con.Insert(_solicitacao);
            await DisplayAlert("Sucesso", "Nova solicita��o inserida com sucesso.", "OK");
        }
    }

  
    private void LoadUsuarios()
    {
        var usuarios = _con.Table<Usuario>().ToList();
        EdUsuario.ItemsSource = usuarios.Select(u => u.NomeUsuario).ToList();
    }

    private void LoadSolicitacao(Solicitacao solicitacao)
    {
        EdUsuario.SelectedItem = solicitacao.NomeSolicitante;
        DtSolicitacao.Text = solicitacao.DataSolicitacao.ToString("dd/MM/yyyy");
        NivelUrgenciaPicker.SelectedIndex = solicitacao.NivelUrgencia-1;
    }

    private void LimparSolicitacao(Solicitacao solicitacao)
    {
        EdUsuario.SelectedIndex = -1;
        NivelUrgenciaPicker.SelectedIndex = -1;
        DtSolicitacao.Text = DateTime.Now.ToString("dd/MM/yyyy");
    }

    private async void BtnExcluir_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirma��o", "Tem certeza de que deseja excluir esta solicita��o?",
            "Sim", "N�o"
        );

        var registroExistente = _con.Find<Solicitacao>(_solicitacao.Id);
        if (registroExistente != null)
        {
            if (confirmacao)
            {
                _con.Delete(_solicitacao);
                await DisplayAlert("Sucesso", "Solicita��o exclu�da com sucesso.", "OK");
                await Navigation.PopAsync();
            }
        }

    }

    private async void BtnVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirma��o", "Todas as altera��es feitas ser�o perdidas?",
            "Sim", "N�o"
        );

        if (confirmacao)
        {
            LoadSolicitacao(_solicitacao);
        }
    }
}