using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Models
{
    public class ManageClientVm
    {
        public string ClientId { get; set; }     
        public string ClientName { get; set; }        
        public string ClientUri { get; set; }     
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string Description { get; set; }
        public ICollection<ClientResourceVm> ClientResources { get; set; }
    }
}
