using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Interfaces
{
    public class RepositorySQLite<T> : IRepositorySqlite<T>
    {
       // protected SQLiteConnection _SqliteConnection;

        public RepositorySQLite()
        {
           // this._SqliteConnection = new SQLiteConnection(,);
        }

        public T Atualizar(T obj)
        {
            throw new NotImplementedException();
        }

        public T Criar(T obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Pesquisar(Expression<Func<T, bool>> Expressao)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> TrazerAtivos()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> TrazerInativos()
        {
            throw new NotImplementedException();
        }

        public T TrazerPorId(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
