using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.Behaviors
{
    public class PasswordValidatorBehavior : Behavior<Entry>
    {
        const string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(PasswordValidatorBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        static readonly BindablePropertyKey MessageIsValidPropertyKey = BindableProperty.CreateReadOnly("MessageIsValid", typeof(bool), typeof(EmailValidatorBehavior), true);

        public static readonly BindableProperty MessageIsValidProperty = MessageIsValidPropertyKey.BindableProperty;

        public bool MessageIsValid
        {
            get { return (bool)base.GetValue(MessageIsValidProperty); }
            private set { base.SetValue(MessageIsValidPropertyKey, value); }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            IsValid = true;
            entry.Unfocused += Entry_Unfocused;
            entry.TextChanged += entry_TextChanged;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            Entry ent = (Entry)sender;
            if (String.IsNullOrEmpty(ent.Text))
                IsValid = true;
            else
                IsValid = (Regex.IsMatch(ent.Text, passwordRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
        }

        void entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsValid)
            {
                if (String.IsNullOrEmpty(e.NewTextValue))
                    MessageIsValid = true;
                else
                    MessageIsValid = (Regex.IsMatch(e.NewTextValue, passwordRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

                ((Entry)sender).TextColor = MessageIsValid ? Color.Default : Color.Red;
            }
        }

        protected override void OnDetachingFrom(Entry entry)
        {            
            entry.Unfocused -= Entry_Unfocused;
            entry.TextChanged -= entry_TextChanged;
        }
    }
}
