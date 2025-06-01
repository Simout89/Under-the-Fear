using UnityEngine;

public class BTNfx : MonoBehaviour
{

    public AudioSource Btn;
    public AudioClip Enr;

    public void Ener()
    {
        Btn.PlayOneShot(Enr);
    }    

   }
