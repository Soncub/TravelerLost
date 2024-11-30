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
        // Subscribe each divot's PlaceItem and ReleaseItem events to check if the puzzle is solved
        foreach (ItemDivot divot in itemDivots)
        {
            divot.PlaceItemEvent.AddListener(CheckPuzzleSolved);
            divot.ReleaseItemEvent.AddListener(CheckPuzzleSolved); // Add listener for item removal
        }
    }

    private void CheckPuzzleSolved()
    {
        bool allCorrectItemsPlaced = true;

        foreach (ItemDivot divot in itemDivots)
        {
            // If a key divot is missing an item, or a non-key divot has an item, the puzzle is not solved
            if ((divot.isKey && !divot.ItemIsPlaced) || (!divot.isKey && divot.ItemIsPlaced))
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
