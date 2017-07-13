using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.User
{
    public class DTO_Input_Register : BaseInputModel
    {
        public string UserName { get; set; }

        public bool? Gender { get; set; }

        public int? Age { get; set; }
    }
}
