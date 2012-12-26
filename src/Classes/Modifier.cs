using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATConverter.Classes
{
    class Modifier
    {
        public string Descriptor = "";

        public string AccessTarget = "";

        public string Name = "";
        public string Desc = "";

        public bool ModifyClassVisibility = false;

        public override string ToString()
        {
            var temp = string.Format("{0} {1}", this.AccessTarget, this.Descriptor);

            if (!this.ModifyClassVisibility)
            {
                temp += "." + this.Name;

                if (!string.IsNullOrEmpty(this.Desc))
                {
                    temp += this.Desc;
                }
            }

            return temp;
        }
    }
}
