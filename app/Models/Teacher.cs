//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Teacher
    {
        public Teacher()
        {
            this.Organization = new HashSet<Organization>();
            this.StudyGroup = new HashSet<StudyGroup>();
        }
    
        public int Id { get; set; }
        public string FIO { get; set; }
        public string Email { get; set; }
    
        public virtual ICollection<Organization> Organization { get; set; }
        public virtual ICollection<StudyGroup> StudyGroup { get; set; }
    }
}
