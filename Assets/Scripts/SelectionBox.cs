using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    private RectTransform selectSquare;
    Vector3 startPos;
    Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        selectSquare = FindObjectOfType<SelectionManager>().selectionBox;
        selectSquare.GetComponent<Image>().enabled = true;
        selectSquare.gameObject.SetActive(false);

    }

    // Update is called once per frame
    bool started;

    void Update(){
        if (selectSquare == null)
            return;

        if (Input.GetMouseButtonDown(0) && SelectionManager.GetGameObjectAtCursor() != null) {
            startPos = SelectionManager.GetCursorWorldPosition();
            started = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            selectSquare.gameObject.SetActive(false);
            started = false;
        }

        if (Input.GetMouseButton(0) && started) {
            if(!selectSquare.gameObject.activeInHierarchy )
                selectSquare.gameObject.SetActive(true);

            endPos = Input.mousePosition;
            Vector3 squareStart = Camera.main.WorldToScreenPoint(startPos);
            squareStart.z = 0;

            Vector3 center = (squareStart + endPos) / 2f;

            selectSquare.position = center;

            float sizeX = Mathf.Abs(squareStart.x - endPos.x);
            float sizeY = Mathf.Abs(squareStart.y - endPos.y);

            selectSquare.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }
}
