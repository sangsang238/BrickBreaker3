using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyBlockController : MonoBehaviour
{
	public List<Transform> ListPiece => listPiece;
	[SerializeField] private List<Transform> listPiece = new List<Transform>();

	//Drop sound effect
    public GameObject dropSoundEffect;

    private void Start()
	{
		StartCoroutine(MoveDown());
	}

	public IEnumerator MoveDown()
	{
		while (true)
		{
			var delay = GameManager.Instance.GameSpeed;
			yield return new WaitForSeconds(delay);

			var isMovable = GameManager.Instance.IsInside(GetPreviewPosition());
			if (isMovable)
                Move();
			else
			{
                Instantiate(dropSoundEffect);
                foreach (var piece in listPiece)
				{
					int x = Mathf.RoundToInt(piece.position.x);
					int y = Mathf.RoundToInt(piece.position.y);

					if (y >= GameManager.Instance.Grid.GetLength(1))
					{
                        //GameOver
                        Time.timeScale = 1f;
                        //SceneManager.LoadScene("GameOverScene1");
                        StartCoroutine(DelayedSceneTransition(1.5f, "GameOverScene1"));
                        yield break;
					}

					GameManager.Instance.Grid[x, y] = true;
				}

				GameManager.Instance.UpdateRemoveObjectController();

                GameManager.Instance.Spawn();

				break;
			}
		}
	}
    IEnumerator DelayedSceneTransition(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    private List<Vector2> GetPreviewPosition()
	{
		var result = new List<Vector2>();
		foreach (var piece in listPiece)
		{
			var position = piece.position;
			position.y--;
			result.Add(position);
		}

		return result;
	}

	private void Move()
	{
		var position = transform.position;
		position.y--;
		transform.position = position;
	}

}