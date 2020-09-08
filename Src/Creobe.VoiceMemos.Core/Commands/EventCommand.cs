using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Creobe.VoiceMemos.Core.Commands
{
    public class EventCommand : TriggerAction<DependencyObject>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommand), new PropertyMetadata(null));

        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject != null)
            {
                if ((Command != null) && Command.CanExecute(parameter))
                {
                    Command.Execute(parameter);
                }
            }
        }
    }
}
