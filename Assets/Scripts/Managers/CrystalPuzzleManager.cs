using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrystalPuzzleManager : MonoBehaviour
{
    [Tooltip("List of all item divots in the puzzle.")]
    [SerializeField] private List<ItemDivot> itemDivots;

    [Tooltip("Event triggered when all correct divots have items placed.")]
    public UnityEvent onPuzzleSolved;

    private void Start()
    {
        // Subscribe each divot's PlaceItem method to check if the puzzle is solved whenever an item is placed
        foreach (ItemDivot divot in itemDivots)
        {
            if (divot.isKey)
            {
                // Optionally you can check initial status in case items are already placed
                divot.PlaceItemEvent.AddListener(CheckPuzzleSolved);
            }
        }
    }

    private void CheckPuzzleSolved()
    {
        // Check if all key divots have items placed
        bool allCorrectItemsPlaced = true;
        foreach (ItemDivot divot in itemDivots)
        {
            if (divot.isKey && !divot.ItemIsPlaced)
            {
                allCorrectItemsPlaced = false;
                break;
            }
        }

        if (allCorrectItemsPlaced)
        {
            Debug.Log("Puzzle Solved!");
            onPuzzleSolved.Invoke();
        }
    }
}
