﻿using System;
using System.Collections.Generic;
using System.Linq;

using BlueBoxMoon.Data.EntityFramework;
using BlueBoxMoon.Data.EntityFramework.Migrations;
using BlueBoxMoon.Data.EntityFramework.Sqlite;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Console.Runner
{
    class Program
    {
        static void Main( string[] args )
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging( a => a.AddConsole() )
                .AddDbContext<DatabaseContext>( options =>
                {
                    options.UseSqlite( "Data Source=database.db" );
                    options.UseEntityDbContext( o => o.UseSqlite() );
                } )
                .BuildServiceProvider();

            var ctx = serviceProvider.GetService<DatabaseContext>();
            ctx.Database.Migrate();

            var plugin = new EntityPlugin( "com.blueboxmoon.test", new List<Type> { typeof( TestMigration ) } );
            ctx.Database.MigratePlugin( plugin );

            ctx.People.Add( new Person { FirstName = "Daniel", LastName = Guid.NewGuid().ToString() } );
            ctx.SaveChanges();

            var list = ctx.People.AsQueryable().ToList();
        }
    }

    [Migration( "20200112_Initial" )]
    public class TestMigration : EntityMigration
    {
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.CreateEntityTable( "TestPlugin",
                table => new
                {
                    Value = table.Column<string>()
                } );
        }

        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropTable( "TestPlugin" );
        }
    }
}
