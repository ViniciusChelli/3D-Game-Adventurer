using UnityEngine;

public class FadeController : MonoBehaviour
{
    public Animator anim;
    public bool fadeIn; //se true fadein, se false fadeout

    private void Start()
    {
        if(fadeIn)
        {
            anim.Play("fadeIn");
        }
        else
        {
            anim.Play("fadeOut");
        }
    }

}
