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

        protected override void OnAttachedTo(Entry entry)
        {
            IsValid = true;
            entry.Unfocused += Entry_Unfocused;            
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

        protected override void OnDetachingFrom(Entry entry)
        {            
            entry.Unfocused -= Entry_Unfocused;
        }
    }
}
