using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace prjSolicitaCompras
{
    internal class AppShellViewModel
    {
        public ICommand NavigateToMainPageCommand { get; }
        public ICommand NavigateToNovaSolicitacaoCommand { get; }
        public ICommand NavigateToListaSolicitacoesCommand { get; }

        public AppShellViewModel()
        {
            NavigateToMainPageCommand = new Command(static async () => await Shell.Current.GoToAsync("MainPage"));
            NavigateToNovaSolicitacaoCommand = new Command(async () => await Shell.Current.GoToAsync("NovaSolicitacao"));
            NavigateToListaSolicitacoesCommand = new Command(async () => await Shell.Current.GoToAsync("ListaSolicitacoes"));
        }
    }
}
