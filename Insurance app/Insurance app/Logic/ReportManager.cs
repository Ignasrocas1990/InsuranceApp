using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class ReportManager : IDisposable
    {
        private RealmDb realmDb;
        public ReportManager()
        {
            realmDb = new RealmDb();
        }

        public Task<Dictionary<string,int>> GetWeeksMovData(User user)
        {
            return realmDb.GetWeeksMovData(user);
        }

        public void Dispose()
        {
            realmDb.Dispose();
        }
    }
}