using System.Collections.Generic;
using System.Linq;
using RestApi_Dicom.Models;

namespace RestApi_Dicom.Data
{
    public class SqlCommanderRepo : ICommanderRepo
    {
        private readonly CommanderContext _context;

        public SqlCommanderRepo(CommanderContext context)
        {
            _context = context;
        }
        public IEnumerable<Command> GetAllCommands()
        {
            return _context.Commands.ToList();     
        }

        public Command GetCommandById(int Id)
        {
            return _context.Commands.FirstOrDefault(p=>p.Id==Id);
        }
    }
}