using System;

namespace Lopakodo.ViewModel
{
    public class LopakodoField : ViewModelBase
    {
        private Boolean _isLocked;
        private String _text;

        public Boolean IsLocked
        {
            get { return _isLocked; }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        public String Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int32 X { get; set; }

        public Int32 Y { get; set; }

        public Int32 Number { get; set; }
        public DelegateCommand StepCommand { get; set; }
    }
}
