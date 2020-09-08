using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Creobe.VoiceMemos.Controls
{
    public class FocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            Dispatcher.BeginInvoke(() =>
            {
                AssociatedObject.Focus();
            });
        }
    }
}
