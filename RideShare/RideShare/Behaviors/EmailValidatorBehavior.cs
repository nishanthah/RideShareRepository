using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.Behaviors
{
    public class EmailValidatorBehavior : Behavior<Entry>
    {
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EmailValidatorBehavior), false);

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
                IsValid = (Regex.IsMatch(ent.Text, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
        }

        void entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsValid)
            {
                if (String.IsNullOrEmpty(e.NewTextValue))
                    MessageIsValid = true;
                else
                    MessageIsValid = (Regex.IsMatch(e.NewTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

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
