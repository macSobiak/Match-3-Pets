﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameEvent OnClickEvent;
    private bool _inputDisabled = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_inputDisabled)
        {
            var clickedObject = ClickSelect();
            if(clickedObject != null)
                OnClickEvent.Raise(clickedObject);
        }

    }

    GameObject ClickSelect()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

        if (hit)
        {
            Debug.Log(hit.transform.name);
            return hit.transform.gameObject;
        }
        else 
            return null;
    }

    public void EnableInput()
    {
        _inputDisabled = false;
    }
    public void DisableInput()
    {
        _inputDisabled = true;
    }
}
