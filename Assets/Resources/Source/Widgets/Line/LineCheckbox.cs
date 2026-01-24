using System.Collections.Generic;

using UnityEngine;

public class LineCheckbox : MonoBehaviour
{
    //Region that this checkbox is asigned to
    public Region region;

    //Frame of the checkbox
    public GameObject frame;

    //Bool field asigned to this checkbox
    public Bool value;

    //When inverting the list the corresponding bool is in, this is the list
    public List<Bool> referenceList;

    //Initialisation method
    public void Initialise(Region region, Bool value, List<Bool> referenceList)
    {
        this.value = value;
        this.region = region;
        this.referenceList = referenceList;
        region.checkbox = this;
    }

    //Event called on interacting with the checkbox
    public void OnMouseUp()
    {
        //Invert the field value
        value.Invert();
        if (Root.CDesktop.title == "PrepareArtistBattle") Root.CDesktop.RespawnAll();
        else if (Root.WindowUp(region.regionGroup.window.title + "Scrollbar")) Root.CDesktop.RespawnAll();
        Root.CDesktop.RespawnAllScrollbarRelatedWindows();
    }

    public void RightClick()
    {
        value.Invert();
        referenceList?.ForEach(x => x.Invert());
        if (Root.CDesktop.title == "PrepareArtistBattle") Root.CDesktop.RespawnAll();
        else if (Root.WindowUp(region.regionGroup.window.title + "Scrollbar")) Root.CDesktop.RespawnAll();
        Root.CDesktop.RespawnAllScrollbarRelatedWindows();
    }
}
