using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<itemData> allData;
    public List<InventorySlot> slots;

    int index = 0;
    int previousIndex = 0;

    public float selectHighlight = 1f;
    public float normalSize = 0.85f;
    private InventorySlot previousSlot;

    public Player player;
    public static Inventory instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        previousSlot = slots[0];
        slots[index].ReSize(selectHighlight);
    }
    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (previousSlot != null)
            {
                previousSlot.ReSize(normalSize);
            }
            index = (index - 1 < 0) ? slots.Count - 1 : index - 1;
            previousSlot = slots[index];
            //index = (index - 1 + slots.Count) % slots.Count;
            slots[index].ReSize(selectHighlight);
            player.EqipAxe(slots[index].id == 3);
            
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (previousSlot != null)
            {
                previousSlot.ReSize(normalSize);
            }
            index = (index + 1 > slots.Count-1) ? 0 : index + 1;
            previousSlot = slots[index];
            //index = (index + 1) % slots.Count;
            slots[index].ReSize(selectHighlight);
            player.EqipAxe(slots[index].id == 3);
        }
    }
    public void Use()
    {
        slots[index].Use();
    }
    public bool PickUp(int id)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].id == 0)
            {
                slots[i].Add(getItem(id));
                if (id == 3 && index == i)
                {
                    player.EqipAxe(true);
                }
                return true;
            }
        }
        return false;
    }
    public itemData getItem(int id)
    {
        for (int i = 0; i < allData.Count; i++)
        {
            if (allData[i].id == id)
            {
                return allData[i];
            }
        }
        return new itemData();
    }
}