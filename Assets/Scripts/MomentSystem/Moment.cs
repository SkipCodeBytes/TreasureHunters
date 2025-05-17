using System;
using UnityEngine;

[Serializable]
public class Moment
{
    [SerializeField] private string _momentName;
    private Action _momentAction;
    private bool _continueAction = true;

    public string MomentName { get => _momentName; }
    public Action MomentAction { get => _momentAction; }

    public Moment(Action action)
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

        Debug.Log("Running " + _momentName);
        this._continueAction = true;
        //InitMoment: Declara ser el siguiente "Momento" en ser ejecutado
        EventManager.TriggerEvent("InitMoment");
        if (!_continueAction) return;
        //CheckMoment: Declara que no hay intervenciones (Este es el Evento para comprobar si cumple con las condicones para ejecutar este Momento)
        EventManager.TriggerEvent("CheckMoment");
        if (!_continueAction) return;
        //PlayMoment: Declara que cumple con las condiciones para ejecutar este momento y est� a punto de Accionar este "Momento"
        EventManager.TriggerEvent("PlayMoment");
        if (!_continueAction) return;
        this._momentAction.Invoke();
        //EndMoment: Declara que ha terminado de ejecutar este Momento satisfactoriamente
        Debug.Log("Ending " + _momentName);
        EventManager.TriggerEvent("EndMoment");

    }

    public void CancelMoment(){
        this._continueAction = false;
        Debug.Log("Cancel " + _momentName);
        //Stop Moment: Declara que este "Momento" ha sido interrumpido pero aún se mantiene en la cola de ejecución
        EventManager.TriggerEvent("StopMoment"); 
    }

}
