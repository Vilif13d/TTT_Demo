using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class NewButton : MonoBehaviour
{
    public Renderer rend;
    [SerializeField]
    private Material mat;

    private bool canUse = true;

    [SerializeField]
    private bool isGrouped; //if grouped with other buttons, only one can be selected at a time. This will retain color when selected and revert other buttons.
    public List<NewButton> otherButtons;
    public bool startSelected;
    public bool isSelected;
    public bool isGrid; //reusing the same script for the grid selections by noting if the button is part of a grid

    public Color startColor;
    public Color hoverColor;
    public Color downColor;

    public Animator anim; //used for animation options
    public string down = "Down";
    public string selected = "Selected";
    public string up = "Up";

    public UnityEvent events;
    // Start is called before the first frame update
    void Start()
    {
        if (rend != null)
        {
            mat = rend.material;
        }
        else
        {
            mat = GetComponent<Renderer>().material;
        }

        startColor = mat.color;
        if (startSelected)
        {
            mat.color = downColor;
            isSelected = true;
            if (anim != null)
            {
                anim.SetTrigger(selected);
            }
        }

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        if(!isSelected && canUse)
            mat.color = hoverColor;
    }

    private void OnMouseDown()
    {
        if (!isSelected && canUse)
        {
            mat.color = downColor;
            if (anim != null)
            {
                anim.SetTrigger(down);
            }
        }
    }

    public void OnMouseUp()
    {
        if (!isGrouped && !isGrid && canUse)
        {
            //basic button function
            mat.color = startColor;
            events.Invoke();
            StartCoroutine(DelayUse());
        }
        else if (isGrid)
        {
            //if the button is part of the grid
            mat.color = downColor;
            isSelected = true;
            events.Invoke();
        }
        else if (!isSelected)
        {
            //if the button is grouped with others, un select the others
            foreach (NewButton button in otherButtons)
            {
                button.mat.color = button.startColor;
                button.isSelected = false;
                if (button.anim != null)
                {
                    button.anim.SetTrigger(up);
                }
            }
            mat.color = downColor;
            isSelected = true;
            if (anim != null)
            {
                anim.SetTrigger(selected);
            }
            events.Invoke();
        }
    }

    private void OnMouseExit()
    {
        if (!isGrouped || !isSelected)
        {
            mat.color = startColor;
        }
        else
        {
            mat.color = downColor;
        }
    }

    public void ResetButton()
    {
        //resets buttons on the grid for next game
        if (isGrid)
        {
            isSelected = false;
            mat.color = startColor;
        }
    }

    IEnumerator DelayUse()
    {
        //for buttons like the start button, this will prevent the button from being used multiple times before the game starts,
        //and then auto reset so it can be used again when transitioning back to the start screen
        canUse = false;
        yield return new WaitForSeconds(2);
        canUse = true;
    }
}
