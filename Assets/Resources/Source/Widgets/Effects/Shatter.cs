using UnityEngine;

using System.Collections;

public class Shatter : MonoBehaviour
{
    //Time that the shatter effect will be delayed by
    public float delay;

    //Time that the shatter effect will be delayed by
    public float travelDelay;

    //Speed at which the shatter effect proceeds
    public float speed;

    //Renderer that will be affected by this effect
    public SpriteRenderer render;

    //Color at the beginning
    private Color startingColor;

    //Color at the end
    private Color aimColor;

    //Overall time this shatter lives
    private float duration;

    //Initiates the shatter effect
    public void Initiate(float speed, float delay = 0, SpriteRenderer r = null)
    {
        this.speed = speed;
        this.delay = delay;
        if (r != null) render = r;
        else render = GetComponent<SpriteRenderer>();
        if (render == null) StartCoroutine(SelfDestruct(speed));
        else
        {
            startingColor = render.color;
            aimColor = new Color(startingColor.r, startingColor.g, startingColor.b, 0);
        }
    }

    public void Update()
    {
        //Wait with executing the effect till the delay is zeroed
        if (delay > 0) delay -= Time.deltaTime;

        //If delay is already dealt with and renderer isn't null..
        else if (render != null)
        {
            //Extend the object duration as it's still alive
            duration += Time.deltaTime;
            
            //Update the color of the object making it fade away with time
            render.color = Color.Lerp(startingColor, aimColor, duration * speed);

            //Remove the object from the scene if it's already transparent
            if (render.color.a <= 0) Destroy(gameObject);
        }
    }

    //Set the object to self destruct in a specified amount of time
    public IEnumerator SelfDestruct(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
