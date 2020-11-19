using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi_Dicom.Data
{
    public class CommanderFactory : IDesignTimeDbContextFactory<CommanderContext>
    {
        public CommanderContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommanderContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLServerek;Initial Catalog=Dicom_DB; User ID=sa; Password=kolarz9;");

            return new CommanderContext(optionsBuilder.Options);
        }
    }
}