using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.WP.Helpers
{
    public class NonEmptyDateValidator : DataFormValidator
    {
        public override void Validate(ValidatingDataFieldEventArgs args)
        {
            args.IsInputValid = (args.AssociatedDataField.Value as DateTime?) != null;
        }
    }
}
