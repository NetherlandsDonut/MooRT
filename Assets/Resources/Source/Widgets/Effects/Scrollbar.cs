using UnityEngine;

public class Scrollbar : MonoBehaviour
{
    //Connected highlightable
    Highlightable highlightable;

    //Offset of the mouse when pressed the scrollbar
    float mousePressOffset;

    //Offset of the mouse when pressed the scrollbar
    float startingPositionOffset;

    //Size of the scrollbar
    int size;

    //Size of the scrollbar fill
    int fillSize;

    //Scrollbar that is currently used
    public static Scrollbar scrollbarUsed;

    public void Initialise(int size, int fillSize)
    {
        this.size = size;
        this.fillSize = fillSize;
        highlightable = GetComponent<Highlightable>();
    }

    public void Update()
    {
        if (highlightable == null) return;
        if (highlightable.pressedState == "Left")
        {
            var curScreenSpace = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var curPosition = (Vector2)Root.CDesktop.screen.ScreenToWorldPoint(curScreenSpace);
            if (mousePressOffset == 0)
            {
                scrollbarUsed = this;
                startingPositionOffset = transform.localPosition.y;
                mousePressOffset = curPosition.y;
            }
            transform.localPosition = new Vector3(transform.localPosition.x, startingPositionOffset - mousePressOffset + curPosition.y);
            if (transform.localPosition.y > -4) transform.localPosition = new Vector3(transform.localPosition.x, -4);
            else if (transform.localPosition.y < fillSize - size - 4) transform.localPosition = new Vector3(transform.localPosition.x, fillSize - size - 4);
        }
        else mousePressOffset = 0;
    }
}
