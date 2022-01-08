using Insurance_app.Models;
using Realms;

namespace Insurance_app
{
    public class RealmDb
    {

        public async void AddMovData(MovData movData)
        {
            using (var realm = Realm.GetInstanceAsync())
            {
                
            };
            
        }
    }
}