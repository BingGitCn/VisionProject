

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartLogo
{
    public class TimeSpanIncreaser : Increaser<TimeSpan>
    {
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(TimeSpan), typeof(TimeSpanIncreaser), new PropertyMetadata(default(TimeSpan)));

        private TimeSpan _current;

        public override TimeSpan Next
        {
            get
            {
                var result = Start + _current;
                _current += Step;
                return result;
            }
        }

        public override TimeSpan Start
        {
            get { return (TimeSpan)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }
    }

    public abstract class Increaser<T> : DependencyObject
    {
        public virtual T Start { get; set; }

        public T Step { get; set; }

        public abstract T Next { get; }
    }
}

