using Microsoft.Toolkit.Uwp.Notifications;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace GeofenceBackground
{
    public sealed class TaskGeofence : IBackgroundTask
    {
        SQLiteConnection Conn;
        public void Run(IBackgroundTaskInstance taskInstance)
        {

            var monitor = GeofenceMonitor.Current;

            if (monitor.Geofences.Any())
            {

                var reports = monitor.ReadReports();

                foreach (var report in reports)
                {
                    var geofenceId = report.Geofence.Id;

                    switch (report.NewState)
                    {
                        case GeofenceState.None:
                            break;
                        case GeofenceState.Entered:
                            Conn = returnConn();

                            if (!Conn.Table<Domain.Ponto.Ponto>().ToList().Exists(p => p.Data.Date == DateTime.Now.Date && p.DescGeofence == geofenceId && p.HoraChegada != null))
                            {
                                string Hora = string.Format("Hora Chegada: {0}", DateTime.Now.TimeOfDay.ToString(@"hh\:mm"));
                                ShowToast(geofenceId, "Entrou no Local");
                                reports.ToList().Remove(report);
                                insertPonto(geofenceId, DateTime.Now.Date, DateTime.Now.TimeOfDay, GeofenceState.Entered);
                            }

                            break;
                        case GeofenceState.Exited:
                            Conn = returnConn();
                            if (Conn.Table<Domain.Ponto.Ponto>().ToList().Exists(p => p.Data.Date == DateTime.Now.Date && p.DescGeofence == geofenceId  && p.HoraChegada != null))
                            {
                                string Hora = string.Format("Hora Saída: {0}", DateTime.Now.TimeOfDay.ToString(@"hh\:mm"));
                                ShowToast(geofenceId, "Saiu no Local");
                                insertPonto(geofenceId, DateTime.Now.Date, DateTime.Now.TimeOfDay, GeofenceState.Exited);
                            }

                            break;
                        case GeofenceState.Removed:
                            break;
                        default:
                            break;
                    }

                }
            }


        }

        private void insertPonto(string GeofenceId, DateTime Data, TimeSpan Hora, GeofenceState status)
        {

            Conn = returnConn();//SQLiteConn.Conn();
            if (status == GeofenceState.Entered)
            {
                var exist = Conn.Table<Domain.Ponto.Ponto>().ToList().Exists(p => p.Data.Date == Data.Date && p.DescGeofence == GeofenceId);
                if (!exist)
                {
                    Domain.Ponto.Ponto ponto = new Domain.Ponto.Ponto() { Data = Data.Date, HoraChegada = Hora, DescGeofence = GeofenceId };

                    Conn.Insert(ponto);
                }
            }
            else if (status == GeofenceState.Exited)
            {
                var pontoDataNow = Conn.Table<Domain.Ponto.Ponto>().Last(p => p.Data.Date == Data && p.HoraChegada != null && p.DescGeofence == GeofenceId);
                if (pontoDataNow != null)
                {
                    pontoDataNow.HoraSaida = Hora;
                    Conn.Update(pontoDataNow);
                }

            }


        }

        SQLiteConnection returnConn()
        {
            string dbaseName = "pontodb";

            string patch = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbaseName);

            return new SQLiteConnection(new SQLitePlatformWinRT(), patch);
        }


        private static void ShowToast(string firstLine, string secondLine)
        {
            var toastXmlContent =
              ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var txtNodes = toastXmlContent.GetElementsByTagName("text");
            txtNodes[0].AppendChild(toastXmlContent.CreateTextNode(firstLine));
            txtNodes[1].AppendChild(toastXmlContent.CreateTextNode(secondLine));

            var toast = new ToastNotification(toastXmlContent);
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toast);

        }
    }
}
