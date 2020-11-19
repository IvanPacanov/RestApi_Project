using RestApi_Dicom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi_Dicom.Data
{
    public class MockCommanderRepo : ICommanderRepo
    {
        public IEnumerable<Command> GetAllCommands()
        {
            var commands = new List<Command>
            {
                new Command { Id = 1, HowTo = "Boil an egg", Line = "Boil water", Platform = "Surfing & Pacan1" },
                new Command { Id = 1, HowTo = "Cut bread", Line = "Boil water", Platform = "Surfing & Pacan2" },
                new Command { Id = 1, HowTo = "Make cup of tea", Line = "Boil water", Platform = "Surfing & Pacan3" }
            };
            return commands;
        }

        public Command GetCommandById(int Id)
        {
            return new Command { Id = 1, HowTo = "Boil an egg", Line = "Boil water", Platform = "Surfing & Pacan" };
        }
    }
}
