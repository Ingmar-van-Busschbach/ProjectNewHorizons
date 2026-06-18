using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockMenuHandler : MonoBehaviour
{
    private Room currentRoomToUnlock;
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text woodToUnlock;
    [SerializeField] private TMP_Text stoneToUnlock;
    [SerializeField] private TMP_Text metalToUnlock;
    [SerializeField] private Button unlockButton;
    public void SetRoomToUnlock(Room newRoomToUnlock)
    {
        currentRoomToUnlock = newRoomToUnlock;
        roomName.text = currentRoomToUnlock.roomName;
        woodToUnlock.text = currentRoomToUnlock.woodToUnlock.ToString();
        stoneToUnlock.text = currentRoomToUnlock.stoneToUnlock.ToString();
        metalToUnlock.text = currentRoomToUnlock.metalToUnlock.ToString();
    }

    private void Update()
    {
        unlockButton.image.color = CheckResourceCount() ? Color.white : Color.gray ;
    }

    private bool CheckResourceCount()
    {
        if(currentRoomToUnlock == null) return false;
        return ResourceManager.instance.wood  > currentRoomToUnlock.woodToUnlock  &&
               ResourceManager.instance.stone > currentRoomToUnlock.stoneToUnlock &&
               ResourceManager.instance.metal > currentRoomToUnlock.metalToUnlock;
    }
    public void UnlockRoom()
    {
        if (CheckResourceCount())
        {
            currentRoomToUnlock.unlockedRoom = true;
        }
    }
}
