using System;
using UnityEngine;

[Serializable]
public class GameMoment
{
    [SerializeField] private string _momentName;
    private Action _momentAction;
    private bool _continueAction = true;

    public string MomentName { get => _momentName; }
    public Action MomentAction { get => _momentAction; }

    public GameMoment(Action action)
    {
        this._momentName = action.Method.Name;
        this._momentAction = action;
    }

    public void PlayMoment()
    {
        //Declara el inicio
        //Si hay intervensi�n y si es v�lida, Los momentos que intervienen se ponen primeros en la lista y este Momento se desplazar� luego de ellos
        //En caso de que no, se Declara la ejecuci�n de este Momento
        //Al terminar el momento, se Declara la finalizaci�n del Momento
        this._continueAction = true;
        EventManager.TriggerEvent("I_" + _momentName); //InitMoment: Declara ser el siguiente "Momento" en ser ejecutado
        if (!_continueAction) return;
        EventManager.TriggerEvent("C_" + _momentName); //CheckMoment: Declara que no hay intervenciones (Este es el Evento para comprobar si cumple con las condicones para ejecutar este Momento)
        if (!_continueAction) return;
        EventManager.TriggerEvent("P_" + _momentName); //PlayMoment: Declara que cumple con las condiciones para ejecutar este momento y est� a punto de Accionar este "Momento" 
        if (!_continueAction) return;
        this._momentAction.Invoke();
        EventManager.TriggerEvent("E_" + _momentName); //End actions: Declara que ha terminado de ejecutar este Momento satisfactoriamente
        GameManager.Instance.MomentDestroy(this);
        
    }

    public void CancelMoment(){
        this._continueAction = false;
        EventManager.TriggerEvent("S_" + _momentName); //Stop Moment: Declara que este "Momento" ha sido interrumpido pero aún se mantiene en la cola de ejecución
    }

    public void DestroyMoment()
    {
        this._continueAction = false;
        GameManager.Instance.MomentDestroy(this);
        EventManager.TriggerEvent("D_" + _momentName); //Destroy Moment: Declara que este "Momento" ha sido quitado de la Cola de ejecución
    }
}
