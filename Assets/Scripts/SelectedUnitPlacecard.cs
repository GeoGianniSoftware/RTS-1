using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitPlacecard : MonoBehaviour
{
    Unit referenceUnit;
    Text nameText;

    public void Activate(Unit unitToRef) {
        referenceUnit = unitToRef;
        nameText = transform.GetComponentInChildren<Text>();
        nameText.text = referenceUnit.Name;
    }

    public void OnClick() {
        SelectionManager SM = FindObjectOfType<SelectionManager>();
        SM.TrySelectGameObject(referenceUnit.gameObject, true);
    }
}
