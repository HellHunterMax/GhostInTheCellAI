using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostInTheCellAI.Models
{
    public class Game
    {
        public List<Factory> Factories = new List<Factory>();
        public List<Troop> Troops = new List<Troop>();
        public List<Bomb> Bombs = new List<Bomb>();
        public int RemainingBombs = 2;
        private string[] _Inputs;

        public Game()
        {
        }

        public void UpdateGame()
        {
            int entityCount = int.Parse(Console.ReadLine());
            Troops.Clear();
            Bombs.Clear();
            for (int i = 0; i < entityCount; i++)
            {
                _Inputs = Console.ReadLine().Split(' ');
                int entityId = int.Parse(_Inputs[0]);
                string entityType = _Inputs[1];
                if (entityType == "FACTORY")
                {
                    Factory factory = Factories.First((fac => fac.Id == entityId));
                    UpdateFactory(factory, (Owner)int.Parse(_Inputs[2]), int.Parse(_Inputs[3]), int.Parse(_Inputs[4]));
                }
                else if (entityType == "TROOP")
                {
                    Troops.Add(
                        new Troop()
                        {
                            Id = entityId,
                            Owner = (Owner)int.Parse(_Inputs[2]),
                            Source = Factories.First((fac => fac.Id == int.Parse(_Inputs[3]))),
                            Destination = Factories.First((fac => fac.Id == int.Parse(_Inputs[4]))),
                            Size = int.Parse(_Inputs[5]),
                            TurnsToArrive = int.Parse(_Inputs[6])
                        });
                }
                else if (entityType == "BOMB")
                {
                    int destination = int.Parse(_Inputs[4]);
                    Bombs.Add(
                        new Bomb()
                        {
                            Id = entityId,
                            Owner = (Owner)int.Parse(_Inputs[2]),
                            Source = Factories.First((fac => fac.Id == int.Parse(_Inputs[3]))),
                            Destination = Factories.FirstOrDefault((fac => fac.Id == int.Parse(_Inputs[4]))),
                            TurnsToArrive = int.Parse(_Inputs[5])
                        });
                }
            }
        }
        private void UpdateFactory(Factory factory, Owner owner, int cyborgs, int production)
        {
            factory.Owner = owner;
            factory.Cyborgs = cyborgs;
            factory.Production = production;
        }
        public void BuildFactories()
        {
            int factoryCount = int.Parse(Console.ReadLine());

            for (int i = 0; i < factoryCount; i++)
            {
                Factories.Add(new Factory() { Id = i });
            }
            int linkCount = int.Parse(Console.ReadLine()); // the number of links between factories

            for (int i = 0; i < linkCount; i++)
            {
                _Inputs = Console.ReadLine().Split(' ');
                Factory factory1 = Factories.First(fac => fac.Id == int.Parse(_Inputs[0]));
                Factory factory2 = Factories.First(fac => fac.Id == int.Parse(_Inputs[1]));
                int distance = int.Parse(_Inputs[2]) + 1;

                Link link = new Link()
                {
                    Factory1 = factory1,
                    Factory2 = factory2,
                    Distance = distance
                };
                factory1.Links.Add(link);
                factory2.Links.Add(link);
            }
        }
    }
}
