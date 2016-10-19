using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RideShare.Behaviors
{
    public class NumberValidatorBehavior : Behavior<Entry>
    {

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EmailValidatorBehavior), false);

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
}
