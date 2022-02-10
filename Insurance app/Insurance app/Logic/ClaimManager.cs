using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Java.Util;
using Realms;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class ClaimManager : IDisposable
    {
        private RealmDb db;
        public List<Claim> Claims { get; set; }

        public ClaimManager()
        {
            db = new RealmDb();
        }
        public async Task CreateClaim(string hospitalPostcode, string patientNr, string type, bool status, User user)
        {
            await db.AddClaim(hospitalPostcode, patientNr, type, status, user);
        }

        public async Task<List<Claim>> GetClaims(User user)
        {
            Claims = await db.GetClaims(user);
            return Claims;
        }

        public Claim GetCurrentClaim()
        {
            try
            {
                return Claims.FirstOrDefault(claim => claim.CloseDate == null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetCurrentClaim error : {e}");
            }

            return null;
        }

        public void Dispose()
        {
            if (db !=null) db.Dispose();
            if (Claims != null) Claims = null;
        }
    }
}