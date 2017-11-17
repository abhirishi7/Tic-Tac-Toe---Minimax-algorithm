using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class GamePlay : MonoBehaviour {

        private int _boardSize;
        private int _depth ;
        private float horizontalTileDist = 1.9f;
        private float verticalTileDist = 1.85f;
        private int[][] _winCombinations;
        private int[,] _gameBoard;
        private GameObject[] _gameBoardUI;
        private int _totalMovesPlayed;

        public GameObject GameBoard;
        public GameObject EmptyTilePrefab;
        public Sprite EmptyTile;
        public Sprite XTile;
        public Sprite OTile;
        public Transform InitialTileObj;
        [HideInInspector]
        public bool IsGameDrawn;
        [HideInInspector]
        public bool IsGameWon;
        [HideInInspector]
        public bool IsGameLost;
        [HideInInspector]
        public bool IsGameQuit;
        [HideInInspector]
        public bool IsSettings;
        public GameObject Canvas;
        public GameQuit GameQuit;
        public Settings GameSettings;
        public GameObject TurnSprite;
        public GameObject TurnCanvas;

        void Start() {
            _boardSize = Settings.BoardSize;
            if (Settings.Difficulty == 1)
                _depth = 0;
            else if (Settings.Difficulty == 2)
                _depth = 1;
            else if (Settings.Difficulty == 3)
                _depth = 4;
            var totalWinCombo = (2 * _boardSize) + 2;
            _winCombinations = new int[totalWinCombo][];
            _gameBoard = new int[_boardSize, _boardSize];
            _gameBoardUI = new GameObject[_boardSize * _boardSize];
            _totalMovesPlayed = 0;
            IsGameDrawn = false;
            IsGameWon = false;
            IsGameLost = false;
            IsGameQuit = false;
            IsSettings = false;
            Canvas.SetActive(false);
            createGameBoardUI();
            clearGameBoard();
            getAllWinningCombinations();
        }

        private void createGameBoardUI() {
            var initXpos = InitialTileObj.localPosition.x;
            var initYpos = InitialTileObj.localPosition.y;

            for (int i = 0; i < _boardSize; i++) {
                for (int j = 0; j < _boardSize; j++) {
                    GameObject gameObj = Instantiate(EmptyTilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    gameObj.transform.parent = GameBoard.transform;
                    var xPos = initXpos + (j * horizontalTileDist);
                    var yPos = initYpos - (i * verticalTileDist);
                    gameObj.transform.localPosition = new Vector3(xPos, yPos, 0);
                    var squareNumber = getBoardSquareNumber(i, j);
                    _gameBoardUI[squareNumber - 1] = gameObj;
                    gameObj.GetComponent<TileInfo>().SquareNumber = squareNumber;
                }
            }
            positionGameBoard();          
        }

        private void positionGameBoard() {
            switch(_boardSize) {
                case 3:
                    GameBoard.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    GameBoard.transform.localPosition = new Vector3(0, 0.6f, 0);
                    break;
                case 5:
                    GameBoard.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    GameBoard.transform.localPosition = new Vector3(-1.7f, 2, 0);
                    break;
                case 7:
                    GameBoard.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    GameBoard.transform.localPosition = new Vector3(-2.6f, 2.5f, 0);
                    break;
                case 9:
                    GameBoard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    GameBoard.transform.localPosition = new Vector3(-3.5f, 3f, 0);
                    break;
                case 11:
                    GameBoard.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    GameBoard.transform.localPosition = new Vector3(-5.4f, 4.2f, 0);
                    break;
                default:
                    GameBoard.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    GameBoard.transform.localPosition = new Vector3(0, 0.6f, 0);
                    break;
            }
        }

        private void getAllWinningCombinations() {
            for (int i = 0; i < _boardSize; i++) { // for all horizontal combo
                _winCombinations[i] = new int[_boardSize];
                for (int j = 0; j < _boardSize; j++) {
                    _winCombinations[i][j] = (i * _boardSize) + (j + 1);
                }
            }
            for (int i = 0; i < _boardSize; i++) { // for all vertical combo
                _winCombinations[i + _boardSize] = new int[_boardSize];
                for (int j = 0; j < _boardSize; j++) {
                    _winCombinations[(i + _boardSize)][j] = (j * _boardSize) + (i + 1);
                }
            }
            _winCombinations[2 * _boardSize] = new int[_boardSize];
            for (int i = 0; i < _boardSize; i++) { // for right diagonal combo
                _winCombinations[(2 * _boardSize)][i] = (i * _boardSize) + (i + 1);
            }
            _winCombinations[(2 * _boardSize) + 1] = new int[_boardSize];
            for (int i = 0; i < _boardSize; i++) { // for left diagonal combo
                _winCombinations[((2 * _boardSize) + 1)][i] = ((i + 1) * _boardSize) - i;
            }
        }

        private int getBoardSquareNumber(int row, int col) {
            return ((row * _boardSize) + (col + 1));
        }

        private bool isSquareFree(int row, int col) {
            return (_gameBoard[row, col] == 0) ? true : false;
        }

        private bool checkIfDraw() {
            return (_totalMovesPlayed == (_boardSize * _boardSize)) ? true : false;
        }

        private int[] getRowAndColfromSquareNum(int squareNum) {
            int[] rowAndCol = new int[2];
            rowAndCol[0] = (squareNum - 1) / _boardSize;
            rowAndCol[1] = (squareNum - 1) % _boardSize;
            return rowAndCol;
        }

        /*
         *  Function returns 
         *  1 - if AI wins
         *  0 - if nobody has won yet
         * -1 - if oppn wins
         */
        private int checkIfWin() {
            for (int i = 0; i < _winCombinations.Length; i++) {
                int aiCount = 0;
                int oppnCount = 0;
                for (int j = 0; j < _boardSize; j++) {
                    var squareNum = _winCombinations[i][j];
                    var rowAndCol = getRowAndColfromSquareNum(squareNum);
                    var row = rowAndCol[0];
                    var col = rowAndCol[1];
                    if (_gameBoard[row, col] == 0)
                        break;
                    else if (_gameBoard[row, col] == 1)
                        aiCount++;
                    else
                        oppnCount++;
                }
                if (aiCount == _boardSize)
                    return 1;
                else if (oppnCount == _boardSize)
                    return -1;
            }
            return 0;
        }

        private void clearGameBoard() {
            for (int i = 0; i < _boardSize; i++) {
                for (int j = 0; j < _boardSize; j++) {
                    _gameBoard[i, j] = 0;
                }
            }
        }

        private bool isOver() {
            if (checkIfDraw())
                return true;
            if (checkIfWin() != 0)
                return true;
            return false;
        }

        private List<int> possibleMoves() {
            List<int> validMoves = new List<int>();
            for (int i = 0; i < _boardSize; i++) {
                for (int j = 0; j < _boardSize; j++) {
                    if (isSquareFree(i, j))
                        validMoves.Add(getBoardSquareNumber(i, j));
                }
            }
            return validMoves;
        }

        private int[] miniMax(int depth, int player, int alpha, int beta) {
            int bestMove = -1;
            int score = 0;
            List<int> validMoves = possibleMoves();
            if (isOver() || depth == 0) {
                int[] result = new int[2];         
                result[0] = scoreBoard(); // score
                result[1] = bestMove;
                return result;
            }

            for (var i = 0; i < validMoves.Count; i++) {
                var rowAndCol = getRowAndColfromSquareNum(validMoves[i]);
                _gameBoard[rowAndCol[0], rowAndCol[1]] = player; // Account for array index
                _totalMovesPlayed++; // increment total moves played
                if (player == 1) { // If AI's turn
                    score = miniMax(depth - 1, -1, alpha, beta)[0];
                    if (score > alpha) {
                        alpha = score;
                        bestMove = validMoves[i];
                    }
                } else { // If opponent's turn
                    score = miniMax(depth - 1, 1, alpha, beta)[0];
                    if (score < beta) {
                        beta = score;
                        bestMove = validMoves[i];
                    }
                }
                _gameBoard[rowAndCol[0], rowAndCol[1]] = 0; // Undo move
                _totalMovesPlayed--; // undo total moves played
                // Prune: stop iteration if alpha >= beta
                if (alpha >= beta)
                    break;
            }
            score = (player == 1) ? alpha : beta;
            int[] resultArray = new int[2];
            resultArray[0] = score;
            resultArray[1] = bestMove;
            return resultArray;
        }

        private int scoreBoard() {
            int score = 0;
            for (int i = 0; i < _winCombinations.Length; i++) {
                score += evaluateCombo(_winCombinations[i]);
            }
            return score;
        }

        private int evaluateCombo(int[] combo) {
            var score = 0;
            var rowAndCol = getRowAndColfromSquareNum(combo[0]);
            // one in a row
            if (_gameBoard[rowAndCol[0], rowAndCol[1]] == 1)
                score = 1;
            else if (_gameBoard[rowAndCol[0], rowAndCol[1]] == -1)
                score = -1;

            // two in a row
            //rowAndCol = getRowAndColfromSquareNum(combo[1]);
            //if (_gameBoard[rowAndCol[0], rowAndCol[1]] == 1) {
            //    if (score == 1)
            //        score = 10; // two in a row for AI
            //    else if (score == -1)
            //        return 0;
            //    else
            //        score = 1;
            //} else if (_gameBoard[rowAndCol[0], rowAndCol[1]] == -1) {
            //    if (score == -1)
            //        score = -10; // two in a row for opponet
            //    else if (score == 1)
            //        return 0;
            //    else
            //        score = -1;
            //}

            //// Three in a row
            //rowAndCol = getRowAndColfromSquareNum(combo[2]);
            //if (_gameBoard[rowAndCol[0], rowAndCol[1]] == 1) {
            //    if (score > 0)
            //        score *= 10; // three in a row for AI
            //    else if (score < 0)
            //        return 0;
            //    else
            //        score = 1;
            //} else if (_gameBoard[rowAndCol[0], rowAndCol[1]] == -1) {
            //    if (score < 0)
            //        score *= 10; // three in a row for opponet
            //    else if (score > 1)
            //        return 0;
            //    else
            //        score = -1;
            //}

            for(int i=1;i<_boardSize;i++) {
                rowAndCol = getRowAndColfromSquareNum(combo[i]);
                if (_gameBoard[rowAndCol[0], rowAndCol[1]] == 1) {
                    if (score > 0)
                        score += 10; 
                    else if (score < 0)
                        return 0;
                    else
                        score = 1;
                } else if (_gameBoard[rowAndCol[0], rowAndCol[1]] == -1) {
                    if (score < 0)
                        score -= 10; 
                    else if (score > 0)
                        return 0;
                    else
                        score = -1;
                }
            }
            return score;

        }

        public void Update() {
            GameObject clickedObj = null;
            if (Input.GetMouseButtonDown(0)) {
                clickedObj = GetClickedGameObject();
                if (clickedObj == null)
                    return;
                if (clickedObj.GetComponent<TileInfo>() != null) { // if object is tile
                    StartCoroutine(handleTileClick(clickedObj));
                } else if(clickedObj.tag == "Home") {
                    TurnCanvas.SetActive(false);
                    TurnSprite.SetActive(false);
                    GameBoard.SetActive(false);
                    handleHomeClick();
                } else if(clickedObj.tag == "Settings") {
                    TurnCanvas.SetActive(false);
                    TurnSprite.SetActive(false);
                    GameBoard.SetActive(false);
                    handleSettingsClick();
                }
            }
        }

        private void handleSettingsClick() {
            GameSettings.OnSettingsClick();
        }

        private void handleHomeClick() {
            if (IsSettings) {
                GameSettings.setInitialSettings();
                Settings.IsSet = true;
                SceneManager.LoadScene(0);
            }
            GameQuit.OnHomeClick();
            GameQuit.QuitYes.gameObject.SetActive(true);
            GameQuit.QuitNo.gameObject.SetActive(true);
        }

        private IEnumerator handleTileClick(GameObject tileObject) {
            var squareNum = tileObject.GetComponent<TileInfo>().SquareNumber;
            var rowAndCol = getRowAndColfromSquareNum(squareNum);
            if (!isSquareFree(rowAndCol[0], rowAndCol[1]))
                yield break;
            _gameBoardUI[squareNum - 1].GetComponent<SpriteRenderer>().sprite = XTile;
            _gameBoard[rowAndCol[0], rowAndCol[1]] = -1;
            _totalMovesPlayed++;
            if (checkIfDraw()) {
                Canvas.SetActive(true);
                IsGameDrawn = true;
                GameQuit.QuitYes.gameObject.SetActive(false);
                GameQuit.QuitNo.gameObject.SetActive(false);
                TurnCanvas.SetActive(false);
                TurnSprite.SetActive(false);
                yield break;
            }
            if (checkIfWin() != 0) {
                Canvas.SetActive(true);
                IsGameLost = true;
                GameQuit.QuitYes.gameObject.SetActive(false);
                GameQuit.QuitNo.gameObject.SetActive(false);
                TurnCanvas.SetActive(false);
                TurnSprite.SetActive(false);
                yield break;
            }
            TurnSprite.GetComponent<SpriteRenderer>().sprite = OTile;
            TurnCanvas.GetComponentInChildren<Text>().text = "Bot's Turn";
            yield return null;
            var bestMove = miniMax(_depth, 1, int.MinValue, int.MaxValue)[1];
            if (bestMove == -1) {
                var validMoves = possibleMoves();
                bestMove = validMoves[Random.Range(0, validMoves.Count)];
            }               
            rowAndCol = getRowAndColfromSquareNum(bestMove);
            _gameBoardUI[bestMove - 1].GetComponent<SpriteRenderer>().sprite = OTile;
            _gameBoard[rowAndCol[0], rowAndCol[1]] = 1;
            _totalMovesPlayed++;
            TurnSprite.GetComponent<SpriteRenderer>().sprite = XTile;
            TurnCanvas.GetComponentInChildren<Text>().text = "Your Turn";
            if (checkIfWin() != 0) {
                Canvas.SetActive(true);
                GameQuit.QuitYes.gameObject.SetActive(false);
                GameQuit.QuitNo.gameObject.SetActive(false);
                TurnCanvas.SetActive(false);
                TurnSprite.SetActive(false);
                IsGameWon = true;
            }
        }

        GameObject GetClickedGameObject() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                return hit.transform.gameObject;
            else
                return null;
        }
    }
}