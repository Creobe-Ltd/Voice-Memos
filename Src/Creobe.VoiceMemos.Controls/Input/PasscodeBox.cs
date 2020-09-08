using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Creobe.VoiceMemos.Controls
{
    public class PasscodeBox : TextBox
    {
        private string _enteredPasscode = string.Empty;
        private string _passwordChar = "\u25CF";

        public string Passcode
        {
            get { return (string)GetValue(PasscodeProperty); }
            set { SetValue(PasscodeProperty, value); }
        }

        public static readonly DependencyProperty PasscodeProperty =
            DependencyProperty.Register("Passcode", typeof(string), typeof(PasscodeBox), new PropertyMetadata(string.Empty, PasscodeChanged));

        private static void PasscodeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            BindingExpression expression = (obj as PasscodeBox).GetBindingExpression(PasscodeBox.PasscodeProperty);

            if (expression != null)
                expression.UpdateSource();

        }

        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            _enteredPasscode = GetNewPasscode(_enteredPasscode, e);
            this.Passcode = _enteredPasscode;
            this.Text = Regex.Replace(_enteredPasscode, @".", _passwordChar);
            this.SelectionStart = this.Text.Length;
        }

        private string GetNewPasscode(string oldPasscode, KeyEventArgs keyEventArgs)
        {
            string newPasscode = string.Empty;

            switch (keyEventArgs.Key)
            {
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    newPasscode = oldPasscode + (keyEventArgs.PlatformKeyCode - 48);
                    break;
                case Key.Back:
                    if (oldPasscode.Length > 0)
                        newPasscode = oldPasscode.Substring(0, oldPasscode.Length - 1);
                    break;
                default:
                    //others
                    newPasscode = oldPasscode;
                    break;
            }

            return newPasscode;
        }
    }
}
