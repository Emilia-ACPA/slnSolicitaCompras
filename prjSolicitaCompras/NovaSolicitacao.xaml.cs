using System.Globalization;
using System.Linq;
using SQLite;
using static System.Net.Mime.MediaTypeNames;

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
        con.CreateTable<ItemSolicitacao>();

        try
        {
            LoadUsuarios();

            if (_solicitacao.Id == 0)
            {
                AddNovaSolicitacao(_solicitacao);
                CarregarCabecalhoItens();
                NovoItemSolicitacao(_solicitacao.ItensSolicitacao);
            }
            else
            {
                LoadSolicitacao(_solicitacao);
                CarregarItensSolicitacao(_solicitacao.Id);
                NovoItemSolicitacao(_solicitacao.ItensSolicitacao);
            }
        }
        catch (SQLiteException e)
        {
            DisplayAlert("Erro", "Erro ao carregar os dados: " + e.Message, "OK");
        }
    }

    private void NovoItemSolicitacao(List<ItemSolicitacao> itens)
    {
        gridItensSolicitacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        int linhaAtual = itens.Count + 1;

        // Código
        var edCodigo = new Entry
        {
            MaximumWidthRequest = 80
        };
        Grid.SetRow(edCodigo, linhaAtual );
        Grid.SetColumn(edCodigo, 0);
        this.gridItensSolicitacao.Children.Add(edCodigo);

        // Descrição
        var edDescricao = new Picker
        {
            MaximumWidthRequest = 800
        };
        Grid.SetRow(edDescricao, linhaAtual);
        Grid.SetColumn(edDescricao, 1);
        gridItensSolicitacao.Children.Add(edDescricao);

        // Unidade de Medida
        var edUnidadeMedida = new Picker
        {
            MaximumWidthRequest = 50
        };
        Grid.SetRow(edUnidadeMedida, linhaAtual);
        Grid.SetColumn(edUnidadeMedida, 1);
        gridItensSolicitacao.Children.Add(edUnidadeMedida);

        // Quantidade
        var edQuantidade = new Entry
        {
            Keyboard = Keyboard.Numeric,
            MaximumWidthRequest = 150
        };
        //edQuantidade.TextChanged += (sender, e) =>
        //{
        //    if (decimal.TryParse(e.NewTextValue, out var quantidade))
        //    {
        //        MaximumWidthRequest = 150;
        //    }
        //};
        Grid.SetRow(edQuantidade, linhaAtual);
        Grid.SetColumn(edQuantidade, 2);
        gridItensSolicitacao.Children.Add(edQuantidade);

        // Valor Unitário
        var edValorUnitario = new Entry
        {
//            Text = itens[linhaAtual].ValorUnitario.ToString("F2"),
            Keyboard = Keyboard.Numeric,
        };
        //edValorUnitario.TextChanged += (sender, e) =>
        //{
        //    if (decimal.TryParse(e.NewTextValue, out var valorUnitario))
        //    {
        //        itens[linhaAtual].ValorUnitario = valorUnitario;
        //    }
        //};
        Grid.SetRow(edValorUnitario, linhaAtual);
        Grid.SetColumn(edValorUnitario, 3);
        gridItensSolicitacao.Children.Add(edValorUnitario);

        // Valor Total
        var lbCalcValorTotal = new Label
        {
//            Text = itens[linhaAtual].ValorTotal.ToString("F2"),
        };
        Grid.SetRow(lbCalcValorTotal, linhaAtual);
        Grid.SetColumn(lbCalcValorTotal, 4);
        gridItensSolicitacao.Children.Add(lbCalcValorTotal);

        // Botão AddItem
        var btAddItem = new Button
        {
//            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
//            IsVisible = true,
        };
        Grid.SetRow(btAddItem, linhaAtual);
        Grid.SetColumn(btAddItem, 5);
        btAddItem.HeightRequest = 50;
        btAddItem.WidthRequest = 50;
        btAddItem.ImageSource = "adicionaritem.png";
        btAddItem.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                AdicionarItemSolicitacao(linhaAtual);
            })
        });
        gridItensSolicitacao.Children.Add(btAddItem);
    }

    private void CarregarCabecalhoItens()
    {
        gridItensSolicitacao.Children.Clear();
        gridItensSolicitacao.RowDefinitions.Clear();
        gridItensSolicitacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Cabeçalho do Grid de Itens
        var lbCodigo = new Label
        {
            Text = "Código",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 80
        };
        Grid.SetRow(lbCodigo, 0);
        Grid.SetColumn(lbCodigo, 0);
        this.gridItensSolicitacao.Children.Add(lbCodigo);

        var lbDescricao = new Label
        {
            Text = "Descrição",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 800
        };
        Grid.SetRow(lbDescricao, 0);
        Grid.SetColumn(lbDescricao, 1);
        this.gridItensSolicitacao.Children.Add(lbDescricao);

        var lbUnidadeMedida = new Label
        {
            Text = "Und Medida",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 50
        };
        Grid.SetRow(lbUnidadeMedida, 0);
        Grid.SetColumn(lbUnidadeMedida, 1);
        this.gridItensSolicitacao.Children.Add(lbUnidadeMedida);

        var lbQuantidade = new Label
        {
            Text = "Quantidade",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 150
        };
        Grid.SetRow(lbQuantidade, 0);
        Grid.SetColumn(lbQuantidade, 2);
        this.gridItensSolicitacao.Children.Add(lbQuantidade);

        var lbValorUnitario = new Label
        {
            Text = "Valor Unitário",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 150
        };
        Grid.SetRow(lbValorUnitario, 0);
        Grid.SetColumn(lbValorUnitario, 3);
        this.gridItensSolicitacao.Children.Add(lbValorUnitario);

        var lbValorTotal = new Label
        {
            Text = "Valor Total",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 150
        };
        Grid.SetRow(lbValorTotal, 0);
        Grid.SetColumn(lbValorTotal, 4);
        this.gridItensSolicitacao.Children.Add(lbValorTotal);

        var lbAddItem = new Label
        {
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 50
        };
        Grid.SetRow(lbAddItem, 0);
        Grid.SetColumn(lbAddItem, 5);
        this.gridItensSolicitacao.Children.Add(lbAddItem);
    }

    private void CarregarItensSolicitacao(int _idSolicitacao)
    {
        List<ItemSolicitacao> itens = _con.Table<ItemSolicitacao>()
            .Where(i => i.IdSolicitacao == _idSolicitacao)
            .ToList();

        _solicitacao.ItensSolicitacao = itens;

        // Limpa e carrega cabeçalho
        CarregarCabecalhoItens();

        // Itens da Solicitação
        for (int i = 0; i < itens.Count; i++)
        {
            var item = itens[i];

            // Adicionar uma nova linha para cada item
            gridItensSolicitacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Código
            var edCodigo = new Entry
            {
                Text = item.IdItem.ToString(),
                IsReadOnly = true,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(edCodigo, i + 1);
            Grid.SetColumn(edCodigo, 0);
            gridItensSolicitacao.Children.Add(edCodigo);

            // Descrição
            var edDescricao = new Picker
            {
                Title = "Selecione o item",
                ItemsSource = _con.Table<Item>().Select(i => i.Descricao).ToList(),
                SelectedItem = item.DescricaoItem,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            edDescricao.SelectedIndexChanged += Picker_SelectedIndexChanged;
            Grid.SetRow(edDescricao, i + 1);
            Grid.SetColumn(edDescricao, 1);
            gridItensSolicitacao.Children.Add(edDescricao);

            // Unidade de Medida
            var edUnidadeMedida = new Picker
            {
                Title = "Selecione a Unidade de Medida",
                ItemsSource = _con.Table<UnidadeMedida>().Select(u => u.Descricao).ToList(),
                SelectedItem = item.UnidadeMedida,
                IsEnabled = false,
                IsVisible = true,
                WidthRequest = 50,
                HeightRequest = 50,
            };
            Grid.SetRow(edUnidadeMedida, i + 1);
            Grid.SetColumn(edUnidadeMedida, 2);
            gridItensSolicitacao.Children.Add(edUnidadeMedida);

            // Quantidade
            var edQuantidade = new Entry
            {
                Text = item.Quantidade.ToString(),
                Keyboard = Keyboard.Numeric,
                HorizontalOptions = LayoutOptions.Center
            };
            edQuantidade.TextChanged += (sender, e) =>
            {
                if (decimal.TryParse(e.NewTextValue, out var quantidade))
                {
                    item.Quantidade = quantidade;
                }
            };
            Grid.SetRow(edQuantidade, i + 1);
            Grid.SetColumn(edQuantidade, 3);
            gridItensSolicitacao.Children.Add(edQuantidade);

            // Valor Unitário
            var edValorUnitario = new Entry
            {
                Text = item.ValorUnitario.ToString("F2"),
                Keyboard = Keyboard.Numeric,
                HorizontalOptions = LayoutOptions.Center
            };
            edValorUnitario.TextChanged += (sender, e) =>
            {
                if (decimal.TryParse(e.NewTextValue, out var valorUnitario))
                {
                    item.ValorUnitario = valorUnitario;
                }
            };
            Grid.SetRow(edValorUnitario, i + 1);
            Grid.SetColumn(edValorUnitario, 4);
            gridItensSolicitacao.Children.Add(edValorUnitario);

            // Valor Total
            var lbCalcValorTotal = new Label
            {
                Text = item.ValorTotal.ToString("F2"),
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(lbCalcValorTotal, i + 1);
            Grid.SetColumn(lbCalcValorTotal, 5);
            gridItensSolicitacao.Children.Add(lbCalcValorTotal);
        }
    }

    private void Picker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is Picker descricaoPicker && descricaoPicker.BindingContext is ItemSolicitacao item)
        {
            string valorSelecionado = descricaoPicker.SelectedItem?.ToString();
            item.DescricaoItem = valorSelecionado;
        }
    }

    private void AdicionarItemSolicitacao(int indexLinha)
    {
        ItemSolicitacao _itemSolicitacao = new ItemSolicitacao();

        _itemSolicitacao.IdSolicitacao = _solicitacao.Id;

//        var pickerDescricao = (Picker)gridItensSolicitacao.Children
        //    .FirstOrDefault(c => (Picker)Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 1);

        //if (pickerDescricao != null)
        //{
        //    var descricaoSelecionada = pickerDescricao.SelectedItem?.ToString();

        //}

        //_itemSolicitacao.IdItem = pickerDescricao.SelectedIndex;
//        _itemSolicitacao.UnidadeMedida = pickerUnidadeMedida.SelectedIndex;
        _itemSolicitacao.Quantidade = decimal.Parse(((Entry)gridItensSolicitacao.Children[indexLinha]).Text);
        _itemSolicitacao.ValorUnitario = decimal.Parse(((Entry)gridItensSolicitacao.Children[indexLinha]).Text);
        _itemSolicitacao.ValorTotal = _itemSolicitacao.Quantidade * _itemSolicitacao.ValorUnitario;

        _con.Insert(_itemSolicitacao);
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
        _solicitacao.IdSolicitante = (int)EdUsuario.SelectedIndex + 1;
        _solicitacao.DataSolicitacao = DateTime.ParseExact(DtSolicitacao.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        _solicitacao.NivelUrgencia = (int)NivelUrgenciaPicker.SelectedIndex + 1;

        if (_solicitacao.IdSolicitante == 0 || _solicitacao.NivelUrgencia == 0 || _solicitacao.DataSolicitacao == DateTime.MinValue)
        {
            await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
            return;
        }

        var registroExistente = _con.Find<Solicitacao>(_solicitacao.Id);
        if (registroExistente != null)
        {
            bool confirmacao = await DisplayAlert(
                "Confirmação", "Qualquer alteração feita será registrada. Deseja salvar?",
                "Sim", "Não"
            );

            if (confirmacao) {
                _con.Update(_solicitacao);
                await DisplayAlert("Sucesso", "Solicitação salva com sucesso.", "OK");
            }
        }
        else
        {
            _con.Insert(_solicitacao);
            await DisplayAlert("Sucesso", "Nova solicitação inserida com sucesso.", "OK");
            AddNovaSolicitacao(_solicitacao);
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

    private void AddNovaSolicitacao(Solicitacao solicitacao)
    {
        EdUsuario.SelectedIndex = -1;
        NivelUrgenciaPicker.SelectedIndex = -1;
        DtSolicitacao.Text = DateTime.Now.ToString("dd/MM/yyyy");
    }

    private async void BtnExcluir_Clicked(object sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Confirmação", "Tem certeza de que deseja excluir esta solicitação?",
            "Sim", "Não"
        );

        var registroExistente = _con.Find<Solicitacao>(_solicitacao.Id);
        if (registroExistente != null)
        {
            if (confirmacao)
            {
                _con.Delete(_solicitacao);
                await DisplayAlert("Sucesso", "Solicitação excluída com sucesso.", "OK");
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
            "Confirmação", "Todas as alterações feitas serão perdidas?",
            "Sim", "Não"
        );

        if (confirmacao)
        {
            LoadSolicitacao(_solicitacao);
            CarregarItensSolicitacao(_solicitacao.Id);
            NovoItemSolicitacao(_solicitacao.ItensSolicitacao);
        }
    }
}