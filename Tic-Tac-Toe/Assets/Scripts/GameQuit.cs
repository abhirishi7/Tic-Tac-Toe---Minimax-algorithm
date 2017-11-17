using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class GameQuit : MonoBehaviour {

        public GamePlay GamePlay;
        public GameOver GameOver;
        public GameObject ScreenFader;
        public Text QuitText;
        public Text QuitYes;
        public Text QuitNo;
  
        public void OnHomeClick() {
            GamePlay.Canvas.SetActive(true);
            GamePlay.IsGameQuit = true;
        }
     
        public void OnQuitYesClick() {

            Application.Quit();
        }

        public void OnQuitNoClick() {
            GamePlay.IsGameQuit = false;
            GamePlay.Canvas.SetActive(false);
            GamePlay.GameBoard.SetActive(true);
            GamePlay.TurnCanvas.SetActive(true);
            GamePlay.TurnSprite.SetActive(true);
            GameOver.anim.ResetTrigger("GameQuit");
            resetQuitScreen();
        }

        private void resetQuitScreen() {
            Color color = ScreenFader.GetComponent<Image>().color;
            ScreenFader.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0);
            color = QuitText.GetComponent<Text>().color;
            QuitText.GetComponent<Text>().color = new Color(color.r, color.g, color.b, 0);
            color = QuitYes.GetComponent<Text>().color;
            QuitYes.GetComponent<Text>().color = new Color(color.r, color.g, color.b, 0);
            color = QuitNo.GetComponent<Text>().color;
            QuitNo.GetComponent<Text>().color = new Color(color.r, color.g, color.b, 0);
            QuitText.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            QuitYes.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            QuitNo.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            QuitYes.gameObject.SetActive(false);
            QuitNo.gameObject.SetActive(false);
        }
  
    }
}
