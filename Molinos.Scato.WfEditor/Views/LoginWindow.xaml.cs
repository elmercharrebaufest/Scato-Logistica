using System;
using System.DirectoryServices.AccountManagement;
using System.Windows;

namespace Molinos.Scato.WfEditor.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly string dominio;

        public LoginWindow(string titulo, string dominio)
        {
            InitializeComponent();
            this.dominio = dominio;
            Title = titulo;
        }

        private void IngresarClick(object sender, RoutedEventArgs e)
        {               
            try
            {
                var usuario = Usuario.Text;
                var password = Password.Password;
                UsuarioEditor.UsuarioActual = usuario;

                //if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(dominio))
                //{
                //    using (var context = new PrincipalContext(ContextType.Domain, dominio))
                //    {
                //        if (context.ValidateCredentials(usuario, password))
                //        {
                //            UsuarioEditor.UsuarioActual = usuario;
                //        }
                //        else
                //        {
                //            MessageBox.Show(Properties.Resources.LoginVerifiqueDatos, Properties.Resources.ErrorMensaje, MessageBoxButton.OK, MessageBoxImage.Warning);  
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                UsuarioEditor.UsuarioActual = null;
                MessageBox.Show(Properties.Resources.LoginError + ex.Message, Properties.Resources.ErrorMensaje, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (UsuarioEditor.UsuarioActual != null)
            {
                Close();
            }
        }

        private void CancelarClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
