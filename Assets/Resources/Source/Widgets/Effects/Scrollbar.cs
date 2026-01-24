using UnityEngine;

public class Scrollbar : MonoBehaviour
{
    //Connected highlightable
    Highlightable highlightable;

    //Reference to the window this scrollbar is handling
    string windowRef;

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

    public void Initialise(int size, int fillSize, string windowRef)
    {
        this.size = size;
        this.fillSize = fillSize;
        this.windowRef = windowRef;
        highlightable = GetComponent<Highlightable>();
    }

    public void FixedUpdate()
    {
        if (highlightable == null) return;
        if (highlightable.pressedState == "Left" || scrollbarUsed == this)
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
            var range = size - fillSize;
            var window = Root.CDesktop.windows.Find(x => x.title == windowRef);
            var maxPagination = window.maxPagination();
            if (maxPagination > 0)
            {
                var percentageOfPaginationDone = 100f / range * -(transform.localPosition.y + 4);
                var paginationResult = (int)(maxPagination * (percentageOfPaginationDone / 100));
                var oldPagination = window.pagination();
                if (oldPagination != paginationResult)
                {
                    window.PreparePagination();
                    window.SetPagination(paginationResult);
                    window.CorrectPagination();
                    window.Respawn();
                    if (oldPagination == maxPagination || paginationResult == maxPagination)
                        Root.Respawn(windowRef + "ScrollbarDown", true);
                    if (oldPagination == 0 || paginationResult == 0)
                        Root.Respawn(windowRef + "ScrollbarUp", true);
                }
            }
        }
        else mousePressOffset = 0;
    }
}
