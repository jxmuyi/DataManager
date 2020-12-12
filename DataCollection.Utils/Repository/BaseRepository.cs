using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace DataCollection.Utils.Repository
{
    public class BaseRepository :IBaseRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        protected SqlSugarClient _dbBase;
        public BaseRepository(SqlSugarClient dbBase)
        {
            _unitOfWork = new UnitOfWork(dbBase);
            _dbBase = _unitOfWork.GetDbClient();
        }


        protected ISqlSugarClient _db<TEntity>() where TEntity:class,new ()
        {

            /* 如果要开启多库支持，
             * 1、在appsettings.json 中开启MutiDBEnabled节点为true，必填
             * 2、设置一个主连接的数据库ID，节点MainDB，对应的连接字符串的Enabled也必须true，必填
             */
            if (Appsettings.app(new string[] { "MutiDBEnabled" }).ObjToBool())
            {
                if (typeof(TEntity).GetTypeInfo().GetCustomAttributes(typeof(SugarTable), true).FirstOrDefault((x => x.GetType() == typeof(SugarTable))) is SugarTable sugarTable && !string.IsNullOrEmpty(sugarTable.TableDescription))
                {
                    _dbBase.ChangeDatabase(sugarTable.TableDescription.ToLower());
                }
                else
                {
                    _dbBase.ChangeDatabase(MainDb.CurrentDbConnId.ToLower());
                }
            }

            return _dbBase;
        }


        #region 存储过程
        

        #endregion

        #region Add


        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<int> Add<TEntity>(TEntity entity) where TEntity : class, new()
        {
            //var i = await Task.Run(() => _db<TEntity>().Insertable(entity).ExecuteReturnBigIdentity());
            ////返回的i是long类型,这里你可以根据你的业务需要进行处理
            //return (int)i;

            var insert = _db<TEntity>().Insertable(entity);

            //这里你可以返回TEntity，这样的话就可以获取id值，无论主键是什么类型
            //var return3 = await insert.ExecuteReturnEntityAsync();

            return await insert.ExecuteReturnIdentityAsync();
        }


        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="insertColumns">指定只插入列</param>
        /// <returns>返回自增量列</returns>
        public async Task<int> Add<TEntity>(TEntity entity, Expression<Func<TEntity, object>> insertColumns = null) where TEntity : class, new()
        {
            var insert = _db<TEntity>().Insertable(entity);
            if (insertColumns == null)
            {
                return await insert.ExecuteReturnIdentityAsync();
            }
            else
            {
                return await insert.InsertColumns(insertColumns).ExecuteReturnIdentityAsync();
            }
        }

        /// <summary>
        /// 批量插入实体(速度快)
        /// </summary>
        /// <param name="listEntity">实体集合</param>
        /// <returns>影响行数</returns>
        public  int Add<TEntity>(List<TEntity> listEntity) where TEntity : class, new()
        {
            return  _db<TEntity>().Insertable(listEntity.ToArray()).ExecuteCommand();
        }

        /// <summary>
        /// 如果不存在就插入,存在返回0
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <param name="whereExpression"></param>
        /// <returns></returns>
        public async Task<int> AddIfNotExists<TEntity>(TEntity model,Expression<Func<TEntity,bool>> whereExpression) where TEntity : class, new()
        {
            var db = _db<TEntity>();
            if(db.Queryable<TEntity>().Where(whereExpression).Any())
            {
                return 0;
            }
            else
            {
                var result = await db.Insertable<TEntity>(model).ExecuteCommandAsync();
                return result;
            }
        }

        

        #endregion

        #region Update

        
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            ////这种方式会以主键为条件
            //var i = await Task.Run(() => _db<TEntity>().Updateable(entity).ExecuteCommand());
            //return i > 0;
            //这种方式会以主键为条件
            return await _db<TEntity>().Updateable(entity).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> Update<TEntity>(TEntity entity, string strWhere) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Updateable(entity).Where(strWhere).ExecuteCommand() > 0);
            return await _db<TEntity>().Updateable(entity).Where(strWhere).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> Update<TEntity>(string strSql, SugarParameter[] parameters = null) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Ado.ExecuteCommand(strSql, parameters) > 0);
            return await _db<TEntity>().Ado.ExecuteCommandAsync(strSql, parameters) > 0;
        }

        public async Task<bool> Update<TEntity>(object operateAnonymousObjects) where TEntity : class, new()
        {
            return await _db<TEntity>().Updateable<TEntity>(operateAnonymousObjects).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update<TEntity>(
          TEntity entity,
          List<string> lstColumns = null,
          List<string> lstIgnoreColumns = null,
          string strWhere = ""
            ) where TEntity : class, new()
        {
            //IUpdateable<TEntity> up = await Task.Run(() => _db<TEntity>().Updateable(entity));
            //if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            //{
            //    up = await Task.Run(() => up.IgnoreColumns(it => lstIgnoreColumns.Contains(it)));
            //}
            //if (lstColumns != null && lstColumns.Count > 0)
            //{
            //    up = await Task.Run(() => up.UpdateColumns(it => lstColumns.Contains(it)));
            //}
            //if (!string.IsNullOrEmpty(strWhere))
            //{
            //    up = await Task.Run(() => up.Where(strWhere));
            //}
            //return await Task.Run(() => up.ExecuteCommand()) > 0;

            IUpdateable<TEntity> up = _db<TEntity>().Updateable(entity);
            if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
            {
                up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
            }
            if (lstColumns != null && lstColumns.Count > 0)
            {
                up = up.UpdateColumns(lstColumns.ToArray());
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                up = up.Where(strWhere);
            }
            return await up.ExecuteCommandHasChangeAsync();
        }

        #endregion

        #region Delete

        
        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">博文实体类</param>
        /// <returns></returns>
        public async Task<bool> Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            //var i = await Task.Run(() => _db<TEntity>().Deleteable(entity).ExecuteCommand());
            //return i > 0;
            return await _db<TEntity>().Deleteable(entity).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteById<TEntity>(object id) where TEntity : class, new()
        {
            //var i = await Task.Run(() => _db<TEntity>().Deleteable<TEntity>(id).ExecuteCommand());
            //return i > 0;
            return await _db<TEntity>().Deleteable<TEntity>(id).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIds<TEntity>(object[] ids) where TEntity : class, new()
        {
            //var i = await Task.Run(() => _db<TEntity>().Deleteable<TEntity>().In(ids).ExecuteCommand());
            //return i > 0;
            return await _db<TEntity>().Deleteable<TEntity>().In(ids).ExecuteCommandHasChangeAsync();
        }
        #endregion

        #region Query

      
        public async Task<TEntity> QueryById<TEntity>(object objId) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().InSingle(objId));
            return await _db<TEntity>().Queryable<TEntity>().In(objId).SingleAsync();
        }
        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById<TEntity>(object objId, bool blnUseCache = false) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().WithCacheIF(blnUseCache).InSingle(objId));
            return await _db<TEntity>().Queryable<TEntity>().WithCacheIF(blnUseCache).In(objId).SingleAsync();
        }

        /// <summary>
        /// 功能描述:根据ID查询数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<TEntity>> QueryByIDs<TEntity>(object[] lstIds) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().In(lstIds).ToList());
            return await _db<TEntity>().Queryable<TEntity>().In(lstIds).ToListAsync();
        }

        /// <summary>
        /// 根据sql语句查询List<T>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryByExecuteSql<TEntity>(string sql) where TEntity : class, new()
        {
            var list = await _db<TEntity>().SqlQueryable<TEntity>(sql).ToListAsync();
            return list;
        }

        /// <summary>
        /// 功能描述:查询所有数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>() where TEntity : class, new()
        {
            return await _db<TEntity>().Queryable<TEntity>().ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(string strWhere) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList());
            return await _db<TEntity>().Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : class, new()
        {
            return await _db<TEntity>().Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToList());
            return await _db<TEntity>().Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(strOrderByFileds != null, strOrderByFileds).ToListAsync();
        }
        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query<TEntity>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).WhereIF(whereExpression != null, whereExpression).ToList());
            return await _db<TEntity>().Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(string strWhere, string strOrderByFileds) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToList());
            return await _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }


        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(
            Expression<Func<TEntity, bool>> whereExpression,
            int intTop,
            string strOrderByFileds) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).Take(intTop).ToList());
            return await _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(
            string strWhere,
            int intTop,
            string strOrderByFileds) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToList());
            return await _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>泛型集合</returns>
        public async Task<List<TEntity>> QueryAdoSql<TEntity>(string strSql, string connid=null, SugarParameter[] parameters = null) where TEntity : class, new()
        {
            if (!string.IsNullOrWhiteSpace(connid)) _dbBase.ChangeDatabase(connid);

            return await _dbBase.Ado.SqlQueryAsync<TEntity>(strSql);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable<TEntity>(string strSql, SugarParameter[] parameters = null) where TEntity : class, new()
        {
            return await _db<TEntity>().Ado.GetDataTableAsync(strSql, parameters);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(
            Expression<Func<TEntity, bool>> whereExpression,
            int intPageIndex,
            int intPageSize,
            string strOrderByFileds) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToPageList(intPageIndex, intPageSize));
            return await _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToPageListAsync(intPageIndex, intPageSize);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// 作　　者:Blog.Core
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="intTotalCount">数据总量</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query<TEntity>(
          string strWhere,
          int intPageIndex,
          int intPageSize,

          string strOrderByFileds) where TEntity : class, new()
        {
            //return await Task.Run(() => _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToPageList(intPageIndex, intPageSize));
            return await _db<TEntity>().Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToPageListAsync(intPageIndex, intPageSize);
        }



      

        /// <summary> 
        ///查询-多表查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam> 
        /// <typeparam name="T2">实体2</typeparam> 
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (join1,join2) => new object[] {JoinType.Left,join1.UserNo==join2.UserNo}</param> 
        /// <param name="selectExpression">返回表达式 (s1, s2) => new { Id =s1.UserNo, Id1 = s2.UserNo}</param>
        /// <param name="whereLambda">查询表达式 (w1, w2) =>w1.UserNo == "")</param> 
        /// <returns>值</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db<T>().Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db<T>().Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }
        #endregion

    }


}

