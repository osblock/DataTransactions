using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using App.Data.DB;

namespace App.Data
{
    public class DataTransContext : IDataContext
    {
        /// <summary>
        /// We asume that DB / Entity Context defined in Data Project, So Data Project need to be referanced App.Data Project
        /// </summary>
        private readonly Data.DB.Entities _db = new Data.DB.Entities();

        public Data.DB.Entities db
        {
            get
            {
                return _db;
            }
        }

        //public int ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
