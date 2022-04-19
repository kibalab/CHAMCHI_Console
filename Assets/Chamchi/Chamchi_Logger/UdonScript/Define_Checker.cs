using System;
using UdonSharp;
using UnityEngine;

namespace Chamchi.Chamchi_Logger.UdonScript
{
    public class Define_Checker : UdonSharpBehaviour
    {
        public void Start()
        {
#if CHAMCHI_CONSOLE
            Debug.Log("[Define Checker] OK");
#else
            Debug.Log("[Define Checker] CAN NOT FIND CONSOLE");
#endif
        }
    }
}