using RestApi_Dicom.Models;
using System.Collections.Generic;

namespace RestApi_Dicom.Data
{
 public interface ICommanderRepo
    {
        IEnumerable<Command> GetAllCommands();
        Command GetCommandById(int Id);
        void AddToList(Command imageCreate);

    }
}