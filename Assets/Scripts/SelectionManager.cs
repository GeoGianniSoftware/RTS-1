using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    GameObject lastObjectClicked;
    public List<SelectableObject> SelectedObjects = new List<SelectableObject>();
    [SerializeField]
    public RectTransform selectionBox;
    List<GameObject> selectionPlacecards = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        //Test for selection boxes.
        SelectionBoxSetup();
        FillSelectedPortrait();

        if (Input.GetMouseButtonDown(0)) {
            lastObjectClicked = GetGameObjectAtCursor();
            
            if (Input.GetKey(KeyCode.LeftControl) && lastObjectClicked != null) {
                TrySelectGameObject(lastObjectClicked, false);
            }
            else if (lastObjectClicked != null) {
                TrySelectGameObject(lastObjectClicked, true);
            }
        }
        if (Input.GetMouseButtonDown(1) && SelectedObjects.Count > 0) {
            if (GetGameObjectAtCursor() != null && IsSelectable(GetGameObjectAtCursor())) {
                GameObject g = GetGameObjectAtCursor();

                foreach (Unit u in SelectedObjects) {
                    u.Interact(g.GetComponent<SelectableObject>());

                }





            }
            else {
                if (Input.GetKey(KeyCode.LeftControl))
                    CommandSelectedAddMove(GetCursorWorldPosition());
                else
                    CommandSelectedMove(GetCursorWorldPosition());
            }


        }
    }

    Camera oldCamera = null;
    void FillSelectedPortrait() {
        if (SelectedObjects.Count > 0) {
            SelectableObject objectRef = SelectedObjects[0];
            RawImage selectedImage = FindObjectOfType<SelectedDisplayImage>().GetComponent<RawImage>();
            Text selectedName = FindObjectOfType<SelectedNameText>().GetComponent<Text>();
            Image selectedHealthBar = FindObjectOfType<SelectedHealthBar>().GetComponent<Image>();

            selectedName.text = objectRef.Name;
            selectedHealthBar.fillAmount = (float)((float)objectRef.currentHealth / (float)objectRef.maxHealth);
            if(oldCamera != null)
                oldCamera.enabled = false;

            if (objectRef.selectedCamera != null) {
                objectRef.selectedCamera.enabled = true;
                oldCamera = objectRef.selectedCamera;
            }
        }

    }

    void ClearSelectionList() {
        foreach(GameObject g in selectionPlacecards) {
            Destroy(g);
        }
        selectionPlacecards.Clear();
    }

    void FillSelectedUnitList() {
        ClearSelectionList();
        if(SelectedObjects.Count > 1) {
            foreach(SelectableObject u in SelectedObjects) {
                if (u.selectableType != SelectableObject.SelectableType.Unit)
                    return;
                GameObject placecard = Instantiate((GameObject)Resources.Load("UI/SelectionUnitPlacecard")as GameObject, GameObject.FindObjectOfType<SelectedList>().transform, false);
                selectionPlacecards.Add(placecard);
                placecard.GetComponent<SelectedUnitPlacecard>().Activate((Unit)u);
            }
        }
    }

    Vector3 mousePos1, mousePos2;
    void SelectionBoxSetup() {
      
        if (Input.GetMouseButtonDown(0)) {
            mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
               if(hit.point != mousePos1) {
                    return;
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (mousePos1 != mousePos2) {
                EvaluateSelectionBox();
            }
        }
    }

    void EvaluateSelectionBox() {
        print("Selection Box");
        List<SelectableObject> removeObjects = new List<SelectableObject>();

        if (!Input.GetKey(KeyCode.LeftControl)) {
            ClearSelection();
        }

        Rect selectionRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);

        foreach(SelectableObject s in FindObjectsOfType<SelectableObject>()) {
            if(s != null) {
                if (selectionRect.Contains(Camera.main.WorldToViewportPoint(s.transform.position), true)){
                    TrySelectGameObject(s.gameObject, false);
                }
            }
            else {
                removeObjects.Add(s);
            }
        }

        if(removeObjects.Count > 0) {
            foreach(SelectableObject rems in removeObjects) {
                SelectedObjects.Remove(rems);
            }
            removeObjects.Clear();
        }
    }

    public static GameObject GetGameObjectAtCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.gameObject != null)
                return hit.transform.gameObject;
        }
        return null;
    }

    public static Vector3 GetCursorWorldPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.gameObject != null)
                return hit.point;
        }
        return new Vector3(0,-9999,0);
    }

    bool IsSelectable(GameObject g) {
        if (g == null)
            return false;
        if (g.GetComponent<SelectableObject>() != null)
            return true;
        return false;
    }


    void ClearSelection() {
        foreach (SelectableObject s in SelectedObjects) {
            s.SendMessage("DeselectObject", SendMessageOptions.DontRequireReceiver);

        }
        SelectedObjects.Clear();

    }


    void CommandSelectedAddMove(Vector3 posToMove) {
        print("AddMove");


        for (int i = 0; i < SelectedObjects.Count; i++) {
            if (SelectedObjects[i].GetComponent<Unit>() != null) {
                Unit u = SelectedObjects[i].GetComponent<Unit>();
                print(i);

                if(u.Owner == FindObjectOfType<PlayerManager>().players[0])
                    u.AddMove(posToMove, i, SelectedObjects.Count);
            }
                
        }
   
    }
    void CommandSelectedMove(Vector3 posToMove) {

        for (int i = 0; i < SelectedObjects.Count; i++) {

            if (SelectedObjects[i].GetComponent<Unit>() != null) {
                Unit u = SelectedObjects[i].GetComponent<Unit>();
                if (u.Owner == FindObjectOfType<PlayerManager>().players[0])
                    u.MoveToDest(posToMove, i, SelectedObjects.Count);
            }
        }
    }

    public bool TrySelectGameObject(GameObject objectToSelect, bool clearSelect) {
        if (IsSelectable(objectToSelect)) {
            SelectableObject selectable = objectToSelect.GetComponent<SelectableObject>();
            //Check if there was an obj previously selected
            

            if (SelectedObjects.Count > 0 && clearSelect)
                ClearSelection();
            //If yes deselect that object.

            if (SelectedObjects.Contains(selectable)) {
                return false;
            }
            //Select the new object
            SelectedObjects.Add(selectable);
            objectToSelect.SendMessage("SelectObject", SendMessageOptions.DontRequireReceiver);
            FillSelectedUnitList();
            return true;
        }
        else {
            return false;
        }
    }


}
