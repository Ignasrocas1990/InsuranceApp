using Realms;

namespace Insurance_app.Models
{
    public class Client : RealmObject
    {
        public Client() { }
        [PrimaryKey] [MapTo("_id")] [Required] public string Id { get; set; } = App.RealmApp.CurrentUser.Id;
        [MapTo("_partition")] [Required] public string Partition { get; set; } = App.RealmApp.CurrentUser.Id;
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyCode { get; set; }
        public bool DelFlag { get; set; } = false;

    }
}