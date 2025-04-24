using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls.Handlers;
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

        try
        {
            CarregarUsuarios();

            if (solicitacao.Id == 0)
            {
                //AddNovaSolicitacao(_solicitacao);
                //CarregarCabecalhoItens();
                //NovoItemSolicitacao(_solicitacao.ItensSolicitacao);
            }
            else
            {
                CarregarSolicitacao(solicitacao);
                CarregarItensSolicitacao(solicitacao);
                NovoItemSolicitacao(solicitacao);
            }
        }
        catch (SQLiteException e)
        {
            DisplayAlert("Erro", "Erro ao carregar os dados: " + e.Message, "OK");
        }
    }

    private void NovoItemSolicitacao(Solicitacao solicitacao)
    {
        List<ItemSolicitacao> itens = solicitacao.ItensSolicitacao;

        gridItensSolicitacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        int linhaAtual = itens.Count+1;

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
            MaximumWidthRequest = 800,
            ItemsSource = _con.Table<Item>().Select(i => i.Descricao).ToList(),

        };
        Grid.SetRow(edDescricao, linhaAtual);
        Grid.SetColumn(edDescricao, 1);
        this.gridItensSolicitacao.Children.Add(edDescricao);

        // Unidade de Medida
        var edUnidadeMedida = new Picker
        {
            MaximumWidthRequest = 100,
            ItemsSource = _con.Table<UnidadeMedida>().Select(i => i.Descricao).ToList(),
        };
        Grid.SetRow(edUnidadeMedida, linhaAtual);
        Grid.SetColumn(edUnidadeMedida, 2);
        this.gridItensSolicitacao.Children.Add(edUnidadeMedida);

        // Quantidade
        var edQuantidade = new Entry
        {
            Keyboard = Keyboard.Numeric,
            MaximumWidthRequest = 100
        };
        int maxTotalLength = 10;
        edQuantidade.TextChanged += (sender, e) =>
        {
            string novoTexto = e.NewTextValue;
            if (novoTexto.Length > maxTotalLength)
            {
                edQuantidade.Text = e.OldTextValue;
                return;
            }

            if (!(decimal.TryParse(e.NewTextValue, out var quantidade)))
            {
                DisplayAlert("Erro", "Valor inválido", "OK");
            }
        };
        Grid.SetRow(edQuantidade, linhaAtual);
        Grid.SetColumn(edQuantidade, 3);
        this.gridItensSolicitacao.Children.Add(edQuantidade);

        // Valor Unitário
        var edValorUnitario = new Entry
        {
            Keyboard = Keyboard.Numeric,
            MaximumWidthRequest = 100
        };
        edValorUnitario.TextChanged += (sender, e) =>
        {
            string novoTexto = e.NewTextValue;
            if (novoTexto.Length > maxTotalLength)
            {
                edValorUnitario.Text = e.OldTextValue;
                return;
            }

            if (!(decimal.TryParse(e.NewTextValue, out var valorUnitario)))
            {
                DisplayAlert("Erro", "Valor inválido", "OK");
            }
            else
            {
                if (edQuantidade.Text != null)
                {
                    var quantidade = decimal.Parse(edQuantidade.Text);
                    var valorTotal = valorUnitario * quantidade;
                    var lbCalcValorTotal = gridItensSolicitacao.Children.OfType<Label>()
                        .FirstOrDefault(c => Grid.GetRow(c) == linhaAtual && Grid.GetColumn(c) == 5);
                    if (lbCalcValorTotal.Text != null)
                    {
                        lbCalcValorTotal.Text = valorTotal.ToString("F2", CultureInfo.CurrentCulture);
                    }
                }
            }
        };
        Grid.SetRow(edValorUnitario, linhaAtual);
        Grid.SetColumn(edValorUnitario, 4);
        this.gridItensSolicitacao.Children.Add(edValorUnitario);

        // Valor Total
        var lbCalcValorTotal = new Label
        {
            MaximumWidthRequest = 100

        };
        Grid.SetRow(lbCalcValorTotal, linhaAtual);
        Grid.SetColumn(lbCalcValorTotal, 5);
        this.gridItensSolicitacao.Children.Add(lbCalcValorTotal);

        // Botão AddItem
        var btAddItem = new Button
        {
            HorizontalOptions = LayoutOptions.Center,
            MaximumWidthRequest = 50
        };
        Grid.SetRow(btAddItem, linhaAtual);
        Grid.SetColumn(btAddItem, 6);
        btAddItem.HeightRequest = 50;
        btAddItem.WidthRequest = 50;
        btAddItem.ImageSource = "adicionaritem.png";
        btAddItem.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                AdicionarItemSolicitacao(solicitacao.Id, linhaAtual);
            })
        });
        this.gridItensSolicitacao.Children.Add(btAddItem);
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
        Grid.SetColumn(lbUnidadeMedida, 2);
        this.gridItensSolicitacao.Children.Add(lbUnidadeMedida);

        var lbQuantidade = new Label
        {
            Text = "Quantidade",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 150
        };
        Grid.SetRow(lbQuantidade, 0);
        Grid.SetColumn(lbQuantidade, 3);
        this.gridItensSolicitacao.Children.Add(lbQuantidade);

        var lbValorUnitario = new Label
        {
            Text = "Valor Unitário",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 150
        };
        Grid.SetRow(lbValorUnitario, 0);
        Grid.SetColumn(lbValorUnitario, 4);
        this.gridItensSolicitacao.Children.Add(lbValorUnitario);

        var lbValorTotal = new Label
        {
            Text = "Valor Total",
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 150
        };
        Grid.SetRow(lbValorTotal, 0);
        Grid.SetColumn(lbValorTotal, 5);
        this.gridItensSolicitacao.Children.Add(lbValorTotal);

        var lbAddItem = new Label
        {
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 50
        };
        Grid.SetRow(lbAddItem, 0);
        Grid.SetColumn(lbAddItem, 6);
        this.gridItensSolicitacao.Children.Add(lbAddItem);
    }

    private void CarregarItensSolicitacao(Solicitacao solicitacao)
    {
        List<ItemSolicitacao> itens = solicitacao.ItensSolicitacao;

        // Limpa e carrega cabeçalho
        CarregarCabecalhoItens();

        // Itens da Solicitação
        for (int linhaAtual = 0; linhaAtual < itens.Count; linhaAtual++)
        {
            var item = itens[linhaAtual];

            // Adicionar uma nova linha para cada item
            gridItensSolicitacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Código
            var edCodigo = new Entry
            {
                Text = item.Id.ToString(),
                IsReadOnly = true,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(edCodigo, linhaAtual + 1);
            Grid.SetColumn(edCodigo, 0);
            gridItensSolicitacao.Children.Add(edCodigo);

            // Descrição
            var itensSolicitacao = _con.Table<Item>().ToList();
            var edDescricaoItem = new Picker
            {
                ItemsSource = itensSolicitacao,
                ItemDisplayBinding = new Binding("Descricao"),
                SelectedIndex = itensSolicitacao.IndexOf(itensSolicitacao.FirstOrDefault(i => i.Id == item.IdItem)),
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
//            edDescricaoItem.SelectedIndexChanged += Picker_SelectedIndexChanged;
            Grid.SetRow(edDescricaoItem, linhaAtual + 1);
            Grid.SetColumn(edDescricaoItem, 1);
            gridItensSolicitacao.Children.Add(edDescricaoItem);

            // Unidade de Medida
            var unidadesMedida = _con.Table<UnidadeMedida>().ToList();
            var edUnidadeMedida = new Picker
            {
                ItemsSource = unidadesMedida,
                ItemDisplayBinding = new Binding("Descricao"),
                MaximumWidthRequest = 100,
                SelectedIndex = unidadesMedida.IndexOf(unidadesMedida.FirstOrDefault(i => i.Id == item.IdUnidadeMedida)),
            };
//            edUnidadeMedida.SelectedIndexChanged += Picker_SelectedIndexChanged;
            Grid.SetRow(edUnidadeMedida, linhaAtual + 1);
            Grid.SetColumn(edUnidadeMedida, 2);
            gridItensSolicitacao.Children.Add(edUnidadeMedida);

            // Quantidade
            var edQuantidade = new Entry
            {
                Text = item.Quantidade.ToString("#,##0.000"),
                Keyboard = Keyboard.Numeric,
                MaximumWidthRequest = 100,
                HorizontalOptions = LayoutOptions.Center
            };
            int maxTotalLength = 10;
            edQuantidade.TextChanged += (sender, e) =>
            {
                string novoTexto = e.NewTextValue;
                if (novoTexto.Length > maxTotalLength)
                {
                    edQuantidade.Text = e.OldTextValue;
                    return;
                }
                if (!(decimal.TryParse(e.NewTextValue, out var quantidade)))
                {
                    DisplayAlert("Erro", "Valor inválido", "OK");
                }
                else
                {
                    item.Quantidade = quantidade;
                    edQuantidade.Text = quantidade.ToString("#,###.###", new CultureInfo("pt-BR"));
                }
            };
            Grid.SetRow(edQuantidade, linhaAtual + 1);
            Grid.SetColumn(edQuantidade, 3);
            gridItensSolicitacao.Children.Add(edQuantidade);

            // Valor Unitário
            var edValorUnitario = new Entry
            {
                Text = item.ValorUnitario.ToString("F2", CultureInfo.CurrentCulture),
                Keyboard = Keyboard.Numeric,
                MaximumWidthRequest = 100,
                HorizontalOptions = LayoutOptions.Center
            };
            edValorUnitario.TextChanged += (sender, e) =>
            {
                string novoTexto = e.NewTextValue;
                if (novoTexto.Length > maxTotalLength)
                {
                    edValorUnitario.Text = e.OldTextValue;
                    return;
                }
                if (!(decimal.TryParse(e.NewTextValue, out var valorUnitario)))
                {
                    DisplayAlert("Erro", "Valor inválido", "OK");
                }
                else
                {
                    item.ValorUnitario = valorUnitario;
                    item.ValorTotal = item.Quantidade * item.ValorUnitario;
                    var lbCalcValorTotal = gridItensSolicitacao.Children.OfType<Label>()
                        .FirstOrDefault(c => Grid.GetRow(c) == linhaAtual && Grid.GetColumn(c) == 5);
                    if (lbCalcValorTotal.Text != null)
                    {
                        lbCalcValorTotal.Text = item.ValorTotal.ToString("F2", CultureInfo.CurrentCulture);
                    }
                }
            };
            Grid.SetRow(edValorUnitario, linhaAtual + 1);
            Grid.SetColumn(edValorUnitario, 4);
            gridItensSolicitacao.Children.Add(edValorUnitario);

            // Valor Total
            var lbCalcValorTotal = new Label
            {
                Text = item.ValorTotal.ToString("F2", CultureInfo.InvariantCulture),
                MaximumWidthRequest = 100,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(lbCalcValorTotal, linhaAtual + 1);
            Grid.SetColumn(lbCalcValorTotal, 5);
            gridItensSolicitacao.Children.Add(lbCalcValorTotal);

            // Botão Salvar ItemSolicitação
            var btSalvarItemSolicitacao = new Button
            {
                ImageSource = "salvar.png",
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 10,
                WidthRequest = 10
            };
            Grid.SetRow(btSalvarItemSolicitacao, linhaAtual + 1);
            Grid.SetColumn(btSalvarItemSolicitacao, 6);
            btSalvarItemSolicitacao.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    SalvarItemSolicitacao(item, linhaAtual);
                })
            });
            gridItensSolicitacao.Children.Add(btSalvarItemSolicitacao);
        }
    }

    private void EdUnidadeMedida_SelectedIndexChanged(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Picker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is Picker descricaoPicker && descricaoPicker.BindingContext is ItemSolicitacao item)
        {
            string valorSelecionado = descricaoPicker.SelectedItem?.ToString();
            item.DescricaoItem = valorSelecionado;
        }
    }

    private void AdicionarItemSolicitacao(int solicitacaoId, int indexLinha)
    {
        ItemSolicitacao _itemSolicitacao = new ItemSolicitacao();

        _itemSolicitacao.IdSolicitacao = solicitacaoId;

        //Descrição
        var pickerDescricaoItem = gridItensSolicitacao.Children
            .OfType<Picker>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 1);
        if (pickerDescricaoItem.SelectedItem != null)
        {
            var descricaoSelecionada = pickerDescricaoItem.SelectedItem?.ToString();
            _itemSolicitacao.DescricaoItem = descricaoSelecionada;
            _itemSolicitacao.IdItem = _con.Table<Item>().FirstOrDefault(i => i.Descricao == descricaoSelecionada)?.Id ?? 0;
        }

        //Unidade de Medida
        var pickerUnidadeMedida = gridItensSolicitacao.Children
            .OfType<Picker>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 2);
        if (pickerUnidadeMedida.SelectedItem != null)
        {
            var undSelecionada = pickerUnidadeMedida.SelectedItem?.ToString();
            _itemSolicitacao.UnidadeMedida = undSelecionada;
            _itemSolicitacao.IdUnidadeMedida = _con.Table<UnidadeMedida>().FirstOrDefault(i => i.Descricao == undSelecionada)?.Id ?? 0;
        }

        //Quantidade
        var entryQuantidade = gridItensSolicitacao.Children
            .OfType<Entry>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 3);
        if (entryQuantidade.Text != null && decimal.TryParse(entryQuantidade.Text, out var quantidade))
        {
            _itemSolicitacao.Quantidade = quantidade;
        }

        // Valor Unitário
        var entryValorUnitario = gridItensSolicitacao.Children
            .OfType<Entry>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 4);
        if (entryValorUnitario.Text != null && decimal.TryParse(entryValorUnitario.Text, out var valorUnitario))
        {
            _itemSolicitacao.ValorUnitario = valorUnitario;
        }

        // Valor Total
        var entryValorTotal = gridItensSolicitacao.Children
            .OfType<Entry>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 5);
        _itemSolicitacao.ValorTotal = _itemSolicitacao.Quantidade * _itemSolicitacao.ValorUnitario;

        if (entryQuantidade.Text != "" && entryValorUnitario.Text != "" && pickerDescricaoItem.SelectedItem != null)
        {
            // Adiciona o item de solicitação no banco de dados
            _con.Insert(_itemSolicitacao);
            DisplayAlert("Sucesso", "Item atualizado com sucesso.", "OK");
        }
        else
        {
            DisplayAlert("Alerta", "Campos obrigatórios: 'Item', 'Quantidade' e 'Valor Unitário'", "OK");
        }
    }

    public void SalvarItemSolicitacao(ItemSolicitacao itemSolicitacao, int indexLinha)
    {
        // Descrição
        var edDescricaoItem = gridItensSolicitacao.Children
            .OfType<Picker>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 1);
        if (edDescricaoItem.SelectedItem != null)
        {
            var descricaoSelecionada = edDescricaoItem.SelectedItem?.ToString();
            itemSolicitacao.DescricaoItem = descricaoSelecionada;
            itemSolicitacao.IdItem = _con.Table<Item>().FirstOrDefault(i => i.Descricao == descricaoSelecionada)?.Id ?? 0;
        }

        // Unidade de Medida
        var edUnidadeMedida = gridItensSolicitacao.Children
            .OfType<Picker>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 2);
        if (edUnidadeMedida.SelectedItem != null)
        {
            itemSolicitacao.UnidadeMedida = edUnidadeMedida.SelectedItem?.ToString();
            itemSolicitacao.IdUnidadeMedida = _con.Table<UnidadeMedida>().FirstOrDefault(i => i.Descricao == itemSolicitacao.UnidadeMedida)?.Id ?? 0;
        }

        // Quantidade
        var edQuantidade = gridItensSolicitacao.Children
            .OfType<Entry>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 3);
        if (edQuantidade.Text != null && edQuantidade.Text != "")
        {
            itemSolicitacao.Quantidade = decimal.Parse(edQuantidade.Text);
        }

            // Valor Unitário
            var edValorUnitario = gridItensSolicitacao.Children
                .OfType<Entry>()
                .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 4);
        if (edValorUnitario.Text != null && edValorUnitario.Text != "")
        {
            itemSolicitacao.ValorUnitario = decimal.Parse(edValorUnitario.Text);
        }

        // Valor Total
        var lbCalcValorTotal = gridItensSolicitacao.Children
            .OfType<Label>()
            .FirstOrDefault(c => Grid.GetRow(c) == indexLinha && Grid.GetColumn(c) == 5);
        if (lbCalcValorTotal.Text != null)
        {
            lbCalcValorTotal.Text = (itemSolicitacao.Quantidade * itemSolicitacao.ValorUnitario).ToString("F2", CultureInfo.CurrentCulture); //pt-BR
        }

        if (edQuantidade.Text != "" && edValorUnitario.Text != "" && edDescricaoItem.SelectedItem != null)
        {
            // Atualiza o item de solicitação no banco de dados
            _con.Update(itemSolicitacao);
            DisplayAlert("Sucesso", "Item atualizado com sucesso.", "OK");
        }
        else
        {
            DisplayAlert("Alerta", "Campos obrigatórios: 'Item', 'Quantidade' e 'Valor Unitário'", "OK");
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
        //var solicitanteSelecionado = EdUsuario.SelectedItem as Usuario;
        //_solicitacao.IdSolicitante = solicitanteSelecionado?.Id ?? 0;
        //_solicitacao.DataSolicitacao = DateTime.ParseExact(DtSolicitacao.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //_solicitacao.NivelUrgencia = (int)NivelUrgenciaPicker.SelectedIndex+1;

        //if (_solicitacao.IdSolicitante == 0 || _solicitacao.NivelUrgencia == 0 || _solicitacao.DataSolicitacao == DateTime.MinValue)
        //{
        //    await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
        //    return;
        //}

        //var registroExistente = _con.Find<Solicitacao>(_solicitacao.Id);
        //if (registroExistente != null)
        //{
        //    bool confirmacao = await DisplayAlert(
        //        "Confirmação", "Qualquer alteração feita será registrada. Deseja salvar?",
        //        "Sim", "Não"
        //    );

        //    if (confirmacao) {
        //        _con.Update(_solicitacao);
        //        await DisplayAlert("Sucesso", "Solicitação salva com sucesso.", "OK");
        //    }
        //}
        //else
        //{
        //    _con.Insert(_solicitacao);
        //    await DisplayAlert("Sucesso", "Nova solicitação inserida com sucesso.", "OK");
        //    AddNovaSolicitacao(_solicitacao);
        //}
    }
  
    private void CarregarUsuarios()
    {
        var usuarios = _con.Table<Usuario>().ToList();
        EdUsuario.ItemsSource = usuarios;
        EdUsuario.ItemDisplayBinding = new Binding("NomeUsuario");
    }

    private void CarregarSolicitacao(Solicitacao solicitacao)
    {
        if (EdUsuario.ItemsSource is List<Usuario> usuarios)
        {
            int index = usuarios.FindIndex(u => u.Id == solicitacao.IdSolicitante);
            if (index >= 0 && index < EdUsuario.ItemsSource.Count)
            {
                EdUsuario.SelectedIndex = index;
            }
            else
            {
                EdUsuario.SelectedIndex = -1;
            }
        }

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
        //bool confirmacao = await DisplayAlert(
        //    "Confirmação", "Tem certeza de que deseja excluir esta solicitação?",
        //    "Sim", "Não"
        //);

        //var registroExistente = _con.Find<Solicitacao>(_solicitacao.Id);
        //if (registroExistente != null)
        //{
        //    if (confirmacao)
        //    {
        //        _con.Delete(_solicitacao);
        //        await DisplayAlert("Sucesso", "Solicitação excluída com sucesso.", "OK");
        //        await Navigation.PopAsync();
        //    }
        //}
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
            CarregarSolicitacao(_solicitacao);
            CarregarItensSolicitacao(_solicitacao);
            NovoItemSolicitacao(_solicitacao);
        }
    }
}