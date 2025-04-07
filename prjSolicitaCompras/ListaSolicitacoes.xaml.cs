using SQLite;

namespace prjSolicitaCompras;

public partial class ListaSolicitacoes : ContentPage
{
    private readonly SQLiteConnection _con;
    public ListaSolicitacoes(SQLiteConnection con)
	{
		InitializeComponent();
		_con = con;
	}
}