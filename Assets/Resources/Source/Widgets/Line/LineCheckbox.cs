using System.Collections.Generic;
using UnityEngine;

using static Root.RegionBackgroundType;

public class LineCheckbox : MonoBehaviour
{
    //Region that this checkbox is asigned to
    public Region region;

    //Frame of the checkbox
    public GameObject frame;

    //Bool field asigned to this checkbox
    public Bool value;

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
        Sound.PlaySound("DesktopButtonPress", 0.8f);
        value.Invert();
    }

    public void RightClick()
    {
        value.Invert();
        referenceList?.ForEach(x => x.Invert());
    }
}
