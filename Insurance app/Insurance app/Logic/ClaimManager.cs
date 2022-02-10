using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class ClaimManager
    {
        private RealmDb db;
        public ClaimManager()
        {
            db = new RealmDb();
        }
        public async Task CreateClaim(string hospitalPostcode, string patientNr, string type, bool status, User user)
        {
            await db.AddClaim(hospitalPostcode, patientNr, type, status, user);
        }
    }
}