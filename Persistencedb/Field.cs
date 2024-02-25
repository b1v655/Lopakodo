using System;
using System.ComponentModel.DataAnnotations;

namespace Lopakodo.Persistence
{
    class Field
    {
        [Key]
        public Int32 Id { get; set; }
        
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
       
        public String Value { get; set; }
        
        public Game Game { get; set; }
    }
}
