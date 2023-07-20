using Assets.Scripts.Store;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Level
{
    public class IAPWallVM : MonoBehaviour
    {
        public void GoToStore()
        {
            StoreHelper.ComingFromLevel = true;
            SceneManager.LoadScene("Store");
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}