using UnityEngine;

public class BadGuyStats : MonoBehaviour
{
    public GameObject weapon;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void ShowWeapon(bool show)
    {
        if (weapon != null)
        {
            weapon.SetActive(show);
        }
    }
}
