using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace prjSolicitaCompras;

public partial class ListaSolicitacoes : ContentPage
{
    private readonly SQLiteConnection _con;
    private object solicitacoesListView;

    public ListaSolicitacoes(SQLiteConnection con)
    {
        InitializeComponent();
        _con = con;
        LoadSolicitacoes();
    }

    private void LoadSolicitacoes()
    {
        // Adjusted to handle the 'new()' constraint issue by creating a default constructor for 'Solicitacao'.
        var solicitacoes = _con.Table<Solicitacao>().ToList();

        var usuarios = _con.Table<Usuario>().ToDictionary(u => u.Id, u => u.NomeUsuario);
        foreach (var solicitacao in solicitacoes)
        {
            if (usuarios.TryGetValue(solicitacao.Solicitante, out var nomeUsuario))
            {
                solicitacao.NomeSolicitante = nomeUsuario;
            }
        }

        var StrNiveisUrgencia = new Dictionary<int, string>
        {
            { 1, "Baixa" },
            { 2, "Média" },
            { 3, "Alta" }
        };
        foreach (var solicitacao in solicitacoes)
        {
            if (StrNiveisUrgencia.TryGetValue(solicitacao.NivelUrgencia, out var strNivelUrgencia))
            {
                solicitacao.StrNivelUrgencia = strNivelUrgencia;
            }
        }

        SolicitacoesListView.ItemsSource = solicitacoes;
    }
}