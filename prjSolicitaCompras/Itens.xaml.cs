using System.Globalization;
using SQLite;

namespace prjSolicitaCompras;

public partial class Itens : ContentPage
{
    private readonly SQLiteConnection _con;
    private readonly Item _item;
    public Itens(SQLiteConnection con, Item item)
    {
        InitializeComponent();
        _con = con;
        _item = item;

        CarregarItem(item);
    }

    //Carrega os dados do item selecionado na tela de edição.
    private void CarregarItem(Item item)
    {
        try
        {
            if (item.Id != 0)
            {
                edCodigo.Text = item.Id.ToString();
                edDescricao.Text = item.Descricao;
                edValorUnitario.Text = (item.ValorUnitario).ToString("F2", CultureInfo.CurrentCulture);
            }
        }
        catch (SQLiteException e)
        {
            DisplayAlert("Erro", "Erro ao carregar os dados: " + e.Message, "OK");
        }
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        //Verifica se todos os campos estão preenchidos.
        if (edDescricao.Text == "" || edValorUnitario.Text == "")
        {
            await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
            return;
        }

        //Novo Item, salva e volta para a tela de lista de usuários.
        if (_item.Id == 0)
        {
            _item.Descricao = edDescricao.Text;
            _item.ValorUnitario = decimal.Parse(edValorUnitario.Text);
            _con.Insert(_item);
            await DisplayAlert("Sucesso", "Item cadastrado com sucesso.", "OK");

            await Navigation.PopAsync();

        }
        //Item já existente, salva e permanece na tela de edição.
        else
        {
            _item.Descricao = edDescricao.Text;
            _item.ValorUnitario = decimal.Parse(edValorUnitario.Text);
            _con.Update(_item);
            await DisplayAlert("Sucesso", "Item atualizado com sucesso.", "OK");
        }
    }

    //Cancela a edição e volta para a tela de lista de usuários.
    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirmação", "Deseja mesmo cancelar? Todas as alterações feitas serão perdidas?",
            "Sim", "Não"
        );

        if (confirmacao)
        {
            CarregarItem(_item);
        }
    }

    //Exclui o item selecionado e volta para a tela de lista de usuários.
    private async void BtnExcluir_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirmação", "Tem certeza de que deseja excluir este Item?",
            "Sim", "Não"
        );

        var registroExistente = _item.Id;
        if (registroExistente != null)
        {
            if (confirmacao)
            {
                _con.Delete(_item);
                await DisplayAlert("Sucesso", "Item excluído com sucesso.", "OK");

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