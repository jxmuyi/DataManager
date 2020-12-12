using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataCollection.Utils.Repository
{
    public interface IBaseRepository 
    {
        Task<TEntity> QueryById<TEntity>(object objId) where TEntity : class, new();
        Task<TEntity> QueryById<TEntity>(object objId, bool blnUseCache = false) where TEntity : class, new();
        Task<List<TEntity>> QueryByIDs<TEntity>(object[] lstIds) where TEntity : class, new();
        Task<List<T>> QueryByExecuteSql<T>(string sql) where T : class, new();
        Task<int> Add<TEntity>(TEntity model) where TEntity : class, new();
        Task<int> AddIfNotExists<TEntity>(TEntity model, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class, new();
        int Add<TEntity>(List<TEntity> listEntity) where TEntity : class, new();

        Task<bool> DeleteById<TEntity>(object id) where TEntity : class, new();

        Task<bool> Delete<TEntity>(TEntity model) where TEntity : class, new();

        Task<bool> DeleteByIds<TEntity>(object[] ids) where TEntity : class, new();

        Task<bool> Update<TEntity>(TEntity model) where TEntity : class, new();
        Task<bool> Update<TEntity>(TEntity entity, string strWhere) where TEntity : class, new();
        Task<bool> Update<TEntity>(object operateAnonymousObjects) where TEntity : class, new();

        Task<bool> Update<TEntity>(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string strWhere = "") where TEntity : class, new();

        Task<List<TEntity>> Query<TEntity>() where TEntity : class, new();
        Task<List<TEntity>> Query<TEntity>(string strWhere) where TEntity : class, new();
        Task<List<TEntity>> Query<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : class, new();
        Task<List<TEntity>> Query<TEntity>(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds) where TEntity : class, new();
        Task<List<TEntity>> Query<TEntity>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true) where TEntity : class, new();
        Task<List<TEntity>> Query<TEntity>(string strWhere, string strOrderByFileds) where TEntity : class, new();

        Task<List<TEntity>> Query<TEntity>(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds) where TEntity : class, new();
        Task<List<TEntity>> Query<TEntity>(string strWhere, int intTop, string strOrderByFileds) where TEntity : class, new();
        Task<List<TEntity>> QueryAdoSql<TEntity>(string strSql, string connid=null,SugarParameter[] parameters = null) where TEntity : class, new();
        Task<DataTable> QueryTable<TEntity>(string strSql, SugarParameter[] parameters = null) where TEntity : class, new();

        Task<List<TEntity>> Query<TEntity>(
            Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds) where TEntity : class, new();
        Task<List<TEntity>> Query<TEntity>(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds) where TEntity : class, new();

        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
    }
}
