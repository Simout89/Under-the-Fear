using System;
using UnityEngine;
using Unity.Barracuda;


public class MonsterAi : MonoBehaviour
{
    [SerializeField] private NNModel _modelAsset;
    [SerializeField] private MonsterController _monsterController;
    private Model model;
    private IWorker _worker;

    private bool isEnable = true;

    private void Awake()
    {
        model = ModelLoader.Load(_modelAsset);
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, model);
    }

    public void Sink(float a, float b, float c, Vector3 vector3 = new Vector3())
    {
        if(!isEnable)
            return;
        
        float[] inputData = new float[3] {a,b, c};
        
        Tensor inputTensor = new Tensor(1, 3, inputData);
        _worker.Execute(inputTensor);
        Tensor outputTensor = _worker.PeekOutput();

        int action = outputTensor.ArgMax()[0];
        Debug.Log($"Range: {a}, Sound: {b}, Player: {c}");
        Debug.Log($"Решение: {action}");
        if (action != 0)
        {
            _monsterController.ChangeState((MonsterState)action, vector3);
        }

        inputTensor.Dispose();
        outputTensor.Dispose();
    }
    
    void OnDestroy()
    {
        _worker.Dispose();
    }

    public void Disable()
    {
        isEnable = false;
        Debug.Log("Ии монстра выключено");
    }

    public void Enable()
    {
        isEnable = true;
        Debug.Log("Ии монстра включено");

    }
}
