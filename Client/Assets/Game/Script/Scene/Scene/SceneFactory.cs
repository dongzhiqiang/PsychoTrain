using UnityEngine;
using System.Collections;

public class SceneFactory
{
    public static LevelBase GetScene(RoomCfg roomCfg)
    {
        LevelBase scene;
        if (roomCfg.id == "100000")
            scene = new MainCityScene();
        else
            scene = new LevelScene();

        return scene;
    }
}
