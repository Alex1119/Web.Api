using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.EF
{

    [Serializable,Table("UserInfo", Schema = "CodeFirst")]
    public class UserInfo :BaseEntity
    {
        
        [Key]
        public string UserID { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        public bool Gender { get; set; }

        [Required]
        public int Age { get; set; }

        [NotMapped]
        public string GenderFormat {
            get {
                switch (Gender) {
                    case false : return "女";
                    case true : return "男";
                    default: return "未知";
                }
            }
        }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public DateTime LastUpdateTime { get; set; }


    }
}
