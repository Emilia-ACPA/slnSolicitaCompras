using System.Globalization;
using SQLite;

namespace prjSolicitaCompras;

public partial class NovaSolicitacao : ContentPage
{
    private readonly SQLiteConnection _con;

    public NovaSolicitacao(SQLiteConnection con)
	{
		InitializeComponent();
        _con = con;
    }

    private void EdUsuario_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EdUsuario.Text))
        {
            EdUsuario.BackgroundColor = Colors.Red;
        }
        else
        {
            EdUsuario.BackgroundColor = Colors.Transparent;
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
        Solicitacao solicitacao = new Solicitacao();
        solicitacao.Solicitante = int.Parse(EdUsuario.Text);
        solicitacao.DataSolicitacao = DateTime.ParseExact(DtSolicitacao.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        solicitacao.NivelUrgencia = (int)NivelUrgenciaPicker.SelectedItem;
        _con.Insert(solicitacao);
    }
}