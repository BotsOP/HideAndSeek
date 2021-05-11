using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text text;
    public Color colorHider;
    public Color colorSeeker;
    private Player player;
    
    public void SetUp(Player _player)
    {
        player = _player;
        SetPlayerText(player);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (targetPlayer != null && targetPlayer == player)
        {
            if (changedProps.ContainsKey("Team"))
            {
                SetPlayerText(player);
            }
        }
    }

    private void SetPlayerText(Player player)
    {
        text.text = player.NickName;
        if (player.CustomProperties.ContainsKey("Team"))
        {
            if ((int)player.CustomProperties["Team"] == 0)
            {
                text.color = colorHider;
                return;
            }
            text.color = colorSeeker;

        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
