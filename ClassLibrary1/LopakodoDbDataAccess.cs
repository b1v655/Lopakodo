using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Lopakodo.Windows.Lopakodo.Persistence;
namespace Lopakodo.Persistence
{

    public class LopakodoDbDataAccess : ILopakodoDataAccess
    {
        private LopakodoContext _context;

        public LopakodoDbDataAccess(String connection)
        {
            _context = new LopakodoContext(connection);
            _context.Database.CreateIfNotExists(); // adatbázis séma létrehozása, ha nem létezik
        }
        public async Task<Tuple<Int32, Creatures, Creatures[], Coord, Coord[]>> LoadAsync(String name)
        {
            try
            {
                Game game =  await _context.Games
                    .Include(cucc => cucc.Fields)
                    .SingleAsync(cucc => cucc.Name == name); // játék állapot lekérdezése

                Creatures player = new Creatures();
                Creatures[] guards = new Creatures[3];
                Coord exit = new Coord();
                Coord[] walls = new Coord[game.TableSize/3];
                Int32 g = 0,w=0;
                foreach (Field field in game.Fields) // mentett mezők feldolgozása
                {
                    if (field.Value == "p")
                    {
                        player.cord.x = field.X;
                        player.cord.y = field.Y;
                    }
                    if (field.Value == "g")
                    {
                        guards[g] = new Creatures();
                        guards[g].cord.x = field.X;
                        guards[g].cord.y = field.Y;
                        g++;
                    }
                    if (field.Value == "e")
                    {
                        exit.x = field.X;
                        exit.y = field.Y;
                    }
                    if (field.Value == "w")
                    {
                        walls[w] = new Coord();
                        walls[w].x = field.X;
                        walls[w].y = field.Y;
                        w++;
                    }
                }

                return Tuple.Create(game.TableSize, player , guards,exit, walls);
            }
            catch
            {
                throw new LopakodoDataException();
            }
        }
        public async Task SaveAsync(String name, Tuple<Int32, Creatures, Creatures[], Coord, Coord[]> OutPut)
        {
            try
            {
                // játékmentés keresése azonos névvel
                Game overwriteGame = await _context.Games
                    .Include(g => g.Fields)
                    .SingleOrDefaultAsync(g => g.Name == name);
                if (overwriteGame != null)
                    _context.Games.Remove(overwriteGame); // törlés

                Game dbGame = new Game
                {
                    TableSize = OutPut.Item1,
                    Name = name
                }; // új mentés létrehozása

                Field player = new Field
                {
                    X = OutPut.Item2.cord.x,
                    Y = OutPut.Item2.cord.y,
                    Value = "p"
                };
                dbGame.Fields.Add(player);
                for (Int32 i = 0; i < 3; ++i)
                {
                    Field guards = new Field
                    {
                        X = OutPut.Item3[i].cord.x,
                        Y = OutPut.Item3[i].cord.y,
                        Value = "g"
                    };
                    dbGame.Fields.Add(guards);
                }
                 Field exit = new Field
                 {
                     X = OutPut.Item4.x,
                     Y = OutPut.Item4.y,
                     Value = "e"
                 };
                dbGame.Fields.Add(exit);
                for (Int32 i = 0; i < OutPut.Item1/3; ++i)
                {
                    Field Walls = new Field
                    {
                        X = OutPut.Item5[i].x,
                        Y = OutPut.Item5[i].y,
                        Value = "w"
                    };
                    dbGame.Fields.Add(Walls);
                }

                _context.Games.Add(dbGame); 
                await _context.SaveChangesAsync();
            }
             catch
             {
                throw new LopakodoDataException();
             }
        }
        
        
        public async Task<ICollection<SaveEntry>> ListAsync()
        {
            try
            {
                return await _context.Games
                    .OrderByDescending(g => g.Time) 
                    .Select(g => new SaveEntry { Name = g.Name, Time = g.Time })
                    .ToListAsync();
            }
            catch
            {
                throw new LopakodoDataException();
            }
        }
    }
}