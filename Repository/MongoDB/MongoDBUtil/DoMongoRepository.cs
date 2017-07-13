﻿using Entity.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MongoDB
{
    public class DoMongoRepoistory<TAggregate> : DoMongoBase
                where TAggregate : AggregateBase
    {
        #region 初始化及字段属性设置
        /// <summary>
        /// 获取集合
        /// </summary>
        public IMongoCollection<TAggregate> Collection;

        public DoMongoRepoistory(string dbSelectionKey, string collectionName)
        {
            this.Collection = ShareMongoDb(dbSelectionKey).GetCollection<TAggregate>(collectionName);
        }

        private List<WriteModel<TAggregate>> writers = new List<WriteModel<TAggregate>>();//写入模型

        /// <summary>
        /// 指示是否起用事务,默认true
        /// </summary>
        public bool IsUseTransaction { get; set; }

        private List<TAggregate> beforeChange = new List<TAggregate>();//记录更新前的数据
        private List<Guid> beforeAdd = new List<Guid>();   //记录添加前的数据ID
        private List<TAggregate> beforeDelete = new List<TAggregate>();//记录数据删除前的数据


        private bool isRollback = false;//回滚控制 
        #endregion

        #region 添加
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="entity"></param>
        public void Add(TAggregate entity)
        {
            if (entity == null)
                return;
            if (IsUseTransaction)
            {
                try
                {
                    beforeAdd.Add(entity.ID);//记录添加之前的数据的ID
                    writers.Add(new InsertOneModel<TAggregate>(entity));
                    isRollback = false;//控制是否回滚
                    return;
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                Collection.InsertOne(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 添加数据集合
        /// </summary>
        /// <param name="entities"></param>
        public void Add(IEnumerable<TAggregate> entities)
        {
            if (entities.Count() <= 0)
                return;
            if (IsUseTransaction)
            {
                try
                {
                    entities.ToList().ForEach(o =>
                    {
                        beforeAdd.Add(o.ID);
                        writers.Add(new InsertOneModel<TAggregate>(o));
                    });
                    isRollback = false;
                    return;
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                Collection.InsertMany(entities);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 替换
        /// <summary>
        /// 替换一条过滤的数据(请确保此方法Id属性是不能变)
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="enity">目标数据(目标数据的Id值必为源数据的Id)</param>
        public void ReplaceOne(Expression<Func<TAggregate, bool>> filter, TAggregate enity)
        {
            if (enity == null)
                return;
            if (IsUseTransaction)
            {
                try
                {
                    //先记录修改之前的数据
                    beforeChange.Add(Collection.Find(Builders<TAggregate>.Filter.Where(filter)).FirstOrDefault());
                    writers.Add(new ReplaceOneModel<TAggregate>(Builders<TAggregate>.Filter.Where(filter), enity));
                    isRollback = false;
                    return;
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }

            try
            {
                Collection.ReplaceOne(filter, enity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 替换一条数据(请确保此方法Id属性是不能变)
        /// </summary>
        /// <param name="id">目标id</param>
        /// <param name="enity">目标数据(目标数据的Id值必为源数据的Id)</param>
        public void ReplaceById(Guid id, TAggregate enity)
        {
            if (enity == null)
                return;
            if (enity.ID != id)
            {
                isRollback = true;
                throw new Exception("the id can not change");
            }
            if (IsUseTransaction)
            {
                try
                {
                    beforeChange.Add(Collection.Find(Builders<TAggregate>.Filter.Eq(o => o.ID, id)).FirstOrDefault());
                    writers.Add(new ReplaceOneModel<TAggregate>(Builders<TAggregate>.Filter.Eq(o => o.ID, id), enity));
                    isRollback = false;
                    return;
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                Collection.ReplaceOne(o => o.ID == id, enity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 查找一条数据并且替换
        /// </summary>
        /// <param name="id">目标数据的id</param>
        /// <param name="enity">更改后的数据</param>
        /// <returns>更改前的数据</returns>
        public TAggregate FindOneAndReplace(Guid id, TAggregate enity)
        {
            if (enity == null)
                return null;
            if (enity.ID != id)
            {
                throw new Exception("the id can not change");
            }

            return Collection.FindOneAndReplace(o => o.ID == id, enity);
        }
        /// <summary>
        /// 查找一条数据并且替换
        /// </summary>
        /// <param name="filter">条件</param>
        /// <param name="enity">更改后的数据</param>
        /// <returns>更改前的数据</returns>
        public TAggregate FindOneAndReplace(Expression<Func<TAggregate, bool>> filter, TAggregate enity)
        {
            if (enity == null)
                return null;
            return Collection.FindOneAndReplace(filter, enity);
        }

        #endregion

        #region 移除
        /// <summary>
        /// 根据过滤删除数据
        /// </summary>
        /// <param name="filter"></param>
        public void Remoe(Expression<Func<TAggregate, bool>> filter)
        {
            if (IsUseTransaction)
            {
                try
                {
                    if (Collection.Find(filter).FirstOrDefault() == null)//如果要删除的数据不存在数据库中
                    {
                        throw new Exception("要删除的数据不存在数据库中");
                    }
                    beforeDelete.Add(Collection.Find(filter).FirstOrDefault());
                    writers.Add(new DeleteOneModel<TAggregate>(Builders<TAggregate>.Filter.Where(filter)));
                    isRollback = false;
                    return;
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                Collection.DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void RemoveById(Guid id)
        {
            if (IsUseTransaction)
            {
                try
                {
                    beforeDelete.Add(Collection.Find(Builders<TAggregate>.Filter.Eq(o => o.ID, id)).FirstOrDefault());
                    writers.Add(new DeleteOneModel<TAggregate>(Builders<TAggregate>.Filter.Eq(o => o.ID, id)));
                    isRollback = false;
                    return;
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                Collection.DeleteOne(o => o.ID == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 更新
        /// <summary>
        /// 过滤数据，执行更新操作(如不便使用，请用Replace相关的方法代替)
        /// 
        /// 一般用replace来代替这个方法。其实这个功能还算强大的，可以很自由修改多个属性
        /// 关健是set参数比较不好配置，并且如果用此方法，调用端必须引用相关的DLL，set举例如下
        /// set = Builders<TAggregate>.Update.Update.Set(o => o.Number, 1).Set(o => o.Description, "002.thml");
        /// set作用：将指定TAggregate类型的实例对象的Number属性值更改为1，Description属性值改为"002.thml"
        /// 说明：Builders<TAggregate>.Update返回类型为UpdateDefinitionBuilder<TAggregate>，这个类有很多静态
        /// 方法，Set()是其中一个，要求传入一个func的表达示，以指示当前要修改的，TAggregate类型中的属性类型，
        /// 另一个参数就是这个属性的值。
        /// 
        /// Builders<TAggregate>类有很多属性，返回很多如UpdateDefinitionBuilder<TAggregate>的很有用帮助类型
        /// 可以能参CSharpDriver-2.2.3.chm文件 下载MongoDB-CSharpDriver时带有些文件 
        /// 或从官网https://docs.mongodb.com/ecosystem/drivers/csharp/看看
        /// 
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="set">修改设置</param>
        public void Update(Expression<Func<TAggregate, bool>> filter, UpdateDefinition<TAggregate> set)
        {
            if (set == null)
                return;
            if (IsUseTransaction)//如果启用事务
            {
                try
                {
                    beforeChange.Add(Collection.Find(filter).FirstOrDefault());
                    writers.Add(new UpdateManyModel<TAggregate>(Builders<TAggregate>.Filter.Where(filter), set));
                    isRollback = false;//不回滚
                    return;//不执行后继操作
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                Collection.UpdateMany(filter, set);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 过滤数据，执行更新操作(如不便使用，请用Replace相关的方法代替)
        /// 
        /// 一般用replace来代替这个方法。其实这个功能还算强大的，可以很自由修改多个属性
        /// 关健是set参数比较不好配置，并且如果用此方法，调用端必须引用相关的DLL，set举例如下
        /// set = Builders<TAggregate>.Update.Update.Set(o => o.Number, 1).Set(o => o.Description, "002.thml");
        /// set作用：将指定TAggregate类型的实例对象的Number属性值更改为1，Description属性值改为"002.thml"
        /// 说明：Builders<TAggregate>.Update返回类型为UpdateDefinitionBuilder<TAggregate>，这个类有很多静态
        /// 方法，Set()是其中一个，要求传入一个func的表达示，以指示当前要修改的，TAggregate类型中的属性类型，
        /// 另一个参数就是这个属性的值。
        /// 
        /// Builders<TAggregate>类有很多属性，返回很多如UpdateDefinitionBuilder<TAggregate>的很有用帮助类型
        /// 可以能参CSharpDriver-2.2.3.chm文件 下载MongoDB-CSharpDriver时带有些文件 
        /// 或从官网https://docs.mongodb.com/ecosystem/drivers/csharp/看看
        /// 
        /// </summary>
        /// <param name="id">找出指定的id数据</param>
        /// <param name="set">修改设置</param>
        public void Update(Guid id, UpdateDefinition<TAggregate> set)
        {
            if (set == null)
                return;
            if (IsUseTransaction)//如果启用事务
            {
                try
                {
                    beforeChange.Add(Collection.Find(Builders<TAggregate>.Filter.Eq(o => o.ID, id)).FirstOrDefault());
                    writers.Add(new UpdateManyModel<TAggregate>(Builders<TAggregate>.Filter.Eq(o => o.ID, id), set));
                    isRollback = false;//不回滚
                    return;//不执行后继操作
                }
                catch (Exception ex)
                {
                    isRollback = true;
                    throw new Exception(ex.Message);
                }
            }
            try
            {
                Collection.UpdateMany(o => o.ID == id, set);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 事务控制
        public void Commit()
        {
            if (!isRollback && writers.Count > 0)//如果不回滚，并且writers有数据
            {
                BulkWriteResult<TAggregate> result;
                try
                {
                    result = Collection.BulkWrite(writers);
                }
                catch (Exception)
                {
                    Rollback();//若BulkWriteResult发生异常
                    throw;
                }
                if (result.ProcessedRequests.Count != writers.Count)//检查完成写入的数量，如果有误，回滚
                {
                    Rollback();
                }
                writers.Clear();//此时说明已成功提交，清空writers数据
                return;
            }
            Rollback();
        }
        public void Rollback()
        {
            writers.Clear();//清空writers
            //执行反操作
            beforeDelete.ForEach(o =>
            {
                Collection.InsertOne(o);
            });
            beforeChange.ForEach(o =>
            {
                Collection.ReplaceOne(c => c.ID == o.ID, o);
            });
            beforeAdd.ForEach(o =>
            {
                Collection.DeleteOne(d => d.ID == o);
            });

        }
        #endregion

        #region 查询
        /// <summary>
        /// 查找所有数据集合
        /// </summary>
        /// <returns></returns>
        public IQueryable<TAggregate> FindAll()
        {
            return Collection.AsQueryable();
        }

        /// <summary>
        /// 根据Id查找一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TAggregate FindById(Guid id)
        {
            var find = Collection.Find(o => o.ID == id);
            if (!find.Any())
                return null;
            return find.FirstOrDefault();
        }

        /// <summary>
        /// 根据过滤条件找出符合条件的集合
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<TAggregate> FindByFilter(Expression<Func<TAggregate, bool>> filter)
        {
            var find = Collection.Find(filter);
            if (!find.Any())
                return new List<TAggregate>();
            return find.ToList();
        }

        /// <summary>
        /// 根据过滤条件找出一条数据
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TAggregate FindOne(Expression<Func<TAggregate, bool>> filter)
        {
            return Collection.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// 根据过滤条件分页获取数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageIndex">从1开始记页</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<TAggregate> FindPagerList(Expression<Func<TAggregate, bool>> filter, SortDefinition<TAggregate> sortFilter, int pageIndex, int pageSize)
        {
            var find = Collection.Find(filter).Sort(sortFilter).Skip((pageIndex - 1) * pageSize).Limit(pageSize);
            if (!find.Any())
                return new List<TAggregate>();
            return find.ToList();
        }

        public int Count(Expression<Func<TAggregate, bool>> filter)
        {
            var count = Collection.Count(filter);
            return (int)count;
        }

        #endregion

        /// <summary>
        /// 根据聚合类ID添加导航数据到 导航集合（中间表）
        /// </summary>
        /// <typeparam name="TNav">导航类</typeparam>
        /// <param name="nav">提供参数时直接new一个具体的nav类就行了</param>
        /// <param name="filter"></param>
        /// <param name="foreignKey"></param>
        //public void AddByAggregate<TNav>(string dbSelectionKey, TNav nav, Expression<Func<TAggregate, bool>> filter, Guid foreignKey)
        //                                where TNav : NavgationBase
        //{
        //    //导航类的集合
        //    var navCollection = ShareMongoDb(dbSelectionKey).GetCollection<TNav>(typeof(TNav).Name);
        //    //遍历当前集合中所有符合条件的数据
        //    Collection.Find(filter).ToList().ForEach(o =>
        //    {
        //        //将导航类的属性赋相应的值
        //        nav.AggregateId = foreignKey;
        //        nav.ValueObjectId = o.Id;

        //        //插入到数据库
        //        navCollection.InsertOne(nav);
        //    });
        //}


    }
}
