using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure
{
    public class IhelpDbLiveContext : DbContext
    {
        public IhelpDbLiveContext()
        {
        }

        public IhelpDbLiveContext(DbContextOptions<IhelpDbLiveContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }

        }
}
