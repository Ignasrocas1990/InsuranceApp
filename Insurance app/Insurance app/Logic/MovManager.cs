using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;

namespace Insurance_app.Logic
{
    public class MovManager
    {
        public MovManager()
        {
            
        }

        public Task<List<MovData>> AddMovData(List<MovData> list)
        {
            return RealmDb.GetInstance().AddMovData(list);
        }

    }
}