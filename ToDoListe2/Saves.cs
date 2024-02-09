using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListe2
{
    public class _saves
    {
        
        public class Saves
        {
            public int ID {  get; set; }

            public string MyTask { get; set; }
            public bool Erledigt { get; set; }
            public int TabIndex { get; set; }
            public string DueDateString { get; set; }
            public Nullable<DateTime> DueDate { get; set; }
        }
    }
}
