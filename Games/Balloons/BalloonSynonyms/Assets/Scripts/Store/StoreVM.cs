using Assets.Scripts.Extensions;
using Assets.Scripts.Purchasing;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Store
{
    public class StoreVM : MonoBehaviour
    {
        private List<Product> products = new List<Product>();
        private PurchasingService purchasingService;

        public GameObject Star1;
        public GameObject Star2;
        public GameObject Star3;

        public GameObject BtnFullGame;
        public GameObject BtnRoPack;
        public GameObject BtnForeignPack;

        public GameObject OwnedText;

        public GameObject MessagePanel;
        public TextMeshProUGUI Message;

        public GameObject RestorePurchasesButton;

        private void Start()
        {
            this.purchasingService = new PurchasingService(false);
            this.purchasingService.Initialized.AddListener(() =>
            {
                this.products.AddRange(this.purchasingService.GetPurchasedProducts());
                this.Initialize();
            }
            );
            this.purchasingService.PurchasedProduct.AddListener(this.OnPurchaseComplete);
            this.purchasingService.PurchaseFailed.AddListener(this.OnPurchaseFailed);

            this.purchasingService.Initialize();

#if UNITY_IOS
            this.RestorePurchasesButton.SetActive(true);
#endif
        }

        private void Initialize()
        {
            var boughtIds = products.Select(p => p.definition.id);
            var originalProducts = Constants.Products.Where(p => boughtIds.Contains(p.Id)).ToList();
            var gamemModes = originalProducts.SelectMany(op => op.GameModes).Distinct().ToList();

            if (!(originalProducts.FirstOrDefault(op => op.Id == "game.full") is null))
            {
                this.AllOwned();
                return;
            }

            if (gamemModes.Count >= Constants.Products.Find(p => p.Id == "game.full").GameModes.Count)
            {
                this.AllOwned();
                return;
            }

            var packs = originalProducts.Where(op => op.Id.EndsWith("pack")).ToList();

            if (packs.Count > 0)
            {
                this.Star1.SetActive(true);
                if (!(packs.FirstOrDefault(p => p.Id == "ro.pack") is null))
                {
                    this.BtnRoPack.SetActive(false);
                }
                if (!(packs.FirstOrDefault(p => p.Id == "foreign.pack") is null))
                {
                    this.BtnForeignPack.SetActive(false);
                }
            }

            var fullGamePrice = this.purchasingService.AvailableProducts
                .Find(p => p.definition.id == "game.full")
                .metadata
                .localizedPriceString;
            var roPackPrice = this.purchasingService.AvailableProducts
                .Find(p => p.definition.id == "ro.pack")
                .metadata
                .localizedPriceString;
            var foreignPackPrice = this.purchasingService.AvailableProducts
                .Find(p => p.definition.id == "foreign.pack")
                .metadata
                .localizedPriceString;

            var priceTag = "TxtPrice";
            this.BtnFullGame.GetComponentInChildWithTag<TextMeshProUGUI>(priceTag).text = fullGamePrice;
            this.BtnRoPack.GetComponentInChildWithTag<TextMeshProUGUI>(priceTag).text = roPackPrice;
            this.BtnForeignPack.GetComponentInChildWithTag<TextMeshProUGUI>(priceTag).text = foreignPackPrice;
        }

        private void AllOwned()
        {
            this.Star1.SetActive(true);
            this.Star2.SetActive(true);
            this.Star3.SetActive(true);

            this.BtnFullGame.SetActive(false);
            this.BtnRoPack.SetActive(false);
            this.BtnForeignPack.SetActive(false);

            this.OwnedText.SetActive(true);
        }

        public void PurchaseProduct(string id)
        {
            this.purchasingService.BuyProduct(id);
        }

        public void OnPurchaseComplete(Product product)
        {
            this.purchasingService.SavePurchasedProduct(product);

            this.products.Add(product);
            this.Initialize();

            this.Message.text = $@"Felicitări!
Ai cumpărat {product.metadata.localizedTitle}.
Acum poți juca până la nivelul 10!";
            this.MessagePanel.SetActive(true);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            if (reason == PurchaseFailureReason.UserCancelled)
            {
                return;
            }

            this.Message.text = $@"Ups!
A apărut o eroare la cumpărare.
Eroare: {reason.ToString()}.";
            this.MessagePanel.SetActive(true);
        }

        public void GoBack()
        {
            if (StoreHelper.ComingFromLevel)
            {
                StoreHelper.ComingFromLevel = false;
                Time.timeScale = 1;
                SceneManager.LoadScene("Level");
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        public void CloseMessagePanel()
        {
            this.MessagePanel.SetActive(false);
        }

        public void RestorePurchases()
        {
            this.purchasingService.RestorePurchases();
            this.products.AddRange(this.purchasingService.GetPurchasedProducts());
            this.Initialize();
        }
    }
}