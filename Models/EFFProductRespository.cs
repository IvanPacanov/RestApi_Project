using RestApi_Dicom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi_Dicom.Models
{
    public class EFFProductRespository : ICommanderRepo
    {

        private CommanderContext commander;
        public EFFProductRespository(CommanderContext commander)
        {
           this.commander = commander;
        }

        public IEnumerable<Command> GetAppCommands => commander.Commands;

        public Command GetCommandById(int Id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Command> ICommanderRepo.GetAllCommands()
        {
            return commander.Commands;
        }
    }
}
