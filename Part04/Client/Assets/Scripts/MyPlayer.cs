using System.Collections;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager networkManager;

    void Start()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        StartCoroutine("CoSendPacket");
    }

    void Update()
    {
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);

            var move = new C_Move { posX = Random.Range(-50, 50), posY = 0, posZ = Random.Range(-50, 50) };
            networkManager.Send(move.Write());
        }
    }
}
