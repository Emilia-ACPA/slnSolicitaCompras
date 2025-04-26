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
    }

    private void CarregaListaSolicitacoes()
    {
        var solicitacoes = _con.Table<Solicitacao>().ToList();

        foreach (var solicitacao in solicitacoes)
        {
            solicitacao.CarregarExternos(_con);
            solicitacao.CarregarItens(_con);
        }

        SolicitacoesListView.ItemsSource = solicitacoes;
    }

    private async void OnLabelTapped(object sender, EventArgs e)
    {
        var label = (Label)sender;
        var solicitacao = (Solicitacao)label.BindingContext;
        var solicitacaoPage = new NovaSolicitacao(_con, solicitacao);

        await Navigation.PushAsync(solicitacaoPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CarregaListaSolicitacoes();
    }

    //public async Task NavigateToListaSolicitacoesAsync()  Todo
    //{
    //    await Shell.Current.GoToAsync("ListaSolicitacoes");
    //}

}