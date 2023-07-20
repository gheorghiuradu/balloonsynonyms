using UnityEngine;

namespace Assets.Scripts.Services
{
    public class MenuPanelFactory
    {
        public GameObject CreateMenu()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Level/MenuCanvas");
            return Object.Instantiate(prefab);
        }

        public GameObject CreateConfirmatioMenu()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Level/ExitToMainMenuCanvas");
            return Object.Instantiate(prefab);
        }

        //public GameObject CreateIAPMenu(PurchasingService purchasingService)
        //{
        //    var prefab = Resources.Load<GameObject>("Prefabs/Level/IAPCanvas");
        //    var iapCanvas = Object.Instantiate(prefab);
        //    iapCanvas.GetComponent<IAPCanvasScript>().PurchasingService = purchasingService;
        //    iapCanvas.GetComponent<IAPCanvasScript>().Initialize();

        //    return iapCanvas;
        //}
    }
}