using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ponto
{
    public abstract class Base
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime CriadoEm { get; set; }

        public DateTime DeletadoEm { get; set; }

        public bool Deletado { get; set; }
    }
}
