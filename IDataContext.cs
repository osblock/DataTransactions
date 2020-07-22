using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using App.Data.DB;

namespace App.Data
{
    public interface IDataContext
    {
        /// <summary>
        ///  We need to update this with the Entity DB Context, with respective to Project
        /// </summary>
        Data.DB.Entities db {
            get;
        }
    }
    //public interface IEntity
    //{
    //    int ID { get; set; }
    //}

}
