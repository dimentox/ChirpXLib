using System;
using Unity.Entities;
using System.Linq;


namespace System.Runtime.CompilerServices
{
    public class IsUnmanagedAttribute : System.Attribute { }

    public class IsExternalInit { }
}

namespace ChirpXLib.Utils
{

    public static class CWorld
    {
        private static World? _clientWorld;

        public static World Client
        {
            get
            {
                if (_clientWorld != null && _clientWorld.IsCreated)
                    return _clientWorld;

                _clientWorld = GetWorld()
                               ?? throw new System.Exception("There is no Client world (yet).");
                return _clientWorld;
            }
        }

        private static World? GetWorld()
        {
            return World.All.FirstOrDefault(world => world is { Name: "Game" });


        }

        public static World Default => World.DefaultGameObjectInjectionWorld;
        public static World Game => Client;

    }

}
