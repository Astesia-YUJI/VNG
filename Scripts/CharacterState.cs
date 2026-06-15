using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterState//角色数据类
{
    public string characterID;
    public int affection;

    public CharacterState(string id)        //构造函数，初始化角色ID和对应的默认好感度
    {
        characterID = id;
        affection = 0;
    }
}
