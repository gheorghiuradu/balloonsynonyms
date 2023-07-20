using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class OnClickLoadLevel : MonoBehaviour
    {
        private readonly SaveLoadService saveLoadLevelService = new SaveLoadService();
        public string SceneName;
        public string GameMode;

        private void OnMouseDown()
        {
            this.saveLoadLevelService.LoadLevel(this.SceneName, this.GameMode);
        }
    }
}
