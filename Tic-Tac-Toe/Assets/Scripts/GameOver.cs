using UnityEngine;

namespace Assets.Scripts {
    public class GameOver : MonoBehaviour {

        public GamePlay GamePlay;
        [HideInInspector]
        public Animator anim;

        void Awake() {
            anim = GetComponent<Animator>();
        }

        void Update() {
            if (GamePlay.IsGameDrawn)
                anim.SetTrigger("GameDraw");
            else if (GamePlay.IsGameWon)
                anim.SetTrigger("GameWon");
            else if (GamePlay.IsGameLost)
                anim.SetTrigger("GameLost");
            else if (GamePlay.IsGameQuit)
                anim.SetTrigger("GameQuit");
            else if (GamePlay.IsSettings)
                anim.SetTrigger("GameSettings");
        }
    }
}