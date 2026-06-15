using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(menuName = "VN/Character Data")]
public class CharacterData : ScriptableObject                                               //数据继承自 ScriptableObject
{
    public string characterID;
    public Sprite standSprite;
    public Vector2 defaultPositon;//角色位置
    public Vector2 defaultScale = Vector2.one;//角色缩放

    [System.Serializable]                                                                   //可序列化标记，为了方便在Inspector中配置角色表情

    public class Expression//表情类
    {
        public string name;     //表情名字
        public Sprite sprite;   //对应的立绘图片
    }
    public List<Expression> expressionsList = new();    //新建以表情类为孩子的表情列表，内涵表情名字，表情图片
    public Dictionary<string, Sprite> expressions;      //根据不同的stirng类型的表情名字，来查找不同的表情图片；

    private void OnEnable()
    {
        expressions = new Dictionary<string, Sprite>();                                     //新建字典
        foreach (var expr in expressionsList)                                               //遍历表情列表
        {
            if (!expressions.ContainsKey(expr.name))                                        //如果字典中没有表情列表中的名字
                expressions.Add(expr.name, expr.sprite);                                    //将该表情名字，表情图片添加到表情字典中
        }
    }

}
