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
            EdUsuario.SelectedIndex = -1;
            NivelUrgenciaPicker.SelectedIndex = -1;
            DtSolicitacao.Text = DateTime.Now.ToString("dd/MM/yyyy");
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

    private void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        _solicitacao.Solicitante = (int)EdUsuario.SelectedIndex + 1;
        _solicitacao.DataSolicitacao = DateTime.ParseExact(DtSolicitacao.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        _solicitacao.NivelUrgencia = (int)NivelUrgenciaPicker.SelectedIndex + 1;


        var registroExistente = _con.Find<Solicitacao>(_solicitacao.Id);
        if (registroExistente != null)
        {
            _con.Update(_solicitacao);
        }
        else
        {
            _con.Insert(_solicitacao);
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
}