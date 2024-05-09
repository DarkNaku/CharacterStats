using System.Collections;
using System.Collections.Generic;
using DarkNaku.Stat;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StatTest
{
    public enum STAT_TYPE { POWER, HEALTH, INTELIGENCE, WISDOM, AGILITY, LUCKY }

    [Test]
    public void _00_생성_테스트()
    {
        var stat = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 100);

        Assert.AreEqual(100, stat.BaseValue);
        Assert.AreEqual(100, stat.Value);
    }

    [Test]
    public void _01_Add_테스트()
    {
        var stat = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 100);

        stat.Add(new Modifier(ModifierType.Add, 5));

        Assert.AreEqual(100, stat.BaseValue);
        Assert.AreEqual(105, stat.Value);

        stat.Add(new Modifier(ModifierType.Add, 10));

        Assert.AreEqual(115, stat.Value);
    }

    [Test]
    public void _01_Percent_테스트()
    {
        var stat = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 100);

        stat.Add(new Modifier(ModifierType.Percent, 0.15f));

        Assert.AreEqual(100, stat.BaseValue);
        Assert.AreEqual(115, stat.Value);

        stat.Add(new Modifier(ModifierType.Percent, 0.25f));

        Assert.AreEqual(140, stat.Value);
    }

    [Test]
    public void _02_Multiply_테스트()
    {
        var stat = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 125);

        stat.Add(new Modifier(ModifierType.Multiply, 0.1f));

        Assert.AreEqual(125, stat.BaseValue);
        Assert.AreEqual(137.5f, stat.Value);

        stat.Add(new Modifier(ModifierType.Multiply, 0.1f));
        Assert.AreEqual(151.25f, stat.Value);
    }

    [Test]
    public void _03_종합_테스트()
    {
        var stat = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 100);

        Assert.AreEqual(100, stat.BaseValue);

        stat.Add(new Modifier(ModifierType.Add, 25));

        Assert.AreEqual(125, stat.Value);

        stat.Add(new Modifier(ModifierType.Percent, 0.15f));

        Assert.AreEqual(143.75f, stat.Value);

        stat.Add(new Modifier(ModifierType.Percent, 0.05f));

        Assert.AreEqual(150f, stat.Value);

        stat.Add(new Modifier(ModifierType.Multiply, 0.05f));

        Assert.AreEqual(157.5f, stat.Value);

        stat.Add(new Modifier(ModifierType.Multiply, 0.15f));

        Assert.AreEqual(181.125f, stat.Value);
    }

    [Test]
    public void _04_상속_테스트()
    {
        var parent = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 85);
        var stat = new Stat<STAT_TYPE>(parent);

        Assert.AreEqual(85, stat.BaseValue);
        Assert.AreEqual(85, stat.Value);

        parent.Add(new Modifier(ModifierType.Add, 15));

        Assert.AreEqual(100, parent.Value);
        Assert.AreEqual(100, stat.BaseValue);
        Assert.AreEqual(100, stat.Value);

        stat.Add(new Modifier(ModifierType.Add, 25));

        Assert.AreEqual(125, stat.Value);

        stat.Add(new Modifier(ModifierType.Percent, 0.15f));

        Assert.AreEqual(143.75f, stat.Value);

        stat.Add(new Modifier(ModifierType.Percent, 0.05f));

        Assert.AreEqual(150f, stat.Value);

        stat.Add(new Modifier(ModifierType.Multiply, 0.05f));

        Assert.AreEqual(157.5f, stat.Value);

        stat.Add(new Modifier(ModifierType.Multiply, 0.15f));

        Assert.AreEqual(181.125f, stat.Value);

        Assert.AreEqual(85, parent.BaseValue);
        Assert.AreEqual(100, parent.Value);
    }

    [Test]
    public void _05_후수정_테스트()
    {
        var parent = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 0);
        var stat = new Stat<STAT_TYPE>(parent, 100);

        Assert.AreEqual(0, parent.BaseValue);
        Assert.AreEqual(0, parent.Value);
        Assert.AreEqual(100, stat.BaseValue);
        Assert.AreEqual(100, stat.Value);

        parent.Add(new Modifier(ModifierType.Percent, 0.25f).SetPost(true));

        Assert.AreEqual(0, parent.BaseValue);
        Assert.AreEqual(0, parent.Value);
        Assert.AreEqual(100, stat.BaseValue);
        Assert.AreEqual(125, stat.Value);

        var parent2 = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 100);
        var stat2 = new Stat<STAT_TYPE>(parent2);

        Assert.AreEqual(100, parent2.BaseValue);
        Assert.AreEqual(100, parent2.Value);
        Assert.AreEqual(100, stat2.BaseValue);
        Assert.AreEqual(100, stat2.Value);

        parent2.Add(new Modifier(ModifierType.Percent, 0.25f).SetPost(true));

        Assert.AreEqual(100, parent2.BaseValue);
        Assert.AreEqual(125, parent2.Value);
        Assert.AreEqual(100, stat2.BaseValue);
        Assert.AreEqual(125, stat2.Value);

        var stat2Changed = false;

        stat2.OnChangeValue.AddListener((s) => 
        { 
            stat2Changed = true;
        });

        parent2.Add(new Modifier(ModifierType.Percent, 0.25f).SetPost(true));

        Assert.IsTrue(stat2Changed);

        Assert.AreEqual(100, parent2.BaseValue);
        Assert.AreEqual(150, parent2.Value);
        Assert.AreEqual(100, stat2.BaseValue);
        Assert.AreEqual(150, stat2.Value);
    }

    [Test]
    public void _06_이벤트_테스트()
    {
        var parent = new Stat<STAT_TYPE>(STAT_TYPE.POWER, 0);
        var stat = new Stat<STAT_TYPE>(parent);

        Assert.AreEqual(0, parent.BaseValue);
        Assert.AreEqual(0, parent.Value);
        Assert.AreEqual(0, stat.BaseValue);
        Assert.AreEqual(0, stat.Value);

        stat.Add(new Modifier(ModifierType.Add, 100f));

        Assert.AreEqual(0, parent.BaseValue);
        Assert.AreEqual(0, parent.Value);
        Assert.AreEqual(0, stat.BaseValue);
        Assert.AreEqual(100, stat.Value);

        var statChanged = false;

        stat.OnChangeValue.AddListener(s => statChanged = true);

        parent.Add(new Modifier(ModifierType.Percent, 0.25f).SetPost(true));

        Assert.IsTrue(statChanged);
        Assert.AreEqual(0, parent.BaseValue);
        Assert.AreEqual(0, parent.Value);
        Assert.AreEqual(0, stat.BaseValue);
        Assert.AreEqual(125, stat.Value);
    }
}