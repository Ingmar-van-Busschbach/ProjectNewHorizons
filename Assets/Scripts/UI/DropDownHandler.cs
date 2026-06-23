using TMPro;
using UnityEngine;

public class DropDownHandler : MonoBehaviour
{
    [Tooltip("Put in the room that the dropdown is controlling")]
    [SerializeField] private RoomResourceHandler room;
    [SerializeField] private TMP_Dropdown dropdown;

    public void GetDropdownValue()
    {
        int pickedEntry = dropdown.value;

        switch (pickedEntry) 
        {
            case 0:
                room.roomType = ERoomType.ResourceRoomWood;
                room.roomAmbience.clip = room.woodAudioClip;
                room.roomAmbience.Play();
                break;
            case 1:
                room.roomType = ERoomType.ResourceRoomStone;
                room.roomAmbience.clip = room.stoneAudioClip;
                room.roomAmbience.Play();
                break;
            case 2:
                room.roomType = ERoomType.ResourceRoomMetal;
                room.roomAmbience.clip = room.metalAudioClip;
                room.roomAmbience.Play();
                break;
        }
    
    }
}
