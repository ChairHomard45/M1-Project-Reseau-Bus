using Backend.Statique;

namespace Backend.StaticTest
{
    public class FakeReseau
    {
        private static FakeReseau? _instance;
        private static readonly object Lock = new();

        private readonly List<Lignes> _lignes;

        private FakeReseau()
        {
            _lignes = new List<Lignes>();
            ConstructFakeReseau();
        }

        public static FakeReseau Instance
        {
            get
            {
                lock (Lock)
                {
                    return _instance ??= new FakeReseau();
                }
            }
        }

        // Return a cloned random ligne for simulation
        public Lignes GetRandomLigne()
        {
            if (_lignes.Count == 0)
                throw new InvalidOperationException("No lignes available");

            int randomIndex = new Random().Next(_lignes.Count);
            return (Lignes)_lignes[randomIndex].Clone();
        }

        // Get next ElementLigne on a ligne given current element and direction
        public ElementLigne GetNextElementDeLigne(Lignes ligne, ElementLigne currentElement, int sens)
        {
            int currentIndex = ligne.GetElementLigne().FindIndex(e => e.GetId() == currentElement.GetId());
            if (currentIndex == -1)
                throw new InvalidOperationException("Element not found in ligne");

            if (sens == 1)
            {
                int nextIndex = (currentIndex + 1) % ligne.GetElementLigne().Count;
                return ligne.GetElementLigne()[nextIndex];
            }
            else if (sens == -1)
            {
                int nextIndex = (currentIndex - 1 + ligne.GetElementLigne().Count) % ligne.GetElementLigne().Count;
                return ligne.GetElementLigne()[nextIndex];
            }
            else
            {
                throw new InvalidOperationException("Sens must be 1 or -1");
            }
        }

        // Add ligne if not exists
        public void AddLigne(Lignes ligne)
        {
            if (!_lignes.Contains(ligne))
                _lignes.Add(ligne);
        }

        // Simple fake reseau setup: two lignes with a few stops
        private void ConstructFakeReseau()
        {
            var ligne1 = new Lignes("Ligne 1");
            ligne1.AjouterLigne(new Terminus(1, "Terminus Start L1", 0, 0));
            ligne1.AjouterLigne(new Arrets(2, "Arret 2 L1", 1, 0));
            ligne1.AjouterLigne(new Arrets(3, "Arret 3 L1", 2, 0));
            ligne1.AjouterLigne(new Terminus(4, "Terminus End L1", 3, 0));

            var ligne2 = new Lignes("Ligne 2");
            ligne2.AjouterLigne(new Terminus(10, "Terminus Start L2", 0, 1));
            ligne2.AjouterLigne(new Arrets(11, "Arret 2 L2", 1, 1));
            ligne2.AjouterLigne(new Arrets(12, "Arret 3 L2", 2, 1));
            ligne2.AjouterLigne(new Terminus(13, "Terminus End L2", 3, 1));

            _lignes.Add(ligne1);
            _lignes.Add(ligne2);
        }
    }
}
