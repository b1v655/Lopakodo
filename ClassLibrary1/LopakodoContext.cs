using System;
using System.Data.Entity;

namespace Lopakodo.Persistence
{
	class LopakodoContext : DbContext
    {
        public LopakodoContext(String connection)
            : base(connection)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Field> Fields { get; set; }
    }
}