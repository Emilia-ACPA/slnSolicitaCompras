using System.Globalization;
using System.Reflection.Metadata;
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
        try
        {
            CarregarUsuarios();

            if (solicitacao.Id == 0)
            {
                AddNovaSolicitacao(solicitacao);
                CarregarCabecalhoItens();
                // NovoItemSolicitacao(solicitacao.Id); Novo item não é liberado neste momento. Não faz sentido porque a Solicitação não existe ainda.
            }
            else
            {
                CarregarSolicitacao(solicitacao);
                CarregarItensSolicitacao(solicitacao.Id);
                NovoItemSolicitacao(solicitacao.Id);
            }
            _solicitacao = solicitacao;

        }
        catch (SQLiteException e)
        {
            DisplayAlert("Erro", "Erro ao carregar os dados: " + e.Message, "OK");
        }
    }
    private void NovoItemSolicitacao(int solicitacaoId)
    {
        List<ItemSolicitacao> itens = _con.Table<ItemSolicitacao>().Where(i => i.IdSolicitacao == solicitacaoId).ToList();

        gridItensSolicitacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        int linhaAtualGrid = itens.Count + 1;

        // Código
        var edCodigo = new Entry
        {
            MaximumWidthRequest = 80
        };
        Grid.SetRow(edCodigo, linhaAtualGrid);
        Grid.SetColumn(edCodigo, 0);
        this.gridItensSolicitacao.Children.Add(edCodigo);

        // Descrição
        var edDescricao = new Picker
        {
            MaximumWidthRequest = 800,
            ItemsSource = _con.Table<Item>().Select(i => i.Descricao).ToList(),

        };
        Grid.SetRow(edDescricao, linhaAtualGrid);
        Grid.SetColumn(edDescricao, 1);
        this.gridItensSolicitacao.Children.Add(edDescricao);

        // Unidade de Medida
        var edUnidadeMedida = new Picker
        {
            MaximumWidthRequest = 100,
            ItemsSource = _con.Table<UnidadeMedida>().Select(i => i.Descricao).ToList(),
        };
        Grid.SetRow(edUnidadeMedida, linhaAtualGrid);
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
        Grid.SetRow(edQuantidade, linhaAtualGrid);
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
                        .FirstOrDefault(c => Grid.GetRow(c) == linhaAtualGrid && Grid.GetColumn(c) == 5);
                    if (lbCalcValorTotal.Text != null)
                    {
                        lbCalcValorTotal.Text = valorTotal.ToString("F2", CultureInfo.CurrentCulture);
                    }
                }
            }
        };
        Grid.SetRow(edValorUnitario, linhaAtualGrid);
        Grid.SetColumn(edValorUnitario, 4);
        this.gridItensSolicitacao.Children.Add(edValorUnitario);

        // Valor Total
        var lbCalcValorTotal = new Label
        {
            MaximumWidthRequest = 100

        };
        Grid.SetRow(lbCalcValorTotal, linhaAtualGrid);
        Grid.SetColumn(lbCalcValorTotal, 5);
        this.gridItensSolicitacao.Children.Add(lbCalcValorTotal);

        // Botão AddItem ItemSolicitação
        var btAddItem = new Button
        {
            ImageSource = "adicionaritem.png",
            HorizontalOptions = LayoutOptions.Center,
            HeightRequest = 10,
            WidthRequest = 10
        };
        Grid.SetRow(btAddItem, linhaAtualGrid);
        Grid.SetColumn(btAddItem, 6);
        btAddItem.Focus();
        btAddItem.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                AdicionarItemSolicitacao(_solicitacao.Id, linhaAtualGrid);

                // Define o foco no próximo edDescricao, para resolver o problema de ativação do picker do usuário. A corrigir. Todo
                var proximoEdDescricao = gridItensSolicitacao.Children
                    .OfType<Picker>()
                    .FirstOrDefault(c => Grid.GetRow(c) == linhaAtualGrid + 1 && Grid.GetColumn(c) == 1);

                if (proximoEdDescricao != null)
                {
                    proximoEdDescricao.Focus();
                }
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

        var lbExcluirItem = new Label
        {
            FontAttributes = FontAttributes.Bold,
            MinimumWidthRequest = 50
        };
        Grid.SetRow(lbExcluirItem, 0);
        Grid.SetColumn(lbExcluirItem, 7);
        this.gridItensSolicitacao.Children.Add(lbExcluirItem);
    }

    private void CarregarItensSolicitacao(int solicitacaoId)
    {
        List<ItemSolicitacao> itens = _con.Table<ItemSolicitacao>().Where(i => i.IdSolicitacao == solicitacaoId).ToList();

        // Limpa e carrega cabeçalho
        CarregarCabecalhoItens();

        // Itens da Solicitação
        for (int i = 0; i < itens.Count; i++)
        {
            var item = itens[i];
            var linhaAtualGrid = i + 1;   //Iniciando de 1, porque linha 0 é o cabeçalho do grid

            // Adicionar uma nova linha para cada item
            gridItensSolicitacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Código
            var edCodigo = new Entry
            {
                Text = item.Id.ToString(),
                IsReadOnly = true,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(edCodigo, linhaAtualGrid);
            Grid.SetColumn(edCodigo, 0);
            gridItensSolicitacao.Children.Add(edCodigo);

            // Descrição
            var itensSolicitacao = _con.Table<Item>().ToList();
            var edDescricaoItem = new Picker
            {
                ItemsSource = _con.Table<Item>().Select(i => i.Descricao).ToList(),
                SelectedIndex = itensSolicitacao.IndexOf(itensSolicitacao.FirstOrDefault(i => i.Id == item.IdItem)),
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            Grid.SetRow(edDescricaoItem, linhaAtualGrid);
            Grid.SetColumn(edDescricaoItem, 1);
            gridItensSolicitacao.Children.Add(edDescricaoItem);

            // Unidade de Medida
            var unidadesMedida = _con.Table<UnidadeMedida>().ToList();
            var edUnidadeMedida = new Picker
            {
                ItemsSource = _con.Table<UnidadeMedida>().Select(i => i.Descricao).ToList(),
                MaximumWidthRequest = 100,
                SelectedIndex = unidadesMedida.IndexOf(unidadesMedida.FirstOrDefault(i => i.Id == item.IdUnidadeMedida)),
            };
            Grid.SetRow(edUnidadeMedida, linhaAtualGrid);
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
            Grid.SetRow(edQuantidade, linhaAtualGrid);
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
                        .FirstOrDefault(c => Grid.GetRow(c) == linhaAtualGrid && Grid.GetColumn(c) == 5);
                    if (lbCalcValorTotal.Text != null)
                    {
                        lbCalcValorTotal.Text = item.ValorTotal.ToString("F2", CultureInfo.CurrentCulture);
                    }
                }
            };
            edValorUnitario.Unfocused += (sender, e) =>
            {
                // Define o foco no próximo edDescricao, para resolver o problema de ativação do picker do usuário. A corrigir. Todo
                var proximoEdDescricao = gridItensSolicitacao.Children
                    .OfType<Picker>()
                    .FirstOrDefault(c => Grid.GetRow(c) == linhaAtualGrid + 1 && Grid.GetColumn(c) == 1);

                if (proximoEdDescricao != null)
                {
                    proximoEdDescricao.Focus();
                }
            };
            Grid.SetRow(edValorUnitario, linhaAtualGrid);
            Grid.SetColumn(edValorUnitario, 4);
            gridItensSolicitacao.Children.Add(edValorUnitario);

            // Valor Total
            var lbCalcValorTotal = new Label
            {
                Text = item.ValorTotal.ToString("F2", CultureInfo.InvariantCulture),
                MaximumWidthRequest = 100,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(lbCalcValorTotal, linhaAtualGrid);
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
            Grid.SetRow(btSalvarItemSolicitacao, linhaAtualGrid);
            Grid.SetColumn(btSalvarItemSolicitacao, 6);
            btSalvarItemSolicitacao.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    SalvarItemSolicitacao(item, linhaAtualGrid);
                })
            });
            gridItensSolicitacao.Children.Add(btSalvarItemSolicitacao);

            // Botão Excluir ItemSolicitação
            var btExcluirItemSolicitacao = new Button
            {
                ImageSource = "excluir.png",
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 10,
                WidthRequest = 10
            };
            Grid.SetRow(btExcluirItemSolicitacao, linhaAtualGrid);
            Grid.SetColumn(btExcluirItemSolicitacao, 7);
            btExcluirItemSolicitacao.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    ExcluirItemSolicitacao(item, linhaAtualGrid);
                })
            });
            gridItensSolicitacao.Children.Add(btExcluirItemSolicitacao);
        }
    }

    private async void ExcluirItemSolicitacao(ItemSolicitacao item, int linhaAtual)
    {
        bool confirmacao = await DisplayAlert(
            "Confirmação", "Tem certeza de que deseja excluir o item selecionado?",
            "Sim", "Não"
        );

        if (confirmacao)
        {
            _con.Delete(item);

            var linhaAtualGrid = gridItensSolicitacao.Children
                .Where(c => gridItensSolicitacao.GetRow(c) == linhaAtual)
                .ToList();

            foreach (var elemento in linhaAtualGrid)
            {
                gridItensSolicitacao.Children.Remove(elemento);
            }

            DisplayAlert("Sucesso", "Item excluído com sucesso.", "OK");
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
        if (solicitacaoId == 0)
        {
            DisplayAlert("Alerta", "Favor salvar a solicitação primeiro, para depois adicionar itens.", "Ok");
            return;
        }

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

            // Posteriormente atualizar apenas o botão que não será mais de inserção, mas de salvar. To do.
            CarregarItensSolicitacao(_solicitacao.Id);
            NovoItemSolicitacao(_solicitacao.Id);
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
        var solicitanteSelecionado = EdUsuario.SelectedItem as Usuario;
        _solicitacao.IdSolicitante = solicitanteSelecionado?.Id ?? 0;
        _solicitacao.DataSolicitacao = DateTime.ParseExact(DtSolicitacao.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        _solicitacao.NivelUrgencia = (int)NivelUrgenciaPicker.SelectedIndex + 1;

        if (_solicitacao.IdSolicitante == 0 || _solicitacao.NivelUrgencia == 0 || _solicitacao.DataSolicitacao == DateTime.MinValue)
        {
            await DisplayAlert("Erro", "Favor preencher todos os campos e tente novamente.", "OK");
            return;
        }

        if (_solicitacao.Id != 0)
        {
            bool confirmacao = await DisplayAlert(
                "Confirmação", "Qualquer alteração feita será registrada. Deseja salvar?",
                "Sim", "Não"
            );

            if (confirmacao)
            {
                _con.Update(_solicitacao);
                await DisplayAlert("Sucesso", "Solicitação salva com sucesso.", "OK");
            }
        }
        else
        {
            _con.Insert(_solicitacao);
            await DisplayAlert("Sucesso", "Nova solicitação inserida com sucesso.", "OK");
            //            AddNovaSolicitacao(_solicitacao); Não vamos abrir novo cadastro neste momento
            NovoItemSolicitacao(_solicitacao.Id);
        }
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
        NivelUrgenciaPicker.SelectedIndex = solicitacao.NivelUrgencia - 1;
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

        var registroExistente = _solicitacao.Id;
        if (registroExistente != null)
        {
            if (confirmacao)
            {
                _con.Execute("Delete from ItemSolicitacao where IdSolicitacao = ?", registroExistente);
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
            CarregarSolicitacao(_solicitacao);
            CarregarItensSolicitacao(_solicitacao.Id);
            NovoItemSolicitacao(_solicitacao.Id);
        }
    }
}

