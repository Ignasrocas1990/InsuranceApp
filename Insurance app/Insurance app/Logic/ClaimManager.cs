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
        public List<Claim> Claims { get; set; }

        public ClaimManager()
        {
        }
        public async Task CreateClaim(string hospitalPostcode, string patientNr, string type, bool status, User user)
        {
            await RealmDb.GetInstance().AddClaim(hospitalPostcode, patientNr, type, status, user);
        }

        public async Task<List<Claim>> GetClaims(User user)
        {
            Claims = await RealmDb.GetInstance().GetClaims(user);
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
            RealmDb.GetInstance().Dispose();
            if (Claims != null) Claims = null;
        }
    }
}