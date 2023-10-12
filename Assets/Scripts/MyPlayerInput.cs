using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class MyPlayerInput : MonoBehaviour
{
    private bool isPressingLeft = false;
    private bool isPressingRight = false;
    private bool isPressingUp = false;
    private bool isPressingDown = false;
    //private bool isTouchingScreen = false;

    public float continuousMoveDelay = 0.001f; // Thời gian trễ giữa các lần di chuyển liên tục
    private float continuousMoveTimer = 0f;

    private void Update()
    {
        HandleInput();

        if (isPressingLeft)
        {
            continuousMoveTimer += Time.deltaTime;
            if (continuousMoveTimer >= continuousMoveDelay)
            {
                MoveHorizontal(-1);
                continuousMoveTimer = 0f;
            }
        }
        else if (isPressingRight)
        {
            continuousMoveTimer += Time.deltaTime;
            if (continuousMoveTimer >= continuousMoveDelay)
            {
                MoveHorizontal(1);
                continuousMoveTimer = 0f;
            }
        }
        else if (isPressingUp)
        {
                Rotate();

        }
        else if (isPressingDown)
        {
            continuousMoveTimer += Time.deltaTime;
            if (continuousMoveTimer >= continuousMoveDelay)
            {
                MoveDown();
                continuousMoveTimer = 0f;
            }
        }
    }

    private void HandleInput()
    {
        isPressingLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        isPressingRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        isPressingUp = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow);
        isPressingDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);


        //// on mobile phone
        //// Xử lý các sự kiện cảm ứng
        //if (Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0);

        //    // Xoay khối gạch khi chạm vào màn hình
        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        Rotate();
        //        isTouchingScreen = true;
        //    }

        //    // Kiểm tra hướng di chuyển của ngón tay và di chuyển khối gạch tương ứng
        //    if (touch.phase == TouchPhase.Moved)
        //    {
        //        Vector2 deltaPosition = touch.deltaPosition;

        //        if (Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y))
        //        {
        //            if (deltaPosition.x > 0)
        //            {
        //                isPressingRight = true;
        //                isPressingLeft = false;
        //            }
        //            else
        //            {
        //                isPressingLeft = true;
        //                isPressingRight = false;
        //            }
        //        }
        //        else
        //        {
        //            if (deltaPosition.y > 0)
        //            {
        //                isPressingUp = true;
        //                isPressingDown = false;
        //            }
        //            else
        //            {
        //                isPressingDown = true;
        //                isPressingUp = false;
        //            }
        //        }
        //    }

        //    // Kết thúc chạm màn hình
        //    if (touch.phase == TouchPhase.Ended)
        //    {
        //        isPressingLeft = false;
        //        isPressingRight = false;
        //        isPressingUp = false;
        //        isPressingDown = false;
        //        isTouchingScreen = false;
        //    }
        //}

        //// Xử lý sự kiện nhấn phím bàn phím trên điện thoại nếu có
        //if (!isTouchingScreen)
        //{
        //    isPressingLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        //    isPressingRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        //    isPressingUp = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow);
        //    isPressingDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        //}

    }


    private List<Vector2> GetPreviewPosition()
	{
		var result = new List<Vector2>();
		var listPiece = GameManager.Instance.Current.ListPiece;
		var pivot = GameManager.Instance.Current.transform.position;
		foreach (var piece in listPiece)
		{
			var position = piece.position;

			position -= pivot;
			position = new Vector3(position.y, -position.x, 0);
			position += pivot;
			
			result.Add(position);
		}
		return result;
	}

	private List<Vector2> GetPreviewHorizontalPosition(int value)
	{
		var result = new List<Vector2>();
		var listPiece = GameManager.Instance.Current.ListPiece;
		foreach (var piece in listPiece)
		{
			var position = piece.position;
			position.x += value;
			result.Add(position);
		}

		return result;
	}
    private List<Vector2> GetPreviewDownPosition()
    {
        var result = new List<Vector2>();
        var listPiece = GameManager.Instance.Current.ListPiece;
        foreach (var piece in listPiece)
        {
            var position = piece.position;
            position.y--;
            result.Add(position);
        }

        return result;
    }
    private void MoveHorizontal(int value)
    {
        var isMovable = GameManager.Instance.IsInside(GetPreviewHorizontalPosition(value));
        if (isMovable)
        {
            var current = GameManager.Instance.Current.transform;
            var position = current.position;
            position.x += value;
            current.position = position;
        }
    }

    private void Rotate()
    {
        var isRotatable = GameManager.Instance.IsInside(GetPreviewPosition());
        if (isRotatable)
        {
            var current = GameManager.Instance.Current.transform;
            var angles = current.eulerAngles;
            angles.z += -90;
            current.eulerAngles = angles;
        }
    }


    private void MoveDown()
    {
        var isMovable = GameManager.Instance.IsInside(GetPreviewDownPosition());
        if (isMovable)
        {
            var current = GameManager.Instance.Current.transform;
            var position = current.position;
            position.y--;
            current.position = position;
        }
    }

}