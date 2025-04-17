using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    [SerializeField] private TileBoard currentTilePosition;

    [SerializeField] private float travelTime = 0.5f;

    [SerializeField] private bool isMoving = false;


    private TileBoard _nextTile = null;
    private int availableMovements = 0;

    private void Update()
    {
        //MovePlayer();
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

    //Tambi�n se implementa la l�gica en caminos alternativos
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
            //A elecci�n del jugador
        }
    }

    private void DisplaceToTile(TileBoard tileTarget, float duration)
    {
        Vector3 newPos = new Vector3(tileTarget.transform.position.x, transform.position.y, tileTarget.transform.position.z);
        if (tileTarget != null)
        {
            StartCoroutine(CinematicAnimation.MoveTo(this.gameObject, newPos, duration, finishMove));
        }
    }

    private void finishMove()
    {
        currentTilePosition = _nextTile;
        _nextTile = null;
        isMoving = false;
    }

}
