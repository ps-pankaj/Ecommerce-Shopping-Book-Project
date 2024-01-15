using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Project1.DataAccess.Repository.IRepository
{
    public interface ISP_CALL:IDisposable
    {
        void Execute(string procedureName, DynamicParameters param=null); //Add,update,delete
        T Single<T>(string procedureName,DynamicParameters param=null); //Find
        T OneRecord<T>(string procedureName, DynamicParameters param=null);
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);

    }
}
