using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lopakodo.Windows.Lopakodo.Persistence;
namespace Lopakodo.Windows.Lopakodo.gametest
{
    class DataMock : ILopakodoDataAccess
    {
        public async Task<Tuple<Int32, Creatures, Creatures[], Coord,Coord[]>> LoadAsync(String path)
        {
            Int32 tableSize = 9;
            Creatures player = new Creatures();
            player.cord.x = 8;
            player.cord.y = 8;
            Coord exit = new Coord();
            exit.x = 7;
            exit.y = 7;
            Creatures[] guards = new Creatures[3];
            for (Int32 i = 0; i < 3; i++)
            {
                guards[i] = new Creatures();
                guards[i].cord.x = i;
                guards[i].cord.y = i;
            }
            Coord[] walls = new Coord[tableSize / 3];
            for (Int32 i = 0; i < tableSize / 3; i++)
            {
                walls[i] = new Coord();
                walls[i].x = 3+i;
                walls[i].y = 3+i;
            }
            Tuple<Int32, Creatures, Creatures[], Coord, Coord[]> ToRet = Tuple.Create(tableSize, player, guards, exit, walls);
            return ToRet;
        }

        public async Task SaveAsync(String path, Tuple<Int32, Creatures, Creatures[], Coord, Coord[]> OutPut) { }
        public async Task<ICollection<SaveEntry>> ListAsync() { return new List<SaveEntry>(); }
    }
}

