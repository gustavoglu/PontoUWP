using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Ponto
{
    public class Ponto : Base
    {
        public string DescGeofence { get; set; }

        public DateTime Data { get; set; }

        public TimeSpan HoraChegada { get; set; }

        public TimeSpan HoraSaida { get; set; }

        public string DescLocalData
        {
            get
            {
                return string.Format("Local: {0} Data: {1}",
                    this.DescGeofence, this.Data.Date.ToString(@"dd/MM/yyyy"));
            }
        }

              public string DescChegadaSaida
        {
            get
            {
                return string.Format("Chegada: {0} Saida: {1}", this.HoraChegada.ToString(@"hh\:mm"), this.HoraSaida.ToString(@"hh\:mm"));
            }

        }
       // public Local Local { get; set; }
    }
}
