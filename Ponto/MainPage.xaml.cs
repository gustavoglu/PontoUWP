using Domain.Ponto.APIGoogle;
using Domain.Ponto.WS;
using Ponto.DB;
using Ponto.Util;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ponto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Domain.Ponto.Ponto> Pontos;

        ObservableCollection<WebLocal> Enderecos;

        ObservableCollection<Geofence> Geofences;

        SQLiteConnection Conn;

        Size size;

        double WidthRes;
        double HeightRes;



        string escolha;

        public MainPage()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
            this.WidthRes = size.Width;
            this.HeightRes = size.Height;

            this.InitializeComponent();

            App.Heandler_Resuming += App_Heandler_Resuming;

            Pontos = new ObservableCollection<Domain.Ponto.Ponto>();

            Enderecos = new ObservableCollection<WebLocal>();

            Geofences = new ObservableCollection<Geofence>();

            Conn = SQLiteConn.Conn();

            this.Map_Locais.Loaded += Map_Locais_Loaded;

            this.listV_Pontos.ItemsSource = Pontos;

            this.listV_Geofences.ItemsSource = Geofences;

            this.listV_Geofences.SelectionChanged += ListV_Geofences_SelectionChanged;

            this.listV_Pontos.SelectionChanged += ListV_Pontos_SelectionChanged;

            this.Map_Locais.MapServiceToken = Domain.Ponto.Constantes.APIMAPBING_KEY;

 

            RegisterTask();

            CriaTabelas();

            CarregaPontos();

            getGeofences();

        }

        private void ListV_Geofences_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listview = sender as ListView;
            var geofence = listview.SelectedItem as Geofence;
            if (geofence != null)
            {
                var latitude = ((Geocircle)geofence.Geoshape).Center.Latitude;
                var longitutde = ((Geocircle)geofence.Geoshape).Center.Longitude;
                var radiu = ((Geocircle)geofence.Geoshape).Radius;
                this.Map_Locais.Center = new Geopoint(new BasicGeoposition() { Latitude = latitude, Longitude = longitutde });
                this.tb_latitude.Text = latitude.ToString();
                this.tb_longitude.Text = longitutde.ToString();
                this.tb_radiu.Text = radiu.ToString();
                this.tb_descLocal.Text = geofence.Id;
                listview.SelectedItem = null;
            }
          
        }

        private async void Map_Locais_Loaded(object sender, RoutedEventArgs e)
        {
            var map = sender as MapControl;
            Geolocator geo = new Geolocator();
            Geoposition position = await geo.GetGeopositionAsync();
            double latitude = Math.Round(position.Coordinate.Point.Position.Latitude, 6);
            double longitude = Math.Round(position.Coordinate.Point.Position.Longitude, 6);
            map.Center = new Geopoint(new BasicGeoposition()
            {
                Latitude = latitude,
                Longitude = longitude
            });
        }

        private void ListV_Pontos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listview = sender as ListView;
            var ponto = listview.SelectedItem as Domain.Ponto.Ponto;

            carregaGeofenceFlyout(ponto, tb_descFlyout, cb_localFlyout, dp_dataFlyout, tp_chegadaFlyout, tp_saida);
            FlyoutBase.ShowAttachedFlyout(listview);
            listview.SelectedItem = null;
        }

        private void App_Heandler_Resuming()
        {
            var gofences = GeofenceMonitor.Current.Geofences;

            if (Geofences.Any())
            {
                this.Geofences.Clear();
                foreach (var item in gofences)
                {
                    this.Geofences.Add(item);
                }
            }

            var pontos = Conn.Table<Domain.Ponto.Ponto>().ToList();
            if (pontos.Any())
            {
                this.Pontos.Clear();
                foreach (var item in pontos)
                {
                    this.Pontos.Add(item);
                }
            }


        }

        private void getGeofences()
        {
            foreach (var item in GeofenceMonitor.Current.Geofences)
            {
                Geofences.Add(item);
            }
        }

        private void tbs_endereco_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            tbs_endereco.Text = "Choose";
        }

        private void tbs_endereco_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                var weblocal = args.ChosenSuggestion as WebLocal;
                this.tbs_endereco.Text = args.ChosenSuggestion.ToString();
                this.escolha = args.ChosenSuggestion.ToString();
                this.Enderecos.Clear();
                this.tb_latitude.Text = weblocal.geometry.location.lat.ToString();
                this.tb_longitude.Text = weblocal.geometry.location.lng.ToString();
                Map_Locais.Center = new Geopoint(new BasicGeoposition() { Latitude = weblocal.geometry.location.lat, Longitude = weblocal.geometry.location.lng });
            }
            else
            {
                tbs_endereco.Text = sender.Text;
            }
        }

        private void tbs_endereco_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender.Text.Count() > 3)
            {
                Enderecos.Clear();

                if (sender.Text != escolha)
                {
                    getEnderecos(sender);
                }
                sender.ItemsSource = Enderecos;
            }
        }

        private async void getEnderecos(AutoSuggestBox asb_sender)
        {
            var web = await WSOpen.GetWebAddress<Result>(asb_sender.Text.Split(' '), "address");

            if (web != null)
            {
                foreach (var item in web.results)
                {
                    if (!Enderecos.ToList().Exists(i => i.address_components == item.address_components))
                    {
                        Enderecos.Add(item);
                    }

                }

            }
        }

        private async void bt_AdicionarLocal_Click(object sender, RoutedEventArgs e)
        {
            if (tb_descLocal.Text.Any() && tb_latitude.Text.Any() && tb_longitude.Text.Any())
            {

                var escolha = await Utilidades.MessageDialogShow(string.Format("Tem certeza que quer adicionar o Local \n Descrição:{0} \n Latitude:{1} \n Longitude:{2}", tb_descLocal.Text, tb_latitude.Text, tb_longitude.Text));

                if (escolha)
                {
                    double radiu;
                    if (tb_radiu.Text != null && (tb_radiu.Text != ""))
                    {
                        radiu = double.Parse(tb_radiu.Text);
                    }
                    else
                    {
                         radiu = 100;
                    }

                    BasicGeoposition basic = new BasicGeoposition() { Latitude = double.Parse(tb_latitude.Text), Longitude = double.Parse(tb_longitude.Text) };
                    Geocircle circle = new Geocircle(basic, radiu);

                    Geofence geofence = new Geofence(
                        tb_descLocal.Text
                        , circle
                        , MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited
                        , false
                        , TimeSpan.FromSeconds(30)
                        , DateTime.Now.Date
                        , TimeSpan.MaxValue);

                    if (!GeofenceMonitor.Current.Geofences.ToList().Exists(g => g.Id == tb_descLocal.Text))
                    {
                        GeofenceMonitor.Current.Geofences.Add(geofence);
                        this.Geofences.Add(geofence);
                    }
                    else
                    {
                        Utilidades.MessageShow("Esta descrição ja é usado por outro Local ja Salvo");
                    }

                    //Debug.Write(string.Format("Geofence {0} Adicionado"), tb_descLocal.Text);


                }


            }
            else
            {
                Utilidades.MessageShow("Os Campos precisam estar preenchidos!");
            }

        }

        private void CriaTabelas()
        {
            Conn.CreateTable<Domain.Ponto.Ponto>();
            //Conn.CreateTable<Domain.Ponto.Local>();
        }

        private void CarregaPontos()
        {
            if (Conn.Table<Domain.Ponto.Ponto>().Any())
            {
                foreach (var item in Conn.Table<Domain.Ponto.Ponto>().ToList())
                {
                    Pontos.Add(item);
                }
            }
        }

        public async void RegisterTask()
        {
            if (!IsTaskRegistered())
            {

                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                BackgroundTaskBuilder taskbuilder = new BackgroundTaskBuilder()
                {
                    Name = nameof(GeofenceBackground.TaskGeofence)
                    ,
                    TaskEntryPoint = typeof(GeofenceBackground.TaskGeofence).FullName

                };
                taskbuilder.SetTrigger(new LocationTrigger(LocationTriggerType.Geofence));

                taskbuilder.Register();

                Utilidades.MessageShow("Tarefa em Segundo Plano Registrada");

            }
        }

        public static bool IsTaskRegistered()
        {

            return BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == nameof(GeofenceBackground.TaskGeofence));
        }

        private async void bbar_Local_Click(object sender, RoutedEventArgs e)
        {
            Geolocator geo = new Geolocator();
            Geoposition position = await geo.GetGeopositionAsync();
            double latitude = Math.Round(position.Coordinate.Point.Position.Latitude, 6);
            double longitude = Math.Round(position.Coordinate.Point.Position.Longitude, 6);
            this.tb_latitude.Text = latitude.ToString();
            this.tb_longitude.Text = longitude.ToString();
            this.Map_Locais.Center = new Geopoint(new BasicGeoposition()
            {
                Latitude = latitude,
                Longitude = longitude
            });
            this.Map_Locais.ZoomLevel = 18;
        }

        private void bbar_update_Click(object sender, RoutedEventArgs e)
        {
            var gofences = GeofenceMonitor.Current.Geofences;

            if (Geofences.Any())
            {
                this.Geofences.Clear();
                foreach (var item in gofences)
                {
                    this.Geofences.Add(item);
                }
            }

            var pontos = Conn.Table<Domain.Ponto.Ponto>().ToList();
            if (pontos.Any())
            {
                this.Pontos.Clear();
                foreach (var item in pontos)
                {
                    this.Pontos.Add(item);
                }
            }
        }

        private async void flyout_Locais_Remove_Click(object sender, RoutedEventArgs e)
        {
            var geofence = ((FrameworkElement)e.OriginalSource).DataContext as Geofence;
            var escolha = await Utilidades.MessageDialogShow(string.Format("Tem certeza que deseja parar de Monitorar este local: {0}?", geofence.Id));
            if (escolha)
            {
                GeofenceMonitor.Current.Geofences.Remove(geofence);
                this.Geofences.Remove(geofence);
            }
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

        }

        private void bba_cancelaPonto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bba_confirmaPonto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void carregaGeofenceFlyout(Domain.Ponto.Ponto ponto, TextBox tb_desc, ComboBox cb_locais, DatePicker dp_data, TimePicker tp_chegada, TimePicker tp_saida)
        {
            if (ponto != null)
            {
                cb_locais.ItemsSource = this.Geofences;
                if (Geofences.ToList().Exists(p => p.Id == ponto.DescGeofence))
                {
                    cb_locais.SelectedItem = ponto;
                }

                dp_data.Date = ponto.Data;
                tp_chegada.Time = ponto.HoraChegada;
                tp_saida.Time = ponto.HoraSaida;
            }
        }

        private void bt_more_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void bt_excluir_Click(object sender, RoutedEventArgs e)
        {
            var ponto = ((FrameworkElement)e.OriginalSource).DataContext as Domain.Ponto.Ponto;
            var escolha = await Utilidades.MessageDialogShow("Tem certeza que quer deletar este Ponto?");
            if (escolha)
            {
                if (Conn.Table<Domain.Ponto.Ponto>().ToList().Exists(p => p.Id == ponto.Id))
                {
                    Conn.Delete(ponto);
                }

                Pontos.Remove(ponto);
            }

        }
    }
}
