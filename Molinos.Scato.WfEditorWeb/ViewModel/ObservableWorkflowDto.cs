using System.ComponentModel;
using System.Runtime.CompilerServices;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.WfEditorWeb.ViewModel
{
    public class ObservableWorkflowDto : INotifyPropertyChanged
    {
        private readonly WorkflowDto workflow;

        public ObservableWorkflowDto(WorkflowDto workflow)
        {
            this.workflow = workflow;
        }

        public int Id
        {
            get { return workflow.Id; }
            set
            {
                workflow.Id = value;
                OnPropertyChanged();
            }
        }

        public string Codigo
        {
            get { return workflow.Codigo; } 
            set
            {
                workflow.Codigo = value;
                OnPropertyChanged();
            }
        }

        public string Descripcion
        {
            get { return workflow.Descripcion; }
            set
            {
                workflow.Descripcion = value;
                OnPropertyChanged();
            }
        }

        public bool Activo
        {
            get { return workflow.Activo; }
            set
            {
                workflow.Activo = value;
                OnPropertyChanged();
            }
        }

        public TipoDeWorkflow TipoDeWorkflow
        {
            get { return workflow.TipoDeWorkflow; }
            set
            {
                workflow.TipoDeWorkflow = value;
                OnPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
