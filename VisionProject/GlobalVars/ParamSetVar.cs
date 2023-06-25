using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionProject.GlobalVars
{
    public class ParamSetVar
    {
        //属性
        private string _name;
        public string Name
                {
            get { return _name; }
            set {  _name= value; }
        }
        public paramSetType _type = paramSetType.Bool;
        public paramSetType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; }
        }


        public string _value = true.ToString();
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string _mark = "";
        public string Mark
        {
            get { return _mark; }
            set { _mark = value; }
        }
        

    }
    public class result
    {

    }
    public enum paramSetType
    {
        Bool = 0,
        String = 1,
        Double = 2,
        Int = 3,
    }


}
