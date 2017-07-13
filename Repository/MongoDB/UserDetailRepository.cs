using Entity.MongoDB;
using IRepository.cs.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MongoDB
{
    public class UserDetailRepository : IUserDetailRepository
    {
        #region
        public const string MONGODB_CONFIG_SELECTION_KEY = "MongoConfig";
        public const string USER_DETAIL_COLLECTION_NAME = "USER_DETAIL";

        private DoMongoRepoistory<UserDetailEntity> _UserRepostory
        {
            get { return new DoMongoRepoistory<UserDetailEntity>(MONGODB_CONFIG_SELECTION_KEY, USER_DETAIL_COLLECTION_NAME); }
        }
        #endregion

        #region

        public int Counts()
        {
            return _UserRepostory.Count(p => true);
        }


        public bool Add(string UserID, string UserName) {
            try
            {
                Guid ID = Guid.Parse(UserID);
                _UserRepostory.Add(new UserDetailEntity
                {
                    UserId = ID,
                    UserName = UserName
                });
                return true;
            } catch (Exception ex) {
                return false;
            }
        }
        #endregion

    }
}
