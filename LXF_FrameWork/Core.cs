using LXF_Framework.DependencyInjection;
using LXF_Framework.MonoYield;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace LXF_Framework
{
    namespace FrameworkCore
    {

        public sealed partial class Core : LXF_Singleton<Core>, IDependencyProvider
        {         
            public void InitCore()
            {
                InitializeSingleton(true);

                DataReader = Singleton<LXF_DataReader>.Instance;
                DataWriter = Singleton<LXF_DataWriter>.Instance;
            }

            
            public LXF_DataReader DataReader { get; private set; }
            
            public LXF_DataWriter DataWriter { get; private set; }


            [LXF_Provide(ProvideMode.Method)]
            public LXF_DataReader ProvideDataReader() => DataReader;

            [LXF_Provide(ProvideMode.Method)]
            public LXF_DataWriter ProvideDataWriter() => DataWriter;
        }       
    }

}



