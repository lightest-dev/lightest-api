using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using Lightest.Data.Mongo.Interfaces;
//using Microsoft.IdentityModel.Protocols;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Lightest.Data.Mongo
{
    public class MongoDbRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase  
    {  
        private MongoCollection<TEntity> collection;  
       
  
        public bool Insert(TEntity entity)  
        {  
            entity.Id = Guid.NewGuid();
            return (collection.Insert(entity)).UpdatedExisting;  
        }  
  
        public bool Update(TEntity entity)  
        {  
            if (entity.Id == null)  
                return Insert(entity);  
  
            return collection  
                       .Save(entity)  
                       .DocumentsAffected > 0;  
        }  
  
        public bool Delete(TEntity entity)  
        {  
            return collection  
                .Remove(Query.EQ("_id", entity.Id)).DocumentsAffected > 0;  
        }

        #region Private Helper Methods

        private string GetConnectionString()  
        {  
            return ConfigurationManager.AppSettings.Get("MongoDbConnectionString").Replace("{DB_NAME}", GetDatabaseName());   
        }  
  
        private string GetDatabaseName()  
        {  
            return ConfigurationManager.AppSettings.Get("MongoDbDatabaseName");  
        }  
        
        #endregion  
    }  
}
