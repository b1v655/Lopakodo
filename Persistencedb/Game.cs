using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Lopakodo.Persistence
{
    class Game
    {
        [Key]
        [MaxLength(32)]
        public String Name { get; set; }
        
        public Int32 TableSize { get; set; }
        
        public DateTime Time { get; set; }
        
        public ICollection<Field> Fields { get; set; }

        public Game()
        {
            Fields = new List<Field>();
            Time = DateTime.Now;
        }
    }
}
