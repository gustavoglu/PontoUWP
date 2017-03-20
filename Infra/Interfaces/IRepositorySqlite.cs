using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Interfaces
{
    public interface IRepositorySqlite <T>
    {
        T Criar(T obj);

        T Atualizar(T obj);

        T TrazerPorId(int Id);

        IEnumerable<T> Pesquisar(Expression<Func<T,bool>> Expressao);

        IEnumerable<T> TrazerAtivos();

        IEnumerable<T> TrazerInativos();
    }
}
