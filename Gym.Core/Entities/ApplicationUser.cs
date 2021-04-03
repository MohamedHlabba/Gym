using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Entities
{
  public  class ApplicationUser : IdentityUser
    {
        // lägg till egna props sen

        public ICollection<ApplicationUserGymClass>  AttendedClasses{ get; set; }
    }
}
