//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRM_Client.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailPeople
    {
        public int ID { get; set; }
        public int ID_People { get; set; }
        public string Email { get; set; }
        public int ID_TypeEmail { get; set; }
    
        public virtual People People { get; set; }
        public virtual TypeEmail TypeEmail { get; set; }
    }
}