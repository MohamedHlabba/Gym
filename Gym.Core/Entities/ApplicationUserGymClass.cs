using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Entities
{
 public    class ApplicationUserGymClass
    {
        public int GymClassId { get; set; }
        public string ApplicationUserId { get; set; }

        //NavigationPropp
        public GymClass GymClass { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
