﻿
using Minifutbol.DAL.Repository;
using System;
using log4net;
using System.Data.Entity;
using Minifutbol.DAL.Context;
using System.Data.Entity.Validation;

namespace Minifutbol.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public MinifutbolContext MinifutbolContext;
        public static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DbContextTransaction transaction;
        private static UnitOfWork uow { get; set; }
        private bool _disposed;

        public UnitOfWork()
        {
            MinifutbolContext = new MinifutbolContext();
            _disposed = false;
        }
        //public static UnitOfWork GetInstance()
        //{
        //    if(uow == null)
        //    {
        //        uow = new UnitOfWork();
        //    }
        //    return uow;
        //}
        public void RollBack()
        {
            transaction.Rollback();
        }

        public int SaveChanges()
        {

            try
            {
                return MinifutbolContext.SaveChanges();
            }
            catch (DbEntityValidationException devx)
            {
                Logger.Error("Db Entity Validation Exception Occured: ", devx);

                foreach (var error in devx.EntityValidationErrors)
                {
                    Logger.Error(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", error.Entry.Entity.GetType().Name, error.Entry.State));

                    foreach (var ve in error.ValidationErrors)
                    {
                        Logger.Error(string.Format("Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }

                throw;
            }
            //try
            //{
            //    return MinifutbolContext.SaveChanges();

            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

        }
        //public async Task<int> SaveChangesAsync()
        //{
        //    return await _context.SaveChangesAsync();
        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    MinifutbolContext.Dispose();
                }

                this._disposed = true;
                uow = null;
            }
        }

        public void BeginTransaction()
        {
            transaction = MinifutbolContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(MinifutbolContext);
        }
    }
}
