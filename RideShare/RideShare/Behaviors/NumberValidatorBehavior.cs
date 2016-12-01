using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.Behaviors
{
    public class NumberValidatorBehavior : Behavior<Entry>
    {

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(NumberValidatorBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            IsValid = true;
            entry.TextChanged += HandleTextChanged;
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (String.IsNullOrEmpty(e.NewTextValue))
                IsValid = true;
            else
                IsValid = int.TryParse(e.NewTextValue, out result);
            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;            
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= HandleTextChanged;
        }

        
    }

    public class MobileNumberValidatorBehavior : Behavior<Entry>
    {
        const string mobileNumberRegex = @"^0[7][0-9]{8}$";

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(MobileNumberValidatorBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        static readonly BindablePropertyKey MessageIsValidPropertyKey = BindableProperty.CreateReadOnly("MessageIsValid", typeof(bool), typeof(MobileNumberValidatorBehavior), true);

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

        void entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!IsValid)
            {
                if (String.IsNullOrEmpty(e.NewTextValue))
                    MessageIsValid = true;
                else
                    MessageIsValid = (Regex.IsMatch(e.NewTextValue, mobileNumberRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

                ((Entry)sender).TextColor = MessageIsValid ? Color.Default : Color.Red;
            }
        }

        void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            Entry ent = (Entry)sender;
            if (String.IsNullOrEmpty(ent.Text))
                IsValid = true;
            else
                MessageIsValid = IsValid = (Regex.IsMatch(ent.Text, mobileNumberRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            ((Entry)sender).TextColor = IsValid ? Color.Default : Color.Red;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.Unfocused -= Entry_Unfocused;
            entry.TextChanged -= entry_TextChanged;
        }


    }

    public class CountryCodeValidatorForPickerBehavior : Behavior<Picker>
    {
        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EmailValidatorBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        protected override void OnAttachedTo(Picker picker)
        {
            IsValid = true;
            picker.Unfocused += picker_Unfocused;
            picker.SelectedIndexChanged += picker_SelectedIndexChanged;
        }

        void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker ent = (Picker)sender;
            
            if (ent.SelectedIndex <= 0)
                IsValid = false;
            else
                IsValid = true;
            ((Picker)sender).BackgroundColor = IsValid ? Color.Default : Color.Red;
        }

        void picker_Unfocused(object sender, FocusEventArgs e)
        {
            Picker ent = (Picker)sender;
            
            if (ent.SelectedIndex <= 0)
                IsValid = false;
            else
                IsValid = true;
            ((Picker)sender).BackgroundColor = IsValid ? Color.Default : Color.Red;
        }

        protected override void OnDetachingFrom(Picker picker)
        {
            picker.Unfocused -= picker_Unfocused;
            picker.SelectedIndexChanged -= picker_SelectedIndexChanged;
        }


    }
}
