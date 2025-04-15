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

    public GameMoment(string name, Action action)
    {
        this._momentName = name;
        this._momentAction = action;
    }

    public void PlayMoment()
    {
        //Declara el inicio
        //Si hay intervensión y si es válida, Los momentos que intervienen se ponen primeros en la lista y este Momento se desplazará luego de ellos
        //En caso de que no, se Declara la ejecución de este Momento
        //Al terminar el momento, se Declara la finalización del Momento
        this._continueAction = true;
        EventManager.TriggerEvent("I_" + _momentName); //InitMoment: Declara ser el siguiente "Momento" en ser ejecutado
        if (!_continueAction) return;
        EventManager.TriggerEvent("C_" + _momentName); //CheckMoment: Declara que no hay intervenciones (Este es el Evento para comprobar si cumple con las condicones para ejecutar este Momento)
        if (!_continueAction) return;
        EventManager.TriggerEvent("P_" + _momentName); //PlayMoment: Declara que cumple con las condiciones para ejecutar este momento y está a punto de Accionar este "Momento" 
        if (!_continueAction) return;
        this._momentAction.Invoke();
        EventManager.TriggerEvent("E_" + _momentName); //End actions: Declara que ha terminado de ejecutar este Momento satisfactoriamente
    }

    public void StopMoment()
    {
        this._continueAction = false;
        EventManager.TriggerEvent("S_" + _momentName); //Stop Moment: Declara que este "Momento" ha sido interrumpido
    }
}
