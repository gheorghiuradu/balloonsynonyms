using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace Assets.Scripts.Purchasing
{
    public class PurchasingService : IStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        public List<Product> AvailableProducts { get; } = new List<Product>();

        public UnityEvent Initialized = new UnityEvent();

        public class PurchasedProductEvent : UnityEvent<Product> { }

        public class PurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason> { }

        public PurchasedProductEvent PurchasedProduct = new PurchasedProductEvent();

        public PurchaseFailedEvent PurchaseFailed = new PurchaseFailedEvent();

        public PurchasingService()
        {
            this.Initialize();
        }

        public PurchasingService(bool initialize)
        {
            if (initialize)
            {
                this.Initialize();
            }
        }

        public void Initialize()
        {
            if (this.storeController is null)
            {
                this.InitializePurchasing();
            }
        }

        private void InitializePurchasing()
        {
            if (this.IsInitialized())
            {
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            Constants.Products.ForEach(p => builder.AddProduct(p.Id, ProductType.NonConsumable));

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyProduct(string productId)
        {
            if (this.IsInitialized())
            {
                var product = this.storeController.products.WithID(productId);

                if (!(product is null) && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asynchronously: '{0}'", product.definition.id));
                    this.storeController.InitiatePurchase(product);
                }
                else
                {
                    // ... report the product look-up failure situation
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public void SavePurchasedProduct(Product purchasedProduct)
        {
            var boughtProducts = PlayerPrefsX.GetStringArray(Constants.PlayersProducts).ToList();
            if (boughtProducts.Find(i => i == purchasedProduct.definition.id) is null)
            {
                boughtProducts.Add(purchasedProduct.definition.id);
            }
            PlayerPrefsX.SetStringArray(Constants.PlayersProducts, boughtProducts.ToArray());
            PlayerPrefs.Save();
        }

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ...
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = this.extensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then
                    // no purchases are available to be restored.

                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        private bool IsInitialized()
        {
            return !(this.storeController is null) && !(this.extensionProvider is null);
        }

        //
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.storeController = controller;
            this.extensionProvider = extensions;
            this.AvailableProducts.AddRange(controller.products.all);
            this.Initialized?.Invoke();
        }

        public IEnumerable<Product> GetPurchasedProducts()
        {
            var products = this.storeController.products.all.Where(p => p.hasReceipt).ToList();
            var savedProducts = this.GetSavedProducts();

            products.AddRange(savedProducts);

            return products.Distinct();
        }

        public IEnumerable<Product> GetSavedProducts()
        {
            var ids = PlayerPrefsX.GetStringArray(Constants.PlayersProducts);
            return this.storeController.products.all.Where(p => ids.Contains(p.definition.id));
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", i.definition.storeSpecificId, p));

            this.PurchaseFailed?.Invoke(i, p);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            // A non-consumable product has been purchased by this user.
            if (!string.IsNullOrEmpty(e.purchasedProduct.definition.id))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));

                this.SavePurchasedProduct(e.purchasedProduct);
                this.PurchasedProduct?.Invoke(e.purchasedProduct);
            }
            else
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", e.purchasedProduct.definition.id));
            }

            return PurchaseProcessingResult.Complete;
        }

        public bool HasGameMode(string gameMode)
        {
            var boughtProductIds = PlayerPrefsX.GetStringArray(Constants.PlayersProducts).ToList();

            var products = Constants.Products.Where(p => boughtProductIds.Contains(p.Id)).ToList();
            var relevantProduct = products.Find(p => p.GameModes.Contains(gameMode.ToLower()));

            return !(relevantProduct is null);
        }
    }
}