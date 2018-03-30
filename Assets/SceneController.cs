using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {

	[SerializeField] private MemoryCard originalCard;
	[SerializeField] private Sprite[] images;
	[SerializeField] private TextMesh scoreLabel;
	[SerializeField] private TextMesh timeLabel;
	[SerializeField] private Dropdown sizeSelect;
	[SerializeField] private GameObject smokePrefab;
	[SerializeField] private GameObject victoryImage;

	//victory message
	public int gridRows;
	public int gridCols;
	public float offsetX;
	public float offsetY;
	private MemoryCard _firstRevealed;
	private MemoryCard _secondRevealed;
	public bool canReveal {
		get {return _secondRevealed == null;}
	}
	private int _score;
	private int perfectScore;
	private float _time;
	private int size;

	public void CardRevealed(MemoryCard card) {
		if (_firstRevealed == null) {
			_firstRevealed = card;
		} else {
			_secondRevealed = card;
			StartCoroutine(CheckMatch());
		}
	}

	public void Restart() {
		Application.LoadLevel("Scene");
	}

	//called by the start button
	void Start() {
		victoryImage.transform.position = new Vector3 (0, 0, 10);
		originalCard.SendMessage ("Unreveal");
		_time = 0f;
		_score = 0;
		loadCards ();
	}

	//called by the end button
	public void GameOver() {
		Application.Quit();
	}

	public void loadCards() {
		GameObject[] cardClones = GameObject.FindObjectsOfType<GameObject> ();
		//destroy all cardClones from previous gridSize first
		for (int i = 0; i < cardClones.Length; i++) {
			if (cardClones [i].name.Contains ("Clone"))
				Destroy (cardClones [i]);
		}
		//2x4
		if (sizeSelect.value == 0) {
			gridRows = 2;
			gridCols = 4;
			offsetX = 2f;
			offsetY = 2.5f;
			originalCard.transform.position = new Vector3 (-3, 1, -1);
			originalCard.transform.localScale = new Vector3 (0.5f, 0.5f, 1f);
		} 
		//2x3
		else if (sizeSelect.value == 1) {
			gridRows = 2;
			gridCols = 3;
			offsetX = 3f;
			offsetY = 2.5f;
			originalCard.transform.position = new Vector3 (-3f, 1f, -1f);
			originalCard.transform.localScale = new Vector3 (0.5f, 0.5f, 1f);

		} 
		//2x5
		else if (sizeSelect.value == 2) {
			gridRows = 2;
			gridCols = 5;
			offsetX = 1.7f;
			offsetY = 2.5f;
			originalCard.transform.position = new Vector3 (-3.3f, 1f, -1f);
			originalCard.transform.localScale = new Vector3 (0.4f, 0.4f, 1f);
		} 
		//3x4
		else if (sizeSelect.value == 3) {
			gridRows = 3;
			gridCols = 4;
			offsetX = 2f;
			offsetY = 2f;
			originalCard.transform.position = new Vector3 (-3f, 1.3f, -1f);
			originalCard.transform.localScale = new Vector3 (0.4f, 0.4f, 1f);
		} 
		//4x4
		else if (sizeSelect.value == 4) {
			gridRows = 4;
			gridCols = 4;
			offsetX = 2f;
			offsetY = 1.5f;
			originalCard.transform.position = new Vector3 (-3f, 1.5f, -1f);
			originalCard.transform.localScale = new Vector3 (0.3f, 0.3f, 1f);
		} 
		//4x5
		else {
			gridRows = 4;
			gridCols = 5;
			offsetX = 1.7f;
			offsetY = 1.5f;
			originalCard.transform.position = new Vector3 (-3.5f, 1.5f, -1f);
			originalCard.transform.localScale = new Vector3 (0.3f, 0.3f, 1f);
		}
		scoreLabel.text = "Score: " + _score;
		Vector3 startPos = originalCard.transform.position;
		size = gridCols * gridRows;
		perfectScore = size / 2;
		int[] numbers = new int[52];
		//create an array of all cardID
		for (int i=0; i<52; i++) {
			numbers [i] = i;
		}
		//shuffle it
		numbers = ShuffleArray (numbers);

		int[] cardIDs = new int[size];
		//create an array of random cardIDs of the gridsize
		for (int i = 0; i < cardIDs.Length/2; i++) {
			cardIDs [i] = numbers [i];
			cardIDs [cardIDs.Length - i - 1] = numbers [i];
		}
		cardIDs = ShuffleArray (cardIDs);

		for (int i = 0; i < gridCols; i++) {
			for (int j = 0; j < gridRows; j++) {
				MemoryCard card;
				if (i == 0 && j == 0) {
					card = originalCard;
				} 
				else {
					card = Instantiate(originalCard) as MemoryCard;
				}

				int index = j * gridCols + i;
				int id = cardIDs[index];
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);
			}
		}
	}

	void Update() {
		if (_score < perfectScore) {
			_time += Time.deltaTime;
			timeLabel.text = "Time: " + (int)_time;
		}
	}

	private IEnumerator CheckMatch() {
		if (_firstRevealed.id == _secondRevealed.id) {
			_score++;
			scoreLabel.text = "Score: " + _score;
			Vector3 shakeLeft = new Vector3 (0f, 0f, 8f);
			Vector3 shakeRight = new Vector3 (0f, 0f, -8f);
			for (int i = 0; i < 10; i++) {
				_firstRevealed.transform.Rotate (shakeLeft);
				_secondRevealed.transform.Rotate (shakeLeft);
				yield return new WaitForSeconds (.05f);
				_firstRevealed.transform.Rotate (shakeRight);
				_secondRevealed.transform.Rotate (shakeRight);
				yield return new WaitForSeconds (.05f);
			}
			//generate a puff of smoke for another half a second
			_firstRevealed.transform.position = new Vector3 (_firstRevealed.transform.position.x, _firstRevealed.transform.position.y, 10);
			_secondRevealed.transform.position = new Vector3 (_secondRevealed.transform.position.x, _secondRevealed.transform.position.y, 10);
			GameObject smoke1 = Instantiate (smokePrefab) as GameObject;
			GameObject smoke2 = Instantiate (smokePrefab) as GameObject;
			smoke1.transform.localScale = new Vector3 (2.2f, 0.7f, 1f);
			smoke2.transform.localScale = new Vector3 (2.2f, 0.7f, 1f);
			smoke1.transform.position = new Vector3(_firstRevealed.transform.position.x, _firstRevealed.transform.position.y - 2, -10);
			smoke2.transform.position = new Vector3(_secondRevealed.transform.position.x, _secondRevealed.transform.position.y - 2, -10);
			yield return new WaitForSeconds (2.5f);
			GameObject.Destroy (smoke1);
			GameObject.Destroy (smoke2);
			//check if gameOver
			if (_score == perfectScore) {
				victoryImage.transform.position = new Vector3 (0, 0, -10);
			}
		}
		else {
			yield return new WaitForSeconds(.5f);
			_firstRevealed.Unreveal();
			_secondRevealed.Unreveal();
		}
		_firstRevealed = null;
		_secondRevealed = null;
	}
		
	private int[] ShuffleArray(int[] numbers) {
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++ ) {
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}
}
