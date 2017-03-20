using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Ponto.Util
{
    public class Utilidades
    {
        public async static Task<bool> MessageDialogShow(string mensagem)
        {
            MessageDialog msg = new MessageDialog(mensagem,"Atenção");
            msg.Commands.Add((new UICommand("Confirmar") { Id = 1 }));
            msg.Commands.Add(new UICommand("Cancelar") { Id = 0 });
            var escolha = await msg.ShowAsync();
            if (escolha.Label == "Cancelar")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async static void MessageShow(string mensagem)
        {
            MessageDialog msg = new MessageDialog(mensagem);
            msg.Commands.Add(new UICommand("Ok"));  
            await msg.ShowAsync();
        }
    }
}
