using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Avatar : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject AvatarMenu; // here is script
    public Button avatarButton; // button to press

    private bool avatarActivated = false;
    public bool menuActivated = false;
    void Start()
    {
        if (AvatarMenu == null) // if its not assigned debug.log 
        {
            Debug.LogError("Avatar Window is not assigned in the Inspector!");
            return;
        }
        AvatarMenu.SetActive(false);
    }

    public void HideAvatar()
    {
        AvatarMenu.SetActive(false);

    }

    public void OpenAvatar()
    {
        avatarActivated = !avatarActivated;
        AvatarMenu.SetActive(avatarActivated);
        InventoryMenu.SetActive(false);
    }
}
