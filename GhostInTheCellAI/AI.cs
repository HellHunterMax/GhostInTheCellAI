using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostInTheCellAI
{
    public class AI
    {
        private List<Factory> Factories = new List<Factory>();
        private string[] _Inputs;

        public AI()
        {
            BuildFactories();
        }

        public void Run()
        {
            // game loop
            while (true)
            {
                List<Troop> troops = new List<Troop>();
                int entityCount = int.Parse(Console.ReadLine()); // the number of entities (e.g. factories and troops)
                for (int i = 0; i < entityCount; i++)
                {
                    _Inputs = Console.ReadLine().Split(' ');
                    int entityId = int.Parse(_Inputs[0]);
                    string entityType = _Inputs[1];
                    if (entityType == "FACTORY")
                    {
                        Factory factory = Factories.First((fac => fac.Id == entityId));
                        UpdateFactory(factory);
                    }
                    else if (entityType == "TROOP")
                    {
                        troops.Add(
                            new Troop()
                            {
                                Owner = (Owner)int.Parse(_Inputs[2]),
                                Source = Factories.First((fac => fac.Id == int.Parse(_Inputs[3]))),
                                Destination = Factories.First((fac => fac.Id == int.Parse(_Inputs[4]))),
                                Size = int.Parse(_Inputs[5]),
                                TurnsToArrive = int.Parse(_Inputs[6])
                            });
                    }
                }

                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");


                // Any valid action, such as "WAIT" or "MOVE source destination cyborgs"
                Console.WriteLine("WAIT");
            }
        }
        private void UpdateFactory(Factory factory)
        {
            factory.Owner = (Owner)int.Parse(_Inputs[2]);
            factory.Cyborgs = int.Parse(_Inputs[3]);
            factory.Production = int.Parse(_Inputs[4]);
        }

        private void BuildFactories()
        {
            int factoryCount = int.Parse(Console.ReadLine()); // the number of factories

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
                int distance = int.Parse(_Inputs[2]);

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
