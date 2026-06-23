using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DropDownHandler))]
public class RoomInfoHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text produceAmountText;
    private DropDownHandler dropDownHandler;

    private Room currentRoom;
    private RoomResourceHandler roomResourceHandler;

    private void Start()
    {
        dropDownHandler = GetComponent<DropDownHandler>();
    }

    public void DisplayInfo(Room room)
    {
        currentRoom = room;
        if (room.gameObject.TryGetComponent(out RoomResourceHandler resourceRoom))
        {
            roomResourceHandler = resourceRoom;
            dropDownHandler.room = resourceRoom;
            dropDownHandler.dropdown.gameObject.SetActive(resourceRoom.canSwitchResourceTypes);
            UpdateProductionAmountDisplay();
        }
        else
        {
            dropDownHandler.dropdown.gameObject.SetActive(true);
        }
        roomNameText.text = room.roomName;
    }

    public void UpdateProductionAmountDisplay()
    {
        if(roomResourceHandler != null)
        {
            switch (roomResourceHandler.roomType)
            {
                case ERoomType.NutritionRoom:
                    produceAmountText.text = roomResourceHandler.nutritionAmount.ToString();
                    break;
                case ERoomType.ResourceRoomWood:
                    produceAmountText.text = roomResourceHandler.woodAmount.ToString();
                    break;
                case ERoomType.ResourceRoomStone:
                    produceAmountText.text = roomResourceHandler.stoneAmount.ToString();
                    break;
                case ERoomType.ResourceRoomMetal:
                    produceAmountText.text = roomResourceHandler.metalAmount.ToString();
                    break;
                case ERoomType.ResearchRoom:
                    produceAmountText.text = roomResourceHandler.plagueVialAmount.ToString();
                    break;
            }
        }
    }

    private void Update()
    {
        if(roomResourceHandler != null)
        {
            progressBar.maxValue = roomResourceHandler.timeToProduce;
            progressBar.value = roomResourceHandler.timeToProduce - roomResourceHandler.currentTime;
        }  
    }
}
