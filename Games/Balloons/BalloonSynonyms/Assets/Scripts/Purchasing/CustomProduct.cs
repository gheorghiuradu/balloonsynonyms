using System.Collections.Generic;

namespace Assets.Scripts.Purchasing
{
    public class CustomProduct : System.IEquatable<CustomProduct>
    {
        public string Id { get; set; }
        public double Price { get; set; }
        public List<string> GameModes { get; set; }

        public bool Equals(CustomProduct other)
        {
            return this.Id == other.Id;
        }

        public static CustomProduct Create(UnityEngine.Purchasing.Product product)
        {
            return Constants.Products.Find(cp => cp.Id == product.definition.id);
        }
    }
}