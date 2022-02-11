using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class ReportManager : IDisposable
    {
        public ReportManager()
        {
        }

        public Task<Dictionary<string,int>> GetWeeksMovData(User user)
        {
            return RealmDb.GetInstance().GetWeeksMovData(user);
        }

        public void Dispose()
        {
            RealmDb.GetInstance().Dispose();
        }
    }
}