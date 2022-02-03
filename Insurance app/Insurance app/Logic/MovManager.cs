using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class MovManager
    {
        private RealmDb db;
        public MovManager()
        {
            db = new RealmDb();
        }

        public void AddMovData(List<MovData> list,Reward reward)
        {
            
            Task.FromResult(db.AddMovData(list,reward,App.RealmApp.CurrentUser));

        }

    }
}