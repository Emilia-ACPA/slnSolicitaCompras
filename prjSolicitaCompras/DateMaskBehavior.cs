using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prjSolicitaCompras
{
    class DateMaskBehavior : Behavior<Entry>
    {
        private const string DateFormat = "dd/MM/yyyy";
        private const string Mask = "__/__/____";

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry == null)
                return;

            string text = entry.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                entry.Text = Mask;
                return;
            }

            if (text.Length > Mask.Length)
            {
                entry.Text = text.Substring(0, Mask.Length);
                return;
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (i < Mask.Length && Mask[i] != '_' && text[i] != Mask[i])
                {
                    text = text.Insert(i, Mask[i].ToString());
                }
            }

            entry.Text = text;

            if (DateTime.TryParseExact(text, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                entry.BackgroundColor = Colors.Transparent;
            }
            else
            {
                entry.BackgroundColor = Colors.Red;
            }
        }
    }
}
