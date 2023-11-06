using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
    public Color aliveColor, deadColor;

    public bool isCellAlive;
    private bool markedAlive, markedDead;

    public SpriteRenderer sr;

    [SerializeField]
    private int stepsAlive = 0;

    private void Awake()
    {
        sr.color = isCellAlive ? aliveColor : deadColor;
    }

    public void MarkAlive()
    {
        markedAlive = true;
    }

    public void MarkDead()
    {
        markedDead = true;
    }

    public void UpdateCell()
    {
        if (isCellAlive)
        {
            stepsAlive++;
            sr.color = new Color((float)stepsAlive / 120, 0, (float)stepsAlive / 10);
        }
        if (markedAlive)
        {
            ActivateCell();       
        }
        if (markedDead)
        {
            DeactivateCell();
            stepsAlive = 0;
        }
    }

    public void ActivateCell()
    {
        markedAlive = false;
        markedDead = false;
        isCellAlive = true;

        sr.color = aliveColor;
    }

    public void DeactivateCell()
    {
        markedAlive = false;
        markedDead = false;
        isCellAlive = false;

        sr.color = deadColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            if (isCellAlive)
            {
                DeactivateCell();
            } else
            {
                ActivateCell();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCellAlive)
        {
            DeactivateCell();
        } else
        {
            ActivateCell();
        }
    }
}
