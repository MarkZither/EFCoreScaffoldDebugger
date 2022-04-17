using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;

#nullable disable

namespace EFCoreScaffoldDebugger.Console
{
    public class TestContext : DbContext
        {
        private readonly string _connString;
            public TestContext(){
            var x = "";
        }
        public TestContext(string connString)
        {
            _connString = connString;
        }
        public TestContext(DbContextOptions options)
                : base(options){
            var y = "";
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connString, opt => opt.CommandTimeout(150000));
        }
    }
}