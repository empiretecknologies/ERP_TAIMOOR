using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class NotesService : INotesService
    {

        public INotesRepository _notesRepository { get; set; }
        public NotesService(INotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _notesRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Notes model, Common common)
        {
            return _notesRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _notesRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetNotesById(int id, Common common)
        {
            return _notesRepository.GetNotesById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _notesRepository.Delete(id, common);
        }
    }
}
