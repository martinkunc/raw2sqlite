using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raw2sqlite
{
    public class RawDbInitializer : SqliteDropCreateDatabaseAlways<RawContext>
    {
        public RawDbInitializer(DbModelBuilder modelBuilder)
            : base(modelBuilder)
        { }

        protected override void Seed(RawContext context)
        {
            // Here you can seed your core data if you have any.
        }
    }
}

