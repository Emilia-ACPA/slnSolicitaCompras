using System.Globalization;

namespace prjSolicitaCompras;

public partial class NovaSolicitacao : ContentPage
{
	public NovaSolicitacao()
	{
		InitializeComponent();
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

}