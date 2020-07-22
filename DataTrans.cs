using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Data
{
    public class DbTransException
    {
        public DbTransException()
        {
            Exception = new List<DbTransExceptionError>();
        }
        public string Type { get; set; }
        public string Lable { get; set; }
        public List<DbTransExceptionError> Exception { get; set; }
    }

    public class DbTransExceptionError
    {
        public string Message { get; set; }
        public string Reference { get; set; }
    }

    public class DbTrans<T> : DataTransContext where T : class
    {
        public DbTrans()
        {
            Exception = new List<DbTransException>();
        }

        public List<DbTransException> Exception { get; set; }

        private void SetExcepton(System.Data.Entity.Validation.DbEntityValidationException ex)
        {
            foreach (System.Data.Entity.Validation.DbEntityValidationResult vr in ex.EntityValidationErrors)
            {
                var dte = new DbTransException();
                dte.Type = "DbEntityValidationException";
                dte.Lable = vr.Entry.Entity.ToString();

                foreach (System.Data.Entity.Validation.DbValidationError ve in vr.ValidationErrors)
                {
                    dte.Exception.Add(new DbTransExceptionError
                    {
                        Message = ve.ErrorMessage,
                        Reference = ve.PropertyName
                    });
                }
                Exception.Add(dte);
            }
        }

        public bool Create(T Entity)
        {
            try
            {
                db.Set<T>().Add(Entity);
                db.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                SetExcepton(ex);
                return false;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {

                // TODO : Need to re-evaluate, How clectively handel exceptions 
                // Related to Entity
                var dte = new DbTransException();
                //dte.Type = "OracleException";
                dte.Lable = ""; //ex.Entries.;

                dte.Exception.Add(new DbTransExceptionError
                {
                    Message = ex.InnerException.InnerException.Message,
                    Reference = ""
                });

                Exception.Add(dte);

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }
        }
        public bool CreateRange(List<T> Entities)
        {
            try
            {
                foreach (var Entity in Entities)
                {
                    db.Set<T>().Add(Entity);
                }
                db.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                SetExcepton(ex);
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Update(T Entity)
        {
            if (Entity == null) throw new ArgumentNullException(nameof(Entity), $"The Parameter Update Entity can not be null");
            var result = 0;
            try
            {
                using (var context = db)
                {
                    var dbSet = context.Set<T>();
                    dbSet.Attach(Entity);
                    context.Entry(Entity).State = EntityState.Modified;
                    result = context.SaveChanges();
                }
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                SetExcepton(ex);
                return false;
            }

            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return result;
        }
        public Task<bool> UpdateAsync(T Entity)
        {
            return Task.Run(() =>
            {
                return Update(Entity);
            });
        }

        public bool Delete(T Entity)
        {
            try
            {
                db.Set<T>().Remove(Entity);
                db.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                SetExcepton(ex);
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteRange(IEnumerable<T> Entities)
        {
            try
            {
                db.Set<T>().RemoveRange(Entities);
                db.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                SetExcepton(ex);
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            try
            {
                DeleteRange(GetData(predicate));
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                SetExcepton(ex);
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T GetByCode(string Code)
        {
            var data = db.Set<T>().Find(Code);
            return data;
        }

        public IEnumerable<T> GetData(Expression<Func<T, bool>> predicate)
        {
            var data = db.Set<T>().Where(predicate).ToList();
            return data;
        }

        public List<T> GetData()
        {
            List<T> data = (from e in db.Set<T>() select e).ToList<T>();
            return data;
        }

    }
}
