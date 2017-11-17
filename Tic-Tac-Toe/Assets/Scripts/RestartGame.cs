using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
    public class RestartGame : MonoBehaviour {

       public void OnPointerClick() {
            SceneManager.LoadScene(0);
        }
    }
}