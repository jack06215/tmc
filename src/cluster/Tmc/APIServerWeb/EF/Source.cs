//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APIServerWeb.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class Source
    {
        public Source()
        {
            this.EnvironmentLogs = new HashSet<EnvironmentLog>();
        }
    
        public int SourceID { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<EnvironmentLog> EnvironmentLogs { get; set; }
    }
}
