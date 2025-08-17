using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class meta_data_properties
    {
        public int dataPropertyId { get; set; }
        public int dataElementId { get; set; }
        public Nullable<int> parentDataPropertyId { get; set; }
        public string dataPropertyValue { get; set; }
        public int viewOrder { get; set; }
        public Nullable<int> insertBy { get; set; }
        public Nullable<System.DateTime> insertDate { get; set; }
        public int status { get; set; }
    }
}
