using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    [SerializeField] private TileBoard currentTilePosition;

    [SerializeField] private float travelTime = 0.5f;

    [SerializeField] private bool isMoving = false;


    private TileBoard _nextTile = null;
    [SerializeField] private int availableMovements = 0;

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if(isMoving == false && availableMovements > 0)
        {
            availableMovements--;
            isMoving = true;
            MoveNextTile();
        }
    }

    //También se implementa la lógica en caminos alternativos
    private void MoveNextTile()
    {
        int numOfRoutes = currentTilePosition.NextTiles.Count;
        if (numOfRoutes == 0) return;
        if (numOfRoutes == 1)
        {
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToTile(_nextTile, travelTime);
        }
        if (numOfRoutes > 1) {
            //A elección del jugador
        }
    }

    private void DisplaceToTile(TileBoard tileTarget, float duration)
    {
        Vector3 newPos = new Vector3(tileTarget.transform.position.x, transform.position.y, tileTarget.transform.position.z);
        if (tileTarget != null)
        {
            StartCoroutine(CinematicAnimation.MoveObject(this.gameObject, newPos, duration, finishMove));
        }
    }

    private void finishMove()
    {
        currentTilePosition = _nextTile;
        _nextTile = null;
        isMoving = false;
    }

}
