using System.Activities.Presentation.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Molinos.Scato.WfEditor.Ejecucion
{
    public class ValidationErrorService : IValidationErrorService
    {
        private readonly ObservableCollection<ValidationErrorInfo> listaErrores;

        public ValidationErrorService()
        {
            this.listaErrores = new ObservableCollection<ValidationErrorInfo>(); ;
        }

        public ObservableCollection<ValidationErrorInfo> ListaErrores
        {
            get { return listaErrores; }
        }

        public void ShowValidationErrors(IList<ValidationErrorInfo> errors)
        {
            listaErrores.Clear();
            foreach (var error in errors)
            {
                listaErrores.Add(error);
            }
        }

        public void ClearValidationErrors()
        {
            listaErrores.Clear();
        }
    }
}
