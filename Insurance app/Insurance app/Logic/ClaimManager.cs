using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class ClaimManager : IDisposable
    {
        public List<Claim> Claims { get; set; }
        private RealmDb realmDb;

        public ClaimManager()
        {
            realmDb=RealmDb.GetInstancePerPage();
        }
        public int GetResolvedClaimCount()
        {
            try
            {
                return Claims.Count(c => c.CloseDate != null && c.DelFlag == false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;
        }

        public List<Claim> GetResolvedClaims()
        {
            try
            {
                return Claims.Where(c => c.CloseDate != null && c.DelFlag == false).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
        public async Task CreateClaim(string hospitalPostcode, string patientNr, string type, bool status, User user,string customerId)
        {
            await realmDb.AddClaim(hospitalPostcode, patientNr, type, status, user,customerId);
        }

        public async Task<List<Claim>> GetClaims(User user,string customerId)
        {
            Claims = await realmDb.GetClaims(user,customerId);
            return Claims;
        }

        public Claim GetCurrentClaim()
        {
            try
            {
                var aClaim = Claims.FirstOrDefault(claim => claim.CloseDate == null);
                if (aClaim is null) return null;
                return aClaim;
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetCurrentClaim error : {e}");
            }
            return null;
        }

        public async Task ResolveClaim(string customerId, User user)
        {
           await realmDb.ResolveClaim(customerId,user);
        }

        public void Dispose()
        {
            realmDb.Dispose();
            if (Claims != null) Claims = null;
        }

        public async Task<IEnumerable<Claim>> GetAllOpenClaims(User user)
        {
           return await realmDb.GetAllOpenClaims(user);
        }
    }
}