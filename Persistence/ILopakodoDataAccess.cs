using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lopakodo.Persistence
{
    public enum Way { Left, Right, Down, Up }
    public struct Coord
    {
        public int x;
        public int y;
    }


    public class Creatures
    {
        public Coord cord;
        public Way actualWay;
        public bool status;
    }

    public interface ILopakodoDataAccess
    {
     
        Task<Tuple<Int32,Creatures,Creatures[], Coord, Coord[]>> LoadAsync(string path);

    
        Task SaveAsync(String path, Tuple<Int32, Creatures, Creatures[], Coord, Coord[]> input);

        Task<ICollection<SaveEntry>> ListAsync();
    }
   
}
