using TMPro;
using UnityEngine;

public class DropDownHandler : MonoBehaviour
{
    [Tooltip("Put in the room that the dropdown is controlling")]
    public RoomResourceHandler room;
    public TMP_Dropdown dropdown;

    public void GetDropdownValue()
    {
        int pickedEntry = dropdown.value;

        switch (pickedEntry) 
        {
            case 0:
                room.roomType = ERoomType.ResourceRoomWood;
                break;
            case 1:
                room.roomType = ERoomType.ResourceRoomStone;
                break;
            case 2:
                room.roomType = ERoomType.ResourceRoomMetal;
                break;
        }
        room.currentTime = room.timeToProduce;
    }
}
