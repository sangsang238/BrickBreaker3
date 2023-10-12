using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public MyBlockController Current { get; set; }

	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI txtCurrLevel;

	public static int score = 0;
	public int Score = score;

	private const int GridSizeX = 10;
	private const int GridSizeY = 21;

	public bool[,] Grid = new bool[GridSizeX, GridSizeY];

	public float GameSpeed => gameSpeed;
	[SerializeField, Range(.1f, 1f)] private float gameSpeed;
	[SerializeField] private Slider sliderGameSpeed;

	[SerializeField] private List<MyBlockController> listPrefabs;
	[SerializeField] private List<MyBlockController> listBrickBlocks;
    //[SerializeField] private List<GameObject> listNextPieces;


    private List<MyBlockController> _listHistory = new List<MyBlockController>();

    //user
    public static MongoDBManager manager = new MongoDBManager();
    public static ModelPlayer mdplayer=null;
    

    //BreakSound Effect
    public GameObject breakSoundEffect;

    //vanishing effect
    public ParticleSystem VanishingEffect;
    public ParticleSystem Effect1;


    //Color Pieces
    public static int colorChoosen = 1;
    ////next pieces
    //   private GameObject currentNextPiece;
    //   private int nextPieceIndex = 0;


    // Report Time
    public static DateTime sessionStartTime;

    #region Test

    public bool IsOpenTest;

	[SerializeField] private SpriteRenderer displayDataPrefabs;
	private SpriteRenderer[,] previewDisplay = new SpriteRenderer[GridSizeX, GridSizeY];
    
	public static int currentLevel = 0;



	private void UpdateDisplayPreview()
	{
		if (!IsOpenTest) return;

		for (int i = 0; i < GridSizeX; i++)
		{
			for (int j = 0; j < GridSizeY; j++)
			{
				var active = Grid[i, j];
				var sprite = previewDisplay[i, j];

				sprite.color = active ? Color.green : Color.red;

				var color = sprite.color;
				color.a = .5f;
				sprite.color = color;

			}
		}
	}

	#endregion



	private void Awake()
	{
		Instance = this;
		if (IsOpenTest)
		{
			for (int i = 0; i < GridSizeX; i++)
			{
				for (int j = 0; j < GridSizeY; j++)
				{
					var sprite = Instantiate(displayDataPrefabs, transform);
					sprite.transform.position = new Vector3(i, j, 0);
					previewDisplay[i, j] = sprite;
				}
			}
		}
	}

	public void Start()
	{
        if (mdplayer!=null)
		{
            Debug.Log(GameManager.mdplayer.PlayerName);
            // Get curr PlaySession and increase by 1
            int a = manager.GetPlaySession(mdplayer.Username);
            manager.SavePlaySession(mdplayer.Username, a + 1);
            // Get curr PlayTime
            sessionStartTime = DateTime.Now;
            // Get curr Color
            if (manager.GetPlayerColor(mdplayer.Username) == 1)
				colorChoosen = 1;
			else if (manager.GetPlayerColor(mdplayer.Username) == 2)
                colorChoosen = 2;
            else if (manager.GetPlayerColor(mdplayer.Username) == 3)
                colorChoosen = 3;
        }
			

        score = 0;
		sliderGameSpeed.onValueChanged.AddListener(UpdateGameSpeed);

		//Nếu đăng nhập rồi chọn level thì mở cái mớ này ra hiện khối chặn
		if (currentLevel>=1)
		{
			SpawnStartingBlock();
			txtCurrLevel.text = "Level: "+ Math.Abs(currentLevel).ToString();
            txtCurrLevel.enabled = true;
		}
				
		else
		{
			Spawn();
			txtCurrLevel.enabled = false;
		}
				
	}


    private void UpdateGameSpeed(float value)
	{
		gameSpeed = value; // Cập nhật giá trị gameSpeed theo giá trị của Slider
	}

	public bool IsInside(List<Vector2> listCoordinate)
	{
		foreach (var coordinate in listCoordinate)
		{
			int x = Mathf.RoundToInt(coordinate.x);
			int y = Mathf.RoundToInt(coordinate.y);

			if (x < 0 || x >= GridSizeX)
			{
				//Horizontal out
				return false;
			}

			if (y < 0 || y >= GridSizeY)
			{
				//Vertical out
				return false;
			}


			if (Grid[x, y])
			{
				//Hit something
				return false;
			}
		}

		return true;
	}


  //  private void ShowNextPiece(int index)
  //  {
  //      if (currentNextPiece != null)
  //      {
  //          Destroy(currentNextPiece);
  //      }
  //      //GameObject currentNextPiece;
  //      var nextPiece = listNextPieces[index];
  //      currentNextPiece = Instantiate(nextPiece);
  //      currentNextPiece.SetActive(true);
  //      currentNextPiece.transform.position = new Vector3(14.25f, 7.95f, 0);
		//Debug.Log(index.ToString());
  //  }
    public void Spawn()
	{
		var index = Random.Range(0, 7);
		if (colorChoosen == 1)
		{
			index = Random.Range(0, 7);
		}
		else if (colorChoosen == 2)
		{
			index = Random.Range(7, 14);
		}
		else if (colorChoosen == 3)
		{
			index = Random.Range(14, 21);
		}
        
        var blockController = listPrefabs[index];
		var newBlock = Instantiate(blockController);
		Current = newBlock;
		_listHistory.Add(newBlock);

		UpdateDisplayPreview();


        //next
        //ShowNextPiece(index);

        //nextPieceIndex = (nextPieceIndex + 1) % listNextPieces.Count;
        //ShowNextPiece((nextPieceIndex + 1) % listNextPieces.Count);
    }

	private bool IsFullRow(int index)
	{
		for (int i = 0; i < GridSizeX; i++)
		{
			if (!Grid[i, index])
				return false;
		}

		return true;
	}

	public void UpdateRemoveObjectController()
	{
		for (int i = 0; i < GridSizeY; i++)
		{
			var isFull = IsFullRow(i);
			if (isFull)
			{
                ParticleSystem EffectClone = Instantiate(Effect1);
                //Remove
                foreach (var myBlock in _listHistory)
				{
					var willDestroy = new List<Transform>(); 
					foreach (var piece in myBlock.ListPiece)
					{
						int y = Mathf.RoundToInt(piece.position.y);
						if (y == i)
						{
							//Add Remove
							willDestroy.Add(piece);
                        }
                        else if (y > i)
						{
							//Move Down
							var position = piece.position;
							position.y--;
							piece.position = position;
						}

					}
					
					//Remove
					foreach (var item in willDestroy)
					{
                        
                        //Debug.Log("" + item.position.ToString());
                        ParticleSystem newClone = Instantiate(VanishingEffect, item.position, Quaternion.identity);
                        myBlock.ListPiece.Remove(item);

                        Destroy(newClone.gameObject, newClone.main.duration);
                        Destroy(item.gameObject);
						
                    }
				}
				//ChangeData
				for (int j = 0; j < GridSizeX; j++)
					Grid[j, i] = false;

				for (int j = i+1; j < GridSizeY; j++)
					for (int k = 0; k < GridSizeX; k++)
						Grid[k, j - 1] = Grid[k, j];

                
                score += 100;
                //UpdateScoreText();
                scoreText.text = score.ToString();
                // Play Break Sound Effect
                Instantiate(breakSoundEffect);
                Destroy(EffectClone.gameObject, EffectClone.main.duration);

				// Check if in starting mode and all starting blocks are removed
				//if(currentLevel==-1)
				//	if (_listHistory.Count >= 1 && _listHistory[0].ListPiece.Count == 0)
				//	{
				//		// Win
				//		Debug.Log("Win1");
				//		SceneManager.LoadScene("GameWinScene1");
				//		return;
				//	}

				if (currentLevel < 0)
				{
					if (_listHistory.Count >= 1 && _listHistory[0].ListPiece.Count == 0)
					{
						// Win

						Debug.Log("Win");
						//SceneManager.LoadScene("GameWinScene1");
                        StartCoroutine(DelayedSceneTransition(1.0f, "GameWinScene1"));
                        return;
					}
				}

				//Call Again
				UpdateRemoveObjectController();
				return;
			}
		}
	}
    IEnumerator DelayedSceneTransition(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }


    //private void UpdateScoreText()
    //{
    //	scoreText.text = score.ToString();
    //}

    private void SpawnStartingBlock()
	{
		//var startingBlockPrefab = listBrickBlocks.Find(block => block.name == "BrickBlock1 Variant");

		var blockIndex = Mathf.Abs(currentLevel);
		if (blockIndex >= 1 && blockIndex <= 12)
		{
			var blockName = $"BrickBlock{blockIndex} Variant";
			var startingBlockPrefab = listBrickBlocks.Find(block => block.name == blockName);
			var newBlock = Instantiate(startingBlockPrefab);
			Current = newBlock;
			_listHistory.Add(newBlock);
			currentLevel = -blockIndex;
			UpdateDisplayPreview();
		}

	}

}