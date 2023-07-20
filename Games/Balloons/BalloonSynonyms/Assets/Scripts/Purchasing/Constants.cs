using System.Collections.Generic;

namespace Assets.Scripts.Purchasing
{
    public static class Constants
    {
        public static List<CustomProduct> Products = new List<CustomProduct>
        {
            new CustomProduct
            {
                Id = "ro.pack",
                Price = 5.99,
                GameModes = new List<string> {"synonym", "antonym", "paronym"}
            },
            new CustomProduct
            {
                Id = "game.full",
                Price = 9.49,
                GameModes = new List<string>{"synonym", "antonym", "paronym", "english", "german"}
            },
            new CustomProduct
            {
                Id = "foreign.pack",
                Price = 5.99,
                GameModes = new List<string> {"english", "german"}
            }
        };

        public const string PlayersProducts = nameof(PlayersProducts);
    }
}