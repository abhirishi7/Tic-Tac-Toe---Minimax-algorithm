using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class Settings : MonoBehaviour {

        private Color _selectedColor;
        private Color _defaultColor;

        public GamePlay GamePlay;
        public GameOver GameOver;
        public GameObject SettingsObj;
        public static int BoardSize;
        public static int Difficulty;
        public static bool IsSound= true;
        public static bool IsSet = false;
        public GameObject[] Levels;
        public GameObject CurrLevel;
        public GameObject Sound;
        public Text SizeText;
        public Slider SizeSlider;

        private void Awake() {
            _selectedColor = new Color(1, 0.25f, 0.25f, 1);
            _defaultColor = new Color(255, 255, 255, 255);
            SizeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
            if (IsSet) {
                getSettings();
                return;
            }                  
            setInitialSettings();
        }

        public void ValueChangeCheck() {
            SizeText.text = ((2 * (int)SizeSlider.value) + 1).ToString();
        }

        private void getSettings() {
            SizeSlider.value = (BoardSize - 1) / 2;
            SizeText.text = BoardSize.ToString();
            int currLevel = Difficulty - 1;
            for(int i=0;i<Levels.Length;i++) {
                if (i == currLevel) {
                    CurrLevel = Levels[i];
                    CurrLevel.GetComponent<Text>().color = _selectedColor;
                } else
                    Levels[i].GetComponent<Text>().color = _defaultColor;
            }
            Sound.SetActive(IsSound);
        }

        public void setInitialSettings() {
            // board size
            var value = SizeSlider.value;
            BoardSize = (2 * (int)value) + 1;
            SizeText.text = BoardSize.ToString();

            // difficulty level
            for (int i=0;i<Levels.Length;i++) {
                if (Levels[i].Equals(CurrLevel)) { 
                    Difficulty = i + 1;
                    break;
                }
            }

            // sound
            Sound.SetActive(IsSound);
        }

        public void OnSettingsClick() {
            GamePlay.Canvas.SetActive(true);
            SettingsObj.SetActive(true);
            GamePlay.IsSettings = true;
        }

        public void OnNoviceClick() {
            if (CurrLevel.Equals(Levels[0]))
                return;
            CurrLevel.GetComponent<Text>().color = _defaultColor;
            CurrLevel = Levels[0];
            CurrLevel.GetComponent<Text>().color = _selectedColor;
            Difficulty = 1;
        }

        public void OnIntermediateClick() {
            if (CurrLevel.Equals(Levels[1]))
                return;
            CurrLevel.GetComponent<Text>().color = _defaultColor;
            CurrLevel = Levels[1];
            CurrLevel.GetComponent<Text>().color = _selectedColor;
            Difficulty = 2;
        }

        public void OnExpertClick() {
            if (CurrLevel.Equals(Levels[2]))
                return;
            CurrLevel.GetComponent<Text>().color = _defaultColor;
            CurrLevel = Levels[2];
            CurrLevel.GetComponent<Text>().color = _selectedColor;
            Difficulty = 3;
        }

        public void OnSoundsClick() {
            IsSound = !IsSound;
            if (IsSound)
                Sound.SetActive(true);
            else
                Sound.SetActive(false);
        }

    }
}
