using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace Lightest.Data.Mongo.Interfaces
{
    public interface IRepository<TEntity> where TEntity : EntityBase  
    {  
        bool Insert(TEntity entity);  
        bool Update(TEntity entity);  
        bool Delete(TEntity entity);  
    }  
}
