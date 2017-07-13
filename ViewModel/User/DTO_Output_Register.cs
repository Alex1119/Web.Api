using AutoMapper;
using Entity.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.User
{
    public class DTO_Output_Register : BaseOutputModel
    {

        public string UserID { get; set; }
        public string UserName { get; set; }
        public Nullable<bool> Gender { get; set; }
        public Nullable<int> Age { get; set; }
        public System.DateTime CreateTime { get; set; }

        public DTO_Output_Register ConvertFrom(UserInfo dataModel) {
            Mapper.Initialize((config) => {
                config.CreateMap<UserInfo, DTO_Output_Register>();
            });
            return Mapper.Map<UserInfo, DTO_Output_Register>(dataModel);
        }
    }
}
