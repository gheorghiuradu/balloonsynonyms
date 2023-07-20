using Assets.Scripts.Purchasing;
using Assets.Scripts.Serialization;
using Assets.Scripts.Services;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level
{
    public class LevelVM : MonoBehaviour
    {
        private readonly MenuPanelFactory menuFactory = new MenuPanelFactory();
        private readonly SaveLoadService saveService = new SaveLoadService();
        private PurchasingService purchasingService;
        private List<string> availableColors;
        private GameObject pauseMenu;

        private List<Word> words;
        private List<GameObject> balloons = new List<GameObject>();
        private int currentIndex = -1;
        private string currentWord;

        private int score;
        private int correct;
        private int mistakes;
        private bool isPaused;
        private bool isLastLevel;
        private bool isFinished;
        private bool isFailed;

        public Rect BalloonSpawnArea;
        public GameObject WinCanvas;
        public GameObject IAPWall;

        public UnityEngine.UI.Button BtnPrevWord;
        public TextMeshProUGUI TxtCurrentWord;
        public UnityEngine.UI.Button BtnNextWord;
        public UnityEngine.UI.Button BtnPause;
        public TextMeshProUGUI TxtCurrentLevel;
        public TextMeshProUGUI TxtScore;
        public UnityEngine.UI.Image Background;
        public float FloaterPercent;

        private void Start()
        {
            Time.timeScale = 1;

            this.BtnNextWord.onClick.AddListener(this.SkipWord);
            this.BtnPrevWord.onClick.AddListener(this.SetPreviousWord);
            this.BtnPause.onClick.AddListener(this.PauseGameDefault);

            this.SetupLevel();
            this.PutIAPWall();
        }

        private void PutIAPWall()
        {
            if (SceneHelper.CurrentLevel > 3)
            {
                this.purchasingService = new PurchasingService(false);
                this.purchasingService.Initialized.AddListener(() =>
                {
                    if (!this.purchasingService.HasGameMode(SceneHelper.GameModeSettings.GameMode))
                    {
                        this.PauseGame(false);
                        this.IAPWall.SetActive(true);
                    }
                });

                this.purchasingService.Initialize();
            }
        }

        public void PauseGameDefault()
        {
            this.PauseGame(true);
        }

        private void PauseGame(bool showMenu)
        {
            if (!this.isPaused)
            {
                if (showMenu)
                {
                    this.pauseMenu = this.menuFactory.CreateMenu();
                    this.pauseMenu.GetComponent<PauseMenu>().OnContinue = this.ResumeGame;
                }

                Time.timeScale = 0;
                this.balloons.ForEach(b => b.GetComponent<FloaterScript>().GameIsPaused = true);

                this.BtnNextWord.onClick.RemoveAllListeners();
                this.BtnPrevWord.onClick.RemoveAllListeners();
                this.BtnPause.onClick.RemoveAllListeners();

                this.isPaused = true;
            }
        }

        private void ResumeGame()
        {
            if (this.isPaused)
            {
                Time.timeScale = 1;
                this.balloons.ForEach(b => b.GetComponent<FloaterScript>().GameIsPaused = false);
                this.BtnNextWord.onClick.AddListener(this.SkipWord);
                this.BtnPrevWord.onClick.AddListener(this.SetPreviousWord);
                this.BtnPause.onClick.AddListener(this.PauseGameDefault);

                Object.Destroy(this.pauseMenu);

                this.isPaused = false;
            }
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !this.isPaused)
            {
                this.PauseGameDefault();
            }
            else
            if (Input.GetKeyDown(KeyCode.Escape) && this.isPaused && !this.isFailed)
            {
                this.ResumeGame();
            }

            // Keep floaters inside the screen;
            //if (!(this.balloons is null))
            //{
            //    this.balloons.ForEach(b =>
            //    {
            //        if (b.transform.position.x > this.BalloonSpawnArea.xMax
            //        || b.transform.position.x < this.BalloonSpawnArea.xMin)
            //        {
            //            var newPosition = this.GetValidSpawnPos(b.GetComponent<CircleCollider2D>().radius);
            //            b.GetComponent<FloaterScript>().InitialPosition = newPosition;
            //            b.GetComponent<Rigidbody2D>().position = newPosition;
            //        }
            //    });
            //}

            // Check Win Condition
            if (this.words.Count == 0 && !this.isFinished)
            {
                this.isFinished = true;
                this.WinLevel();
            }
        }

        private void SetupLevel()
        {
            var currentLevel = SceneHelper.GameModeSettings.Levels.FirstOrDefault(l => l.Level == SceneHelper.CurrentLevel);
            this.isLastLevel = SceneHelper.GameModeSettings.Levels
                            .FirstOrDefault(l => l.Level == currentLevel.Level + 1) is null;

            this.LoadCustomBackground();

            if (SceneHelper.CurrentLevel != 0)
            {
                this.TxtCurrentLevel.text = $"Nivel {SceneHelper.CurrentLevel}";
            }

            this.words = SceneHelper.PickWords(currentLevel.Words);
            this.words.Shuffle();
            var fakeWords = SceneHelper.PickWords(currentLevel.FakeWords);
            fakeWords.Shuffle();

            if (currentLevel.Level > 1)
            {
                this.score += this.saveService.GetLastScore(SceneHelper.GameModeSettings.GameMode);
            }
            this.TxtScore.text = $"{this.score} <sprite=\"Hud/coin/coin-1\" index=0>";

            // Set up floaters
            var tempWords = this.words.Select(w => w.Key).ToList();
            tempWords.AddRange(fakeWords.Select(w => w.Key));
            tempWords.Shuffle();
            switch (SceneHelper.GameModeSettings.LevelTypeEnum)
            {
                case LevelType.Balloon:
                    this.availableColors = Constants.BalloonColors;
                    break;

                case LevelType.Cookie:
                    this.availableColors = Constants.CookieColors;
                    break;
            }

            var balloonFactory = new FloaterFactory();

            //var prefabRadius = balloonFactory.FloaterRadius(SceneHelper.GameModeSettings.LevelTypeEnum);
            var bgRect = this.Background.rectTransform.rect;

            var floatersize = new Vector2();
            floatersize.x = bgRect.size.x * this.FloaterPercent;
            floatersize.y = bgRect.size.x * this.FloaterPercent;
            var floaterRadius = floatersize.x / 2;

            this.BalloonSpawnArea.xMax = bgRect.x + bgRect.size.x - (floatersize.x /2) + this.transform.position.x;
            this.BalloonSpawnArea.xMin = bgRect.x + (floatersize.x / 2) + this.transform.position.x;
            foreach (var word in tempWords)
            {
                this.availableColors.Shuffle();
                var validSpawnPosition = this.GetValidSpawnPos(floaterRadius);

                var balloon = balloonFactory.SpawnFloater(
                    type: SceneHelper.GameModeSettings.LevelTypeEnum,
                    position: validSpawnPosition,
                    color: this.availableColors[0],
                    parent: this.Background.gameObject.transform,
                    size: floatersize, 
                    word: word);

                balloon.transform.SetAsFirstSibling();
                this.balloons.Add(balloon);

                var script = balloon.GetComponent<FloaterScript>();

                script.Popped += () =>
                {
                    this.score += 10;
                    this.correct++;

                    this.TxtScore.text = $"{this.score}   <sprite=\"Hud/coin/coin-2\" index=0>";
                    this.WaitThenExecuteCoroutine(0.2f, () => this.TxtScore.text = $"{this.score}   <sprite=\"Hud/coin/coin-3\" index=0>");
                    this.WaitThenExecuteCoroutine(0.4f, () => this.TxtScore.text = $"{this.score}   <sprite=\"Hud/coin/coin-4\" index=0>");
                    this.WaitThenExecuteCoroutine(0.6f, () => this.TxtScore.text = $"{this.score} <sprite=\"Hud/coin/coin-1\" index=0>");

                    this.words.RemoveAt(this.currentIndex);
                    this.balloons.Remove(balloon);
                };
                script.FailedToPop += () =>
                {
                    this.score -= 10;
                    this.mistakes++;
                    this.TxtScore.text = $"{this.score} <sprite=\"Hud/coin/coin-1\" index=0>";
                    if (this.score <= -30)
                    {
                        this.isFailed = true;
                        this.FailLevel();
                    }
                };

                script.StartRising(currentLevel.BalloonSpeed, this.BtnPause.transform.position.y + 50);
            }

            // move balloons up when popped to reduce wait time for float
            foreach (var balloon in this.balloons)
            {
                var script = balloon.GetComponent<FloaterScript>();
                script.Popped += () =>
                {
                    var difference = this.BalloonSpawnArea.yMin - script.InitialPosition.y;
                    if (difference >= 0.5)
                    {
                        script.InitialPosition = new Vector3
                                    (script.InitialPosition.x,
                                    script.InitialPosition.y + 0.5f,
                                    script.InitialPosition.z);
                    }
                    else
                    {
                        script.InitialPosition = new Vector3
                                    (script.InitialPosition.x,
                                    this.BalloonSpawnArea.yMin,
                                    script.InitialPosition.z);
                    }
                };
            }

            // Starts the level
            this.SetNextWord();
        }

        private void LoadCustomBackground()
        {
            if (!string.IsNullOrEmpty(SceneHelper.GameModeSettings.Background))
            {
                var bgSrpite = Resources.Load<Sprite>($"Sprites/Backgrounds/{SceneHelper.GameModeSettings.Background}");
                if (!(bgSrpite is null)) this.Background.sprite = bgSrpite;
            }
        }

        private Vector3 GetValidSpawnPos(float radius)
        {
            var spawnPos = this.GetRandomPos();

            var collider = Physics2D.OverlapCircle(spawnPos, radius);

            // Search for an available spawn position while avoiding stack overflow
            var retryCount = 0;
            while (!(collider is null))
            {
                retryCount++;
                if (retryCount % 10 == 0)
                {
                    var min = BalloonSpawnArea.min;
                    this.BalloonSpawnArea.min = new Vector2(min.x, min.y - 1); // Expand the vertical area available;
                }

                spawnPos = this.GetRandomPos();
                collider = Physics2D.OverlapCircle(spawnPos, radius);
            }

            return spawnPos;
        }

        private Vector3 GetRandomPos()
        {
            var x = Random.Range(this.BalloonSpawnArea.xMin, this.BalloonSpawnArea.xMax);
            var y = Random.Range(this.BalloonSpawnArea.yMin, this.BalloonSpawnArea.yMax);

            return new Vector3(x, y, 0);
        }

        public void SetNextWord()
        {
            this.currentIndex++;
            this.currentIndex = this.currentIndex >= this.words.Count ?
                                0 : this.currentIndex;

            this.currentWord = this.words.Select(w => w.Value).ToList()[this.currentIndex];
            this.TxtCurrentWord.text = this.currentWord;
        }

        public void SetPreviousWord()
        {
            this.currentIndex--;
            if (this.currentIndex < 0)
            {
                this.currentIndex = this.words.Count - 1;
            }

            this.currentWord = this.words.Select(w => w.Value).ToList()[this.currentIndex];
            this.TxtCurrentWord.text = this.currentWord;
        }

        public void SkipWord()
        {
            this.currentIndex++;
            if (this.currentIndex > this.words.Count - 1)
            {
                this.currentIndex = 0;
            }

            this.currentWord = this.words.Select(w => w.Value).ToList()[this.currentIndex];
            this.TxtCurrentWord.text = this.currentWord;
        }

        public bool IsSynonym(string key)
        {
            return this.words.Find(w => w.Key == key)?.Value == this.currentWord;
        }

        private void WinLevel()
        {
            var nextLevel = this.isLastLevel ? 1 : SceneHelper.CurrentLevel + 1;
            this.saveService.SaveLevelAndScore(SceneHelper.GameModeSettings.GameMode, nextLevel, this.score);
            var winCanvasVM = this.WinCanvas.GetComponent<WinCanvasVM>();
            winCanvasVM.IsLastLevel = this.isLastLevel;
            winCanvasVM.Show(this.correct, this.mistakes, this.score);
            this.PauseGame(false);
        }

        private void FailLevel()
        {
            this.TxtCurrentWord.GetComponent<CurrentWordScript>().FailLevel();
            this.BtnNextWord.onClick.RemoveAllListeners();
            this.BtnPrevWord.onClick.RemoveAllListeners();

            this.balloons.ForEach(b => b.GetComponent<FloaterScript>().GameIsPaused = true);

            this.WaitThenExecuteCoroutine(2.8f, () =>
            {
                this.menuFactory.CreateMenu();
                Time.timeScale = 0;
            });
        }
    }
}